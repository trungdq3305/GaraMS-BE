using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime? Date { get; set; }

    public string? PaymentMethod { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Status { get; set; }

    public int? FeedbackId { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Customer? Customer { get; set; }

    public virtual Feedback? Feedback { get; set; }

    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
}
