using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class Promotion
{
    public int PromotionId { get; set; }

    public string PromotionName { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal? DiscountPercent { get; set; }

    public virtual ICollection<ServicePromotion> ServicePromotions { get; set; } = new List<ServicePromotion>();
}
