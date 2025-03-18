using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.InvoicesService
{
    public class InvoiceService : IInvoicesService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public InvoiceService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string> CreatePaymentUrl(int invoiceId, decimal totalAmount)
        {
            try
            {
                var clientId = _config["PayPal:ClientId"];
                var secret = _config["PayPal:Secret"];

                // Get access token
                var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{secret}"));
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

                var tokenRequest = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                var tokenResponse = await _httpClient.PostAsync("https://api-m.sandbox.paypal.com/v1/oauth2/token", tokenRequest);

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    var errorContent = await tokenResponse.Content.ReadAsStringAsync();
                    throw new InvalidOperationException($"Failed to get PayPal access token. Status: {tokenResponse.StatusCode}, Error: {errorContent}");
                }

                var tokenResult = await tokenResponse.Content.ReadAsStringAsync();
                var tokenData = JsonSerializer.Deserialize<JsonElement>(tokenResult);

                if (!tokenData.TryGetProperty("access_token", out var accessTokenElement))
                {
                    throw new InvalidOperationException($"Invalid token response format: {tokenResult}");
                }

                var accessToken = accessTokenElement.GetString();

                // Create order
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");

                var requestBody = new
                {
                    intent = "CAPTURE",
                    purchase_units = new[]
                    {
                        new
                        {
                            reference_id = invoiceId.ToString(),
                            description = $"Invoice #{invoiceId}",
                            amount = new
                            {
                                currency_code = "USD",
                                value = totalAmount.ToString("F2")
                            }
                        }
                    },
                    application_context = new
                    {
                        brand_name = "Gara Management System",
                        landing_page = "LOGIN",
                        user_action = "PAY_NOW",
                        return_url = "http://localhost:3000/invoice/success",
                        cancel_url = "http://localhost:3000/invoice/fail"
                    }
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://api-m.sandbox.paypal.com/v2/checkout/orders", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new InvalidOperationException($"Failed to create PayPal order. Status: {response.StatusCode}, Error: {errorContent}");
                }

                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"PayPal Response: {result}");

                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(result);

                if (jsonResponse.TryGetProperty("links", out var links))
                {
                    var approvalLink = links.EnumerateArray()
                        .FirstOrDefault(link =>
                            link.TryGetProperty("rel", out var rel) &&
                            rel.GetString() == "approve" &&
                            link.TryGetProperty("href", out _));

                    if (approvalLink.TryGetProperty("href", out var href))
                    {
                        return href.GetString();
                    }
                }

                throw new InvalidOperationException($"Approval URL not found in PayPal response: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreatePaymentUrl: {ex}");
                throw;
            }
        }

        public async Task<PaymentResponse> CapturePayment(string token)
        {
            try
            {
                var clientId = _config["PayPal:ClientId"];
                var secret = _config["PayPal:Secret"];

                // Get access token
                var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{secret}"));
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

                var tokenRequest = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                var tokenResponse = await _httpClient.PostAsync("https://api-m.sandbox.paypal.com/v1/oauth2/token", tokenRequest);

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException("Failed to get PayPal access token");
                }

                var tokenResult = await tokenResponse.Content.ReadAsStringAsync();
                var tokenData = JsonSerializer.Deserialize<JsonElement>(tokenResult);
                var accessToken = tokenData.GetProperty("access_token").GetString();

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var captureResponse = await _httpClient.PostAsync($"https://api-m.sandbox.paypal.com/v2/checkout/orders/{token}/capture", new StringContent("", Encoding.UTF8, "application/json"));
                
                var captureResult = await captureResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Capture response: {captureResult}");

                if (!captureResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to capture payment. Status: {captureResponse.StatusCode}");
                    throw new InvalidOperationException($"Failed to capture payment: {captureResult}");
                }

                var captureData = JsonSerializer.Deserialize<JsonElement>(captureResult);
                Console.WriteLine($"Capture data: {JsonSerializer.Serialize(captureData)}");

                var purchaseUnit = captureData.GetProperty("purchase_units")[0];
                var referenceId = purchaseUnit.GetProperty("reference_id").GetString();
                var status = captureData.GetProperty("status").GetString();

                Console.WriteLine($"Final status: {status}, ReferenceId: {referenceId}");

                return new PaymentResponse
                {
                    ReferenceId = referenceId,
                    Status = status
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error capturing payment: {ex.Message}");
                throw;
            }
        }
    }
}
