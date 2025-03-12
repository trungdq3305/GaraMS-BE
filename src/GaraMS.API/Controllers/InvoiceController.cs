using GaraMS.Data.Models;
using GaraMS.Service.Services.InvoicesService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace GaraMS.API.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoicesService _invoiceService;
        private readonly GaraManagementSystemContext _context;

        public InvoiceController(IInvoicesService invoiceService, GaraManagementSystemContext context)
        {
            _invoiceService = invoiceService;
            _context = context;
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

        [HttpGet("payment-success")]
        public async Task<IActionResult> PaymentSuccess([FromQuery] string token, [FromQuery] string PayerID)
        {
            try
            {
                // Lấy order ID từ token trong URL callback của PayPal
                var response = await _invoiceService.CapturePayment(token);

                if (response != null)
                {
                    // Lấy reference_id (invoiceId) từ response
                    var invoiceId = int.Parse(response.ReferenceId);

                    // Cập nhật trạng thái invoice
                    var invoice = await _context.Invoices.FindAsync(invoiceId);
                    if (invoice != null)
                    {
                        invoice.Status = "Paid";
                        invoice.PaymentMethod = "PayPal";
                        await _context.SaveChangesAsync();
                    }

                    // Chuyển hướng về trang thành công
                    return Redirect($"https://localhost:3000/payment/success?invoiceId={invoiceId}");
                }
            }
            catch (Exception ex)
            {
                // Log lỗi và chuyển hướng về trang thất bại
                Console.WriteLine($"Payment error: {ex.Message}");
                return Redirect("https://localhost:3000/payment/error");
            }

            return Redirect("https://localhost:3000/payment/error");
        }

        [HttpGet("payment-cancel")]
        public IActionResult PaymentCancel()
        {
            // Chuyển hướng về trang hủy thanh toán
            return Redirect("https://localhost:3000/payment/cancel");
        }
    }
}
