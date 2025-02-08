using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class AppointmentService
{
    public int AppointmentServiceId { get; set; }

    public int? ServiceId { get; set; }

    public int? AppointmentId { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual Service? Service { get; set; }
}
