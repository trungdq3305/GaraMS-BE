using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class Specialized
{
    public int SpecializedId { get; set; }

    public string SpecializedName { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
