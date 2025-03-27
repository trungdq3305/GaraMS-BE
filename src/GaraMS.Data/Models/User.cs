﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GaraMS.Data.Models;

[Index("Email", Name = "UQ__Users__A9D10534D44B0854", IsUnique = true)]
[Index("UserName", Name = "UQ__Users__C9F28456327A5883", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string UserName { get; set; }

    [Required]
    [StringLength(255)]
    public string Password { get; set; }

    [StringLength(100)]
    public string Email { get; set; }

    [StringLength(20)]
    public string PhoneNumber { get; set; }

    [StringLength(100)]
    public string FullName { get; set; }

    [StringLength(255)]
    public string Address { get; set; }

    public bool? Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column("RoleID")]
    public int? RoleId { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    [InverseProperty("User")]
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    [InverseProperty("User")]
    public virtual ICollection<Gara> Garas { get; set; } = new List<Gara>();

    [InverseProperty("User")]
    public virtual ICollection<InventoryInvoice> InventoryInvoices { get; set; } = new List<InventoryInvoice>();

    [InverseProperty("User")]
    public virtual ICollection<Manager> Managers { get; set; } = new List<Manager>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual UserRole Role { get; set; }
}