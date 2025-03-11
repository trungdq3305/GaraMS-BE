﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GaraMS.Data.Models;

[Table("ServiceInventory")]
public partial class ServiceInventory
{
    [Key]
    public int ServiceInventoryId { get; set; }

    public int? InventoryId { get; set; }

    public int? ServiceId { get; set; }

    [ForeignKey("InventoryId")]
    [InverseProperty("ServiceInventories")]
    public virtual Inventory Inventory { get; set; }

    [ForeignKey("ServiceId")]
    [InverseProperty("ServiceInventories")]
    public virtual Service Service { get; set; }
}