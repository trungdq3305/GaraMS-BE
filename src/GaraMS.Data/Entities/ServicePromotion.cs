using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class ServicePromotion
{
    public int ServicePromotionId { get; set; }

    public int? ServiceId { get; set; }

    public int? PromotionId { get; set; }

    public virtual Promotion? Promotion { get; set; }

    public virtual Service? Service { get; set; }
}
