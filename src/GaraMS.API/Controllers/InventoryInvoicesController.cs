using GaraMS.Data.Models;
using GaraMS.Service.Services.AccountService;
using GaraMS.Service.Services.TokenService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace GaraMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryInvoicesController : ControllerBase
    {
        private readonly GaraManagementSystemContext _context;
        private readonly ITokenService _token;
        private readonly IAccountService _accountService;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        public InventoryInvoicesController(GaraManagementSystemContext context, ITokenService token, IAccountService accountService, IConfiguration config, HttpClient httpClient)
        {
            _context = context;
            _token = token;
            _accountService = accountService;
            _config = config;
            _httpClient = httpClient;
        }
        [HttpPut("DiliverType")]
        public async Task<IActionResult> EditDiliverType([FromQuery] string diliverType)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var decodeModel = _token.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 1 });

            if (!isValidRole)
            {
                return Unauthorized("Bạn không có quyền thực hiện thao tác này.");
            }

            var useid = Convert.ToInt32(decodeModel.userid);
            var ii = await _context.InventoryInvoices.FirstOrDefaultAsync(x => (x.UserId == useid && x.Status != "False"));


            // Kiểm tra nếu không tìm thấy InventoryInvoice hoặc Inventory
            if (ii == null)
            {
                return NotFound("Không tìm thấy InventoryInvoice của người dùng.");
            }
            var inventoryInvoiceDetails = await _context.InventoryInvoiceDetails.Include(x => x.Inventory)
                .Include(x => x.InventoryInvoice).ThenInclude(x => x.User)
                .Where(x => (x.InventoryInvoice.UserId == useid && x.InventoryInvoice.Status != "False")).ToListAsync();
            decimal? total = 0;
            foreach (var item in inventoryInvoiceDetails)
            {
                total += item.Price;
            }

            ii.TotalAmount = total;
            ii.DiliverType = diliverType;
            _context.InventoryInvoices.Update(ii);
            await _context.SaveChangesAsync();
            return StatusCode(201, ii.DiliverType); // Trả về 201 Created thay vì 200
        }

        [HttpPut("PaymentMethod")]
        public async Task<IActionResult> EditPaymentMethod([FromQuery] string paymentMethod)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var decodeModel = _token.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 1 });

            if (!isValidRole)
            {
                return Unauthorized("Bạn không có quyền thực hiện thao tác này.");
            }

            var useid = Convert.ToInt32(decodeModel.userid);
            var ii = await _context.InventoryInvoices.FirstOrDefaultAsync(x => (x.UserId == useid && x.Status != "False"));


            // Kiểm tra nếu không tìm thấy InventoryInvoice hoặc Inventory
            if (ii == null)
            {
                return NotFound("Không tìm thấy InventoryInvoice của người dùng.");
            }
            var inventoryInvoiceDetails = await _context.InventoryInvoiceDetails.Include(x => x.Inventory)
                .Include(x => x.InventoryInvoice).ThenInclude(x => x.User)
                .Where(x => (x.InventoryInvoice.UserId == useid && x.InventoryInvoice.Status != "False")).ToListAsync();
            decimal? total = 0;
            foreach (var item in inventoryInvoiceDetails)
            {
                total += item.Price;
            }

            ii.TotalAmount = total;
            ii.PaymentMethod = paymentMethod;
            _context.InventoryInvoices.Update(ii);
            await _context.SaveChangesAsync();
            return StatusCode(201, ii.PaymentMethod); // Trả về 201 Created thay vì 200
        }
        [HttpGet]
        public async Task<IActionResult> GetAllInventoryInvoices()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var decodeModel = _token.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 3 });

            if (!isValidRole)
            {
                return Unauthorized("Bạn không có quyền truy cập.");
            }

            var inventoryInvoices = await _context.InventoryInvoices
                .Include(x => x.User)
                .Include(x => x.InventoryInvoiceDetails)
                .ThenInclude(d => d.Inventory)
                .Where(x => (x.Status == "False"))
                .ToListAsync();

            return Ok(inventoryInvoices);
        }
        [HttpGet("of-customer")]
        public async Task<IActionResult> GetAllUserInventoryInvoices()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var decodeModel = _token.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 1 });
            var useid = Convert.ToInt32(decodeModel.userid);
            if (!isValidRole)
            {
                return Unauthorized("Bạn không có quyền truy cập.");
            }

            var inventoryInvoices = await _context.InventoryInvoices
                .Include(x => x.User)
                .Include(x => x.InventoryInvoiceDetails)
                .ThenInclude(d => d.Inventory)
                .Where(x => (x.Status == "False" && x.UserId == useid))
                .ToListAsync();

            return Ok(inventoryInvoices);
        }
        [HttpPut("Edit-to-false")]
        public async Task<IActionResult> EditStatus()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var decodeModel = _token.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 1 });

            if (!isValidRole)
            {
                return Unauthorized("Bạn không có quyền thực hiện thao tác này.");
            }

            var useid = Convert.ToInt32(decodeModel.userid);
            var ii = await _context.InventoryInvoices.FirstOrDefaultAsync(x => (x.UserId == useid && x.Status != "False"));


            // Kiểm tra nếu không tìm thấy InventoryInvoice hoặc Inventory
            if (ii == null)
            {
                return NotFound("Không tìm thấy InventoryInvoice của người dùng.");
            }
            var inventoryInvoiceDetails = await _context.InventoryInvoiceDetails.Include(x => x.Inventory)
                .Include(x => x.InventoryInvoice).ThenInclude(x => x.User)
                .Where(x => (x.InventoryInvoice.UserId == useid && x.InventoryInvoice.Status != "False")).ToListAsync();
            foreach (var item in inventoryInvoiceDetails)
            {
                var inven = await _context.Inventories.AsNoTracking().FirstOrDefaultAsync(x => x.InventoryId == item.InventoryId);
                if (inven != null)
                {
                    inven.Unit = (int.Parse(inven.Unit) - 1).ToString();
                    using (var newContext = new GaraManagementSystemContext())
                    {
                        newContext.Inventories.Update(inven);
                        var a = new InventoryWarranty
                        {
                            StartDay = DateTime.Now,
                            EndDay = DateTime.Now.AddDays((double)inven.WarrantyPeriod),
                            Status = true,
                            InventoryInvoiceDetailId = item.InventoryInvoiceDetailId
                        };
                        newContext.InventoryWarranties.Add(a);
                        await newContext.SaveChangesAsync();
                    }
                }
                

            }

            ii.Status = "False";
            var newii = new InventoryInvoice
            {
                Price = 0,
                DiliverType = "Pending",
                PaymentMethod = "Pending",
                TotalAmount = 0,
                Status = "True",
                UserId = useid
            };

            _context.InventoryInvoices.Update(ii);
            _context.InventoryInvoices.Add(newii);
            await _context.SaveChangesAsync();



            return StatusCode(201, ii.Status); // Trả về 201 Created thay vì 200
        }

        [HttpPost("payment")]
        public async Task<IActionResult> PaySingleInvoice()
        {
            try
            {
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var decodeModel = _token.decode(token);
                var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 1 });
                var useid = Convert.ToInt32(decodeModel.userid);
                var a = await _context.InventoryInvoices.FirstOrDefaultAsync(x => (x.Status != "False" && x.UserId == useid));
                var iiId = a.InventoryInvoiceId;
                decimal totalAmount = (decimal)a.TotalAmount;

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
                            reference_id = iiId.ToString(),
                            description = $"Invoice #{iiId}",
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
                        return_url = "https://gara-ms-fe-three.vercel.app/invoice/inventorysuccess",
                        cancel_url = "https://gara-ms-fe-three.vercel.app/invoice/fail"
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
                        return Ok(href.GetString());
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

        [HttpGet("Get-Inven-Warranties")]
        public async Task<IActionResult> GetAllInvenWarrantyOfUser()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var decodeModel = _token.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 1 });
            var useid = Convert.ToInt32(decodeModel.userid);
            if (!isValidRole)
            {
                return Unauthorized("Bạn không có quyền truy cập.");
            }
            var list = await _context.InventoryWarranties.Include(a => a.InventoryInvoiceDetail)
                .ThenInclude(a => a.InventoryInvoice)
                .Where(a => a.InventoryInvoiceDetail.InventoryInvoice.UserId == useid).ToListAsync();
            return Ok(list);
        }
    }
}
