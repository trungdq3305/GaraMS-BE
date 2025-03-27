using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.AppointmentModel;
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
    public class InventoryInvoiceDetailsController : ControllerBase
    {
        private readonly GaraManagementSystemContext _context;
        private readonly ITokenService _token;
        private readonly IAccountService _accountService;
        public InventoryInvoiceDetailsController(GaraManagementSystemContext context, ITokenService token, IAccountService accountService)
        {
            _context = context;
            _token = token;
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserInventoryInvoiceDetails()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var decodeModel = _token.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 1 });
            var useid = Convert.ToInt32(decodeModel.userid);
            var inventoryInvoiceDetails = await _context.InventoryInvoiceDetails.Include(x => x.Inventory)
                .Include(x => x.InventoryInvoice).ThenInclude(x => x.User)
                .Where(x => (x.InventoryInvoice.UserId == useid && x.InventoryInvoice.Status != "False")).ToListAsync();

            return StatusCode(200, inventoryInvoiceDetails);
        }
        [HttpPost]
        public async Task<IActionResult> AddInventoryToDetail([FromQuery] int inventoryId)
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
            var aa = await _context.Inventories.FirstOrDefaultAsync(x => x.InventoryId == inventoryId);

            // Kiểm tra nếu không tìm thấy InventoryInvoice hoặc Inventory
            if (ii == null)
            {
                return NotFound("Không tìm thấy InventoryInvoice của người dùng.");
            }

            if (aa == null)
            {
                return NotFound("Không tìm thấy Inventory.");
            }

            if (Convert.ToInt32(aa.Unit) <= 0)
            {
                return NotFound("Hết Inventory.");
            }
            var inventoryInvoiceDetails = await _context.InventoryInvoiceDetails.Include(x => x.Inventory)
                .Include(x => x.InventoryInvoice).ThenInclude(x => x.User)
                .Where(x => (x.InventoryInvoice.UserId == useid && x.InventoryInvoice.Status != "False")).ToListAsync();

            foreach (var item in inventoryInvoiceDetails)
            {
                var inven = await _context.Inventories.AsNoTracking().FirstOrDefaultAsync(x => x.InventoryId == inventoryId);
                if (inven != null)
                {
                    var inventorys = await _context.InventoryInvoiceDetails.Include(x => x.Inventory)
                .Include(x => x.InventoryInvoice).ThenInclude(x => x.User)
                .Where(x => (x.InventoryInvoice.UserId == useid && x.InventoryInvoice.Status != "False" && x.InventoryId == inventoryId)).ToListAsync();
                    if (int.Parse(inven.Unit) <= inventorys.Count())
                    {
                        return NotFound("hết để thêm Inventory.");
                    }
                    
                }
            }
            var newInD = new InventoryInvoiceDetail
            {
                InventoryId = inventoryId,
                InventoryInvoiceId = ii.InventoryInvoiceId,
                Price = aa.Price
            };

            _context.InventoryInvoiceDetails.Add(newInD);
            await _context.SaveChangesAsync();
            return StatusCode(201, newInD); // Trả về 201 Created thay vì 200
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteInventoryFromDetail([FromQuery] int inventoryInvoiceDetailId)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var decodeModel = _token.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 1 });

            if (!isValidRole)
            {
                return Unauthorized("Bạn không có quyền thực hiện thao tác này.");
            }

            var useid = Convert.ToInt32(decodeModel.userid);

            var iid = await _context.InventoryInvoiceDetails.FirstOrDefaultAsync(x => x.InventoryInvoiceDetailId == inventoryInvoiceDetailId);    



            _context.InventoryInvoiceDetails.Remove(iid);
            await _context.SaveChangesAsync();
            return StatusCode(201, "Remove success");
        }

        [HttpGet("total")]
        public async Task<IActionResult> TotalInventoryInvoiceDetails()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var decodeModel = _token.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 1 });
            var useid = Convert.ToInt32(decodeModel.userid);
            var inventoryInvoiceDetails = await _context.InventoryInvoiceDetails.Include(x => x.Inventory)
                .Include(x => x.InventoryInvoice).ThenInclude(x => x.User)
                .Where(x => (x.InventoryInvoice.UserId == useid && x.InventoryInvoice.Status != "False")).ToListAsync();
            decimal? total = 0;
            foreach (var item in inventoryInvoiceDetails)
            {
                total += item.Price;
            }

            return StatusCode(200, total);
        }
    }
}
