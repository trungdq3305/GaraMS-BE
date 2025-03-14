using GaraMS.Data.Models;
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
        public async Task<IActionResult> PaymentSuccess([FromQuery] string token, [FromQuery] string payerId)
        {
            try
            {
                var response = await _invoiceService.CapturePayment(token);

                if (response != null && response.Status == "COMPLETED")
                {
                    var invoiceId = int.Parse(response.ReferenceId);

                    var invoice = await _context.Invoices.Include(i=>i.Appointment)
                        .ThenInclude(i => i.AppointmentServices)
                        .ThenInclude(i => i.Service).Where(i => i.InvoiceId == invoiceId)
                .FirstOrDefaultAsync();
                    if (invoice != null)
                    {
                        invoice.Status = "Paid";
                        invoice.PaymentMethod = "PayPal";

                        _context.ChangeTracker.Clear();
                        var appointment = await _context.Appointments.FindAsync(invoice.AppointmentId);
                        if (appointment != null)
                        {
                            appointment.Status = "Paid";
                            _context.Appointments.Update(appointment);
                            await _context.SaveChangesAsync();
                        }
                        _context.ChangeTracker.Clear();
                        _context.Invoices.Update(invoice);
                        await _context.SaveChangesAsync();
                    }

                    return Ok(invoice);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Payment error: {ex.Message}");
                return Ok(new { url ="fail" });
            }

            return Ok(new { url = "fail" });
        }

        [HttpGet("payment-cancel")]
        public IActionResult PaymentCancel()
        {
            return Redirect("http://localhost:3000/invoice/fail");
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
    }
}
