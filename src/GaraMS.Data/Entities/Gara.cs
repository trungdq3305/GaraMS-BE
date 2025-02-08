using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class Gara
{
    public int GaraId { get; set; }

    public string GaraNumber { get; set; } = null!;

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
