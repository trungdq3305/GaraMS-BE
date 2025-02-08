using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string Name { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<InventorySupplier> InventorySuppliers { get; set; } = new List<InventorySupplier>();
}
