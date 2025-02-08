using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class Staff
{
    public int StaffId { get; set; }

    public string? Gender { get; set; }

    public decimal? Salary { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
