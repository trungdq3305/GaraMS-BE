using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class InvoiceDetail
{
    public int InvoiceDetailId { get; set; }

    public decimal? Price { get; set; }

    public int? InvoiceId { get; set; }

    public virtual Invoice? Invoice { get; set; }
}
