using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public decimal? Salary { get; set; }

    public int? SpecializedId { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<ServiceEmployee> ServiceEmployees { get; set; } = new List<ServiceEmployee>();

    public virtual Specialized? Specialized { get; set; }

    public virtual User? User { get; set; }
}
