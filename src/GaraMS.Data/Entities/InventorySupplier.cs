using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class InventorySupplier
{
    public int InventorySupplierId { get; set; }

    public int? InventoryId { get; set; }

    public int? SupplierId { get; set; }

    public virtual Inventory? Inventory { get; set; }

    public virtual Supplier? Supplier { get; set; }
}
