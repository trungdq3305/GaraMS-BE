using GaraMS.Service.Services.InvoicesService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace GaraMS.API.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceService _invoiceService;

        public InvoiceController(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpPost("pay-single-invoice/{invoiceId}")]
        public async Task<IActionResult> PaySingleInvoice(int invoiceId, [FromBody] decimal totalAmount)
        {
            var paymentUrl = await _invoiceService.CreatePaymentUrl(invoiceId, totalAmount);
            return Ok(new { url = paymentUrl });
        }
    }
}
