using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.PromotionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.PromotionRepo
{
    public interface IPromoRepo
    {
        Task<List<Promotion>> GetAllPromotionsAsync();
        Task<Promotion> GetPromotionByIdAsync(int id);
        Task<bool> CreatePromotionAsync(Promotion promotion);
        Task<bool> UpdatePromotionAsync(int id, Promotion promotion);
        Task<bool> DeletePromotionAsync(int id);
        Task<List<Promotion>> GetActivePromotionsAsync();
        Task<(decimal finalPrice, decimal discountAmount)> CalculateDiscountedPrice(int serviceId, decimal basePrice);
        Task<List<Promotion>> GetPromotionsForService(int serviceId);
        Task<bool> IsPromotionValidForService(int promotionId, int serviceId);
    }
}
