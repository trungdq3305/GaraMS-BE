﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GaraMS.Data.Models;

[Table("WarrantyHistory")]
public partial class WarrantyHistory
{
    [Key]
    public int WarrantyHistoryId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDay { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDay { get; set; }

    public string Note { get; set; }

    public bool? Status { get; set; }

    public int? ServiceId { get; set; }

    [ForeignKey("ServiceId")]
    [InverseProperty("WarrantyHistories")]
    public virtual Service Service { get; set; }
}