﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GaraMS.Data.Models;

public partial class Customer
{
    [Key]
    public int CustomerId { get; set; }

    [StringLength(10)]
    public string Gender { get; set; }

    public string Note { get; set; }

    public int? UserId { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [InverseProperty("Customer")]
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    [ForeignKey("UserId")]
    [InverseProperty("Customers")]
    public virtual User User { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}