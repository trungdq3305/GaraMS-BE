using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? FullName { get; set; }

    public string? Address { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Gara> Garas { get; set; } = new List<Gara>();

    public virtual UserRole? Role { get; set; }

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
