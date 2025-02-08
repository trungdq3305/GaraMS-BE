using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class ServiceEmployee
{
    public int ServiceEmployeeId { get; set; }

    public int? ServiceId { get; set; }

    public int? EmployeeId { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Service? Service { get; set; }
}
