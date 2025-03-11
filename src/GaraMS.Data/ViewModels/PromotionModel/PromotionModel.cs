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
        public decimal DiscountPercent { get; set; }  // Changed to non-nullable
        public decimal DiscountAmount { get; set; }   // Added to store calculated discount
        public List<ServicePromotionDTO> ServicePromotions { get; set; } = new();
    }

    public class ServicePromotionDTO
    {
        public int ServicePromotionId { get; set; }
        public int ServiceId { get; set; }
        public int PromotionId { get; set; }
        public decimal OriginalPrice { get; set; }    // Added
        public decimal DiscountedPrice { get; set; }  // Added
    }

    public class CreatePromotionModel
    {
        public string PromotionName { get; set; }
        public DateTime StartDate { get; set; }       // Changed to non-nullable
        public DateTime EndDate { get; set; }         // Changed to non-nullable
        public decimal DiscountPercent { get; set; }  // Changed to non-nullable
    public List<int> ServiceIds { get; set; } = new List<int>();
    }

    public class UpdatePromotionModel
    {
        public string PromotionName { get; set; }
        public DateTime StartDate { get; set; }       // Changed to non-nullable
        public DateTime EndDate { get; set; }         // Changed to non-nullable
        public decimal DiscountPercent { get; set; }  // Changed to non-nullable
    }
}
