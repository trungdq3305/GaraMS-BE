using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.InvoicesService
{
    public interface IInvoicesService
    {
        public Task<string> CreatePaymentUrl(int invoiceId, decimal totalAmount);
        public Task<PaymentResponse> CapturePayment(string token);
    }

    public class PaymentResponse
    {
        public string ReferenceId { get; set; }
        public string Status { get; set; }
    }
}
