using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class ServiceInventory
{
    public int ServiceInventoryId { get; set; }

    public int? InventoryId { get; set; }

    public int? ServiceId { get; set; }

    public virtual Inventory? Inventory { get; set; }

    public virtual Service? Service { get; set; }
}
