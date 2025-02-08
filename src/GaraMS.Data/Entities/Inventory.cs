using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Unit { get; set; }

    public decimal? Price { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<InventorySupplier> InventorySuppliers { get; set; } = new List<InventorySupplier>();

    public virtual ICollection<ServiceInventory> ServiceInventories { get; set; } = new List<ServiceInventory>();
}
