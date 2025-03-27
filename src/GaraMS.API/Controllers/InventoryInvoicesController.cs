using GaraMS.Data.Models;
using GaraMS.Service.Services.AccountService;
using GaraMS.Service.Services.TokenService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public InventoryInvoicesController(GaraManagementSystemContext context, ITokenService token, IAccountService accountService)
        {
            _context = context;
            _token = token;
            _accountService = accountService;
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
                .Where(x => (x.InventoryInvoice.UserId == useid)).ToListAsync();
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
                .Where(x => (x.InventoryInvoice.UserId == useid)).ToListAsync();
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
                .Include(x => x.User) // Nếu cần thông tin User
                .Include(x => x.InventoryInvoiceDetails) // Include danh sách chi tiết hóa đơn
                .ThenInclude(d => d.Inventory) // Nếu muốn lấy thông tin Inventory của từng chi tiết
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
    }
}
