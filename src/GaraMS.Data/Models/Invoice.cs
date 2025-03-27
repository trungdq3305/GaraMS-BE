﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GaraMS.Data.Models;

[Index("AppointmentId", Name = "UQ__Invoices__8ECDFCC3E9748FC0", IsUnique = true)]
public partial class Invoice
{
    [Key]
    public int InvoiceId { get; set; }

    public int? AppointmentId { get; set; }

    public int? CustomerId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Date { get; set; }

    [StringLength(50)]
    public string PaymentMethod { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TotalAmount { get; set; }

    [StringLength(50)]
    public string Status { get; set; }

    [ForeignKey("AppointmentId")]
    [InverseProperty("Invoice")]
    public virtual Appointment Appointment { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Invoices")]
    public virtual Customer Customer { get; set; }
}