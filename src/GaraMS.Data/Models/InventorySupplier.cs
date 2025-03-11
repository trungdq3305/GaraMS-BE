﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GaraMS.Data.Models;

[Table("InventorySupplier")]
public partial class InventorySupplier
{
    [Key]
    public int InventorySupplierId { get; set; }

    public int? InventoryId { get; set; }

    public int? SupplierId { get; set; }

    [ForeignKey("InventoryId")]
    [InverseProperty("InventorySuppliers")]
    public virtual Inventory Inventory { get; set; }

    [ForeignKey("SupplierId")]
    [InverseProperty("InventorySuppliers")]
    public virtual Supplier Supplier { get; set; }
}