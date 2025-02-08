using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
