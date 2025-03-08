using GaraMS.Data.ViewModels.PromotionModel;
using GaraMS.Data.ViewModels.ResultModel;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.PromotionService
{
    public interface IPromotionService
    {
        Task<ResultModel> GetAllPromotionsAsync(string? token);
        Task<ResultModel> GetPromotionByIdAsync(string? token, int id);
        Task<ResultModel> CreatePromotionAsync(string? token, PromotionModel promotionModel);
        Task<ResultModel> UpdatePromotionAsync(string? token, int id, UpdatePromotionModel promotionModel);
        Task<ResultModel> DeletePromotionAsync(string? token, int id);
        Task<ResultModel> GetActivePromotionsAsync(string? token);
        Task<ResultModel> CalculateServiceDiscountAsync(string? token, int serviceId, decimal originalPrice);
    }
} 