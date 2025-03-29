﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GaraMS.Data.Models;

public partial class Employee
{
    [Key]
    public int EmployeeId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Salary { get; set; }

    public int? SpecializedId { get; set; }

    public int? UserId { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();

    [InverseProperty("Employee")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("Employee")]
    public virtual ICollection<EmployeeShift> EmployeeShifts { get; set; } = new List<EmployeeShift>();

    [InverseProperty("Employee")]
    public virtual ICollection<ServiceEmployee> ServiceEmployees { get; set; } = new List<ServiceEmployee>();

    [ForeignKey("SpecializedId")]
    [InverseProperty("Employees")]
    public virtual Specialized Specialized { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Employees")]
    public virtual User User { get; set; }
}