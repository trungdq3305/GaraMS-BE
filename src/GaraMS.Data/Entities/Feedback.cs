using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public string? Comment { get; set; }

    public int? Rating { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
