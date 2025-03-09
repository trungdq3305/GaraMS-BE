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
    }
}
