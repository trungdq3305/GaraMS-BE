using GaraMS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.ViewModels.PromotionModel
{
    public class PromotionModel
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? DiscountPercent { get; set; }

        public List<ServicePromotionDTO> ServicePromotions { get; set; } = new();
    }

    public class ServicePromotionDTO
    {
        public int ServicePromotionId { get; set; }
        public int ServiceId { get; set; }
        public int PromotionId { get; set; }
    }

    public class CreatePromotionModel
    {
        public string PromotionName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? DiscountPercent { get; set; }
        public List<int> ServiceIds { get; set; } = new(); // Just need the service IDs
    }

    public class UpdatePromotionModel
    {
        public string PromotionName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? DiscountPercent { get; set; }
    }
}
