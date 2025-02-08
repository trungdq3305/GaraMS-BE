using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class WarrantyHistory
{
    public int WarrantyHistoryId { get; set; }

    public DateTime? StartDay { get; set; }

    public DateTime? EndDay { get; set; }

    public string? Note { get; set; }

    public bool? Status { get; set; }

    public int? ServiceId { get; set; }

    public virtual Service? Service { get; set; }
}
