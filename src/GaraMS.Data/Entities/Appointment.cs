﻿using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public DateTime? Date { get; set; }

    public string? Note { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? WaitingTime { get; set; }

    public string? RejectReason { get; set; }

    public int? VehicleId { get; set; }

    public int? AppointmentStatusId { get; set; }

    public int? InvoiceId { get; set; }

    public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();

    public virtual AppointmentStatus? AppointmentStatus { get; set; }

    public virtual Invoice? Invoice { get; set; }

    public virtual Vehicle? Vehicle { get; set; }
}
