﻿using GaraMS.Data.Models;
using GaraMS.Data.Repositories.AppointmentRepo;
using GaraMS.Service.Services.InvoicesService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;

namespace GaraMS.API.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoicesService _invoiceService;
        private readonly IAppointmentRepo _appointmentRepo;
        private readonly IConfiguration _config;
        private readonly GaraManagementSystemContext _context;


        public InvoiceController(IAppointmentRepo appointmentRepo,IInvoicesService invoiceService, GaraManagementSystemContext context, IConfiguration config)
        {
            _appointmentRepo = appointmentRepo;
            _invoiceService = invoiceService;
            _context = context;
            _config = config;
        }

        [HttpPost("pay-single-invoice/{invoiceId}")]
        public async Task<IActionResult> PaySingleInvoice(int invoiceId)
        {
            var invoice = await _context.Invoices
                .Where(i => i.InvoiceId == invoiceId)
                .FirstOrDefaultAsync();

            if (invoice == null)
            {
                return NotFound(new { message = "Invoice not found" });
            }

            if (!invoice.TotalAmount.HasValue || invoice.TotalAmount <= 0)
            {
                return BadRequest(new { message = "Invalid total amount" });
            }

            var paymentUrl = await _invoiceService.CreatePaymentUrl(invoiceId, invoice.TotalAmount.Value);
            return Ok(new { url = paymentUrl });
        }
        [HttpGet("iid-by-aid")]
        public async Task<IActionResult> GetIid(int aid)
        {
            
                var invoice = await _context.Invoices
                .Where(i => i.AppointmentId == aid)
                .FirstOrDefaultAsync();
                if (invoice == null)
                {
                return Ok();
                }
                var iid = invoice.InvoiceId;
                return Ok(iid);
            
            
        }
        [HttpPost("payment-success")]
        public async Task<IActionResult> VerifyPayment([FromQuery] string token, [FromQuery] string PayerID)
        {
            try
            {
                Console.WriteLine($"Starting verify-payment with token: {token}, PayerID: {PayerID}");

                // Kiểm tra trạng thái order
                var clientId = _config["PayPal:ClientId"];
                var secret = _config["PayPal:Secret"];
                var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{secret}"));

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

                    // Lấy access token
                    var tokenContent = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                    var tokenResponse = await httpClient.PostAsync("https://api-m.sandbox.paypal.com/v1/oauth2/token", tokenContent);
                    var tokenResult = await tokenResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Token response: {tokenResult}");

                    var tokenData = JsonSerializer.Deserialize<JsonElement>(tokenResult);
                    var accessToken = tokenData.GetProperty("access_token").GetString();

                    // Kiểm tra trạng thái order
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    var orderResponse = await httpClient.GetAsync($"https://api-m.sandbox.paypal.com/v2/checkout/orders/{token}");
                    var orderResult = await orderResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Order response: {orderResult}");

                    var orderData = JsonSerializer.Deserialize<JsonElement>(orderResult);

                    if (!orderData.TryGetProperty("status", out var status))
                    {
                        Console.WriteLine("Status not found in order response");
                        return Ok(new { success = false, redirectUrl = "https://gara-ms-fe-three.vercel.app/invoice/fail" });
                    }

                    var orderStatus = status.GetString();
                    Console.WriteLine($"Order status: {orderStatus}");

                    if (orderStatus == "CREATED")
                    {
                        Console.WriteLine("Payment not completed: Order still in CREATED state");
                        return Ok(new { success = false, redirectUrl = "https://gara-ms-fe-three.vercel.app/invoice/fail" });
                    }

                    try
                    {
                        Console.WriteLine("Attempting to capture payment...");
                        var response = await _invoiceService.CapturePayment(token);
                        Console.WriteLine($"Capture payment response: {JsonSerializer.Serialize(response)}");

                        if (response != null && int.TryParse(response.ReferenceId, out int invoiceId))
                        {
                            Console.WriteLine($"Processing invoice ID: {invoiceId}");

                            var invoice = await _context.Invoices
                                .Include(i => i.Appointment)
                                .ThenInclude(a => a.AppointmentServices)
                                .ThenInclude(s => s.Service)
                                .Include(i => i.Customer)
                                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

                            if (invoice != null)
                            {
                                invoice.Status = "Paid";
                                invoice.PaymentMethod = "PayPal";

                                if (invoice.Appointment != null)
                                {
                                    invoice.Appointment.Status = "Paid";

                                    var appointmentServices = await _context.AppointmentServices
                                        .Where(a => a.AppointmentId == invoice.AppointmentId)
                                        .ToListAsync();

                                    var serviceIds = appointmentServices.Select(a => a.ServiceId).Distinct().ToList();
                                    var serviceInventories = await _context.ServiceInventories
                                        .Where(si => serviceIds.Contains(si.ServiceId))
                                        .ToListAsync();

                                    // Đếm số lần xuất hiện của mỗi InventoryId trong ServiceInventories
                                    var inventoryUsageCount = serviceInventories
                                        .GroupBy(si => si.InventoryId)
                                        .ToDictionary(g => g.Key, g => g.Count());

                                    var inventoryIds = inventoryUsageCount.Keys.ToList();
                                    var inventories = await _context.Inventories
                                        .Where(inv => inventoryIds.Contains(inv.InventoryId))
                                        .ToListAsync();

                                    foreach (var inventory in inventories)
                                    {
                                        if (int.TryParse(inventory.Unit, out int currentUnits) && currentUnits > 0)
                                        {
                                            int deduction = inventoryUsageCount[inventory.InventoryId]; // Số lần trừ thực tế
                                            inventory.Unit = Math.Max(currentUnits - deduction, 0).ToString(); // Đảm bảo không âm
                                            _context.Inventories.Update(inventory);
                                        }
                                    }
                                }


                                _context.Invoices.Update(invoice);
                                _context.Appointments.Update(invoice.Appointment);
                                await _context.SaveChangesAsync();

                                Console.WriteLine($"Successfully updated invoice {invoiceId} and appointment");
                                return Ok(invoice);
                            }
                            else
                            {
                                Console.WriteLine($"Invoice not found for ID: {invoiceId}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Capture payment response is null or invalid");
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error during capture or database update: {ex.Message}");
                        Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    }

                    return Ok(new { success = false, redirectUrl = "https://gara-ms-fe-three.vercel.app/invoice/fail" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in verify-payment: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Ok(new { success = false, redirectUrl = "https://gara-ms-fe-three.vercel.app/invoice/fail" });
            }
        }

        [HttpGet("by-appointment/{appointmentId}")]
        public async Task<IActionResult> GetInvoiceByAppointmentId(int appointmentId)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Appointment)
                .Where(i => i.AppointmentId == appointmentId)
                .FirstOrDefaultAsync();

            if (invoice == null)
            {
                return NotFound(new { message = "No invoice found for this appointment" });
            }

            return Ok(invoice);
        }
    }
}
