using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var clientId = _config["PayPal:ClientId"];
            var secret = _config["PayPal:Secret"];

            // Authenticate with PayPal
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{secret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

            var tokenRequest = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
            var tokenResponse = await _httpClient.PostAsync("https://api-m.sandbox.paypal.com/v1/oauth2/token", tokenRequest);
            var tokenResult = await tokenResponse.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<JsonElement>(tokenResult);
            var accessToken = tokenData.GetProperty("access_token").GetString();

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            // Create PayPal payment
            var requestBody = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                new
                {
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
                    return_url = "https://localhost:5001/api/invoices/payment-success",
                    cancel_url = "https://localhost:5001/api/invoices/payment-cancel"
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api-m.sandbox.paypal.com/v2/checkout/orders", content);
            var result = await response.Content.ReadAsStringAsync();

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(result);
            var approvalUrl = jsonResponse.GetProperty("links").EnumerateArray()
                .FirstOrDefault(link => link.GetProperty("rel").GetString() == "approve")
                .GetProperty("href").GetString();

            return approvalUrl;
        }
    }
}
