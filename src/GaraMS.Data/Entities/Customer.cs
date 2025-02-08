using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string? Gender { get; set; }

    public string? Note { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual User? User { get; set; }

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
