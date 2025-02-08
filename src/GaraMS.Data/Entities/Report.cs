﻿using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class Report
{
    public int ReportId { get; set; }

    public string? Problem { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CustomerId { get; set; }

    public virtual Customer? Customer { get; set; }
}
