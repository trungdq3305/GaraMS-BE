using GaraMS.Data.Models;
using GaraMS.Data.Repositories.AppointmentRepo;
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
        private readonly IAppointmentRepo _appointmentRepo;
        private readonly GaraManagementSystemContext _context;

        public InvoiceController(IAppointmentRepo appointmentRepo,IInvoicesService invoiceService, GaraManagementSystemContext context)
        {
            _appointmentRepo = appointmentRepo;
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

        [HttpPost("payment-success")]
        public async Task<IActionResult> PaymentSuccess([FromQuery] string token)
        {
            try
            {
                var response = await _invoiceService.CapturePayment(token);

                if (response != null)
                {
                    var invoiceId = int.Parse(response.ReferenceId);

                    var invoice = await _context.Invoices.FindAsync(invoiceId);
                    if (invoice != null)
                    {
                        invoice.Status = "Paid";
                        invoice.PaymentMethod = "PayPal";

                        var appointment = await _context.Appointments.FindAsync(invoice.AppointmentId);
                        if (appointment != null)
                        {
                            appointment.Status = "Paid";
                        }

                        await _context.SaveChangesAsync();
                    }

                    return Redirect("http://localhost:3000/invoice/success");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Payment error: {ex.Message}");
                return Redirect("http://localhost:3000/invoice/fail");
            }

            return Redirect("http://localhost:3000/invoice/fail");
        }

        [HttpGet("payment-cancel")]
        public IActionResult PaymentCancel()
        {
            return Redirect("http://localhost:3000/invoice/fail");
        }
    }
}
