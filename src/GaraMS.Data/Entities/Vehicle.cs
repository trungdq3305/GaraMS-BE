using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    public string PlateNumber { get; set; } = null!;

    public string? Brand { get; set; }

    public string? Model { get; set; }

    public int? CustomerId { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Customer? Customer { get; set; }
}
