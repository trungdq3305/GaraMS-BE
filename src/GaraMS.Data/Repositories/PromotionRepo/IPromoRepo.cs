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
    }
}
