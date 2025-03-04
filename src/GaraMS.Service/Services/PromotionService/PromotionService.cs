using GaraMS.Data.Models;
using GaraMS.Data.Repositories.PromotionRepo;
using GaraMS.Data.Repository;
using GaraMS.Data.ViewModels.PromotionModel;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Service.Services.AccountService;
using GaraMS.Service.Services.AutheticateService;
using GaraMS.Service.Services.TokenService;
using Microsoft.VisualBasic;
using RTools_NTS.Util;
using System;
using System.Net;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.PromotionService
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromoRepo _promoRepo;
        private readonly IAccountService _accountService;
        private readonly IAuthenticateService _authentocateService;
        private readonly ITokenService _token;

        public PromotionService(IPromoRepo promoRepo
            , IAuthenticateService authenticateService
            , IAccountService accountService
            , ITokenService tokenService)
        {
            _promoRepo = promoRepo;
            _token = tokenService;
            _accountService = accountService;
            _authentocateService = authenticateService;
        }

        public async Task<ResultModel> GetAllPromotionsAsync(string? token)
        {
            var resultModel = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Data = null,
                Message = null,
            };
            var res = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.Unauthorized,
                Message = "Invalid token."
            };

            var decodeModel = _token.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 4 });
            if (!isValidRole)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.Forbidden;
                resultModel.Message = "You don't have permission to perform this action.";
                return resultModel;
            }
            if (!int.TryParse(decodeModel.userid, out int userId))
            {
                return res;
            }
            if (userId <= 0)
            {
                return res;
            }

            var promotions = await _promoRepo.GetAllPromotionsAsync();
            if (promotions == null)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.NotFound;
                resultModel.Message = "No promotions found.";
                return resultModel;
            }

            resultModel.Data = promotions;
            resultModel.Message = "Promotions retrieved successfully";
            return resultModel;

        }

        public async Task<ResultModel> GetPromotionByIdAsync(string? token, int id)
        {
            var resultModel = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Data = null,
                Message = null,
            };
            var res = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.Unauthorized,
                Message = "Invalid token."
            };

            var decodeModel = _token.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 4 });
            if (!isValidRole)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.Forbidden;
                resultModel.Message = "You don't permission to perform this action.";

                return resultModel;
            }
            if (!int.TryParse(decodeModel.userid, out int userId))
            {
                return res;
            }
            if (userId <= 0)
            {
                return res;
            }

            var promotion = await _promoRepo.GetPromotionByIdAsync(id);
            if (promotion == null)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.NotFound;
                resultModel.Message = "Promotion not found.";
                return resultModel;
            }

            resultModel.Data = promotion;
            resultModel.Message = "Promotion retrieved successfully";
            return resultModel;
        }

        public async Task<ResultModel> CreatePromotionAsync(string? token, PromotionModel promotionModel)
        {
            var resultModel = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Data = null,
                Message = null,
            };
            var res = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.Unauthorized,
                Message = "Invalid token."
            };

            var decodeModel = _token.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 4 });
            if (!isValidRole)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.Forbidden;
                resultModel.Message = "You don't permission to perform this action.";

                return resultModel;
            }
            if (!int.TryParse(decodeModel.userid, out int userId))
            {
                return res;
            }
            if (userId <= 0)
            {
                return res;
            }

            var promotion = new Promotion
            {
                PromotionName = promotionModel.PromotionName,
                StartDate = promotionModel.StartDate,
                EndDate = promotionModel.EndDate,
                DiscountPercent = promotionModel.DiscountPercent,
                ServicePromotions = promotionModel.ServicePromotions.Select(sp => new ServicePromotion
                {
                    ServiceId = sp.ServiceId,
                }).ToList()
            };

            var createResult = await _promoRepo.CreatePromotionAsync(promotion);
            if (!createResult)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.InternalServerError;
                resultModel.Message = "Failed to create promotion.";
                return resultModel;
            }

            resultModel.Data = promotion;
            resultModel.Message = "Promotion created successfully";
            return resultModel;
        }

        public async Task<ResultModel> UpdatePromotionAsync(string? token, int id, UpdatePromotionModel promotionModel)
        {
            var resultModel = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Data = null,
                Message = null,
            };
            var res = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.Unauthorized,
                Message = "Invalid token."
            };

            var decodeModel = _token.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 4 });
            if (!isValidRole)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.Forbidden;
                resultModel.Message = "You don't permission to perform this action.";

                return resultModel;
            }
            if (!int.TryParse(decodeModel.userid, out int userId))
            {
                return res;
            }
            if (userId <= 0)
            {
                return res;
            }

            try
            {
                // Get existing promotion
                var existingPromotion = await _promoRepo.GetPromotionByIdAsync(id);
                if (existingPromotion == null)
                {
                    resultModel.IsSuccess = false;
                    resultModel.Code = (int)HttpStatusCode.NotFound;
                    resultModel.Message = "Promotion not found";
                    return resultModel;
                }

                // Update only the specific fields
                existingPromotion.PromotionName = promotionModel.PromotionName;
                existingPromotion.StartDate = promotionModel.StartDate;
                existingPromotion.EndDate = promotionModel.EndDate;
                existingPromotion.DiscountPercent = promotionModel.DiscountPercent;

                var result = await _promoRepo.UpdatePromotionAsync(id, existingPromotion);
                if (!result)
                {
                    resultModel.IsSuccess = false;
                    resultModel.Code = (int)HttpStatusCode.BadRequest;
                    resultModel.Message = "Failed to update promotion";
                    return resultModel;
                }

                resultModel.Message = "Promotion updated successfully";
                return resultModel;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.InternalServerError;
                resultModel.Message = $"Error updating promotion: {ex.Message}";
                return resultModel;
            }
        }

        public async Task<ResultModel> DeletePromotionAsync(string? token, int id)
        {
            var resultModel = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Data = null,
                Message = null,
            };
            var res = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.Unauthorized,
                Message = "Invalid token."
            };

            var decodeModel = _token.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 4 });
            if (!isValidRole)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.Forbidden;
                resultModel.Message = "You don't have permission to perform this action.";
                return resultModel;
            }
            if (!int.TryParse(decodeModel.userid, out int userId))
            {
                return res;
            }
            if (userId <= 0)
            {
                return res;
            }
            try
            {
                // Check if promotion exists
                var existingPromotion = await _promoRepo.GetPromotionByIdAsync(id);
                if (existingPromotion == null)
                {
                    resultModel.IsSuccess = false;
                    resultModel.Code = (int)HttpStatusCode.NotFound;
                    resultModel.Message = "Promotion not found";
                    return resultModel;
                }

                // Clear related ServicePromotion records first
                existingPromotion.ServicePromotions?.Clear();

                var result = await _promoRepo.DeletePromotionAsync(id);
                if (!result)
                {
                    resultModel.IsSuccess = false;
                    resultModel.Code = (int)HttpStatusCode.InternalServerError;
                    resultModel.Message = "Failed to delete promotion";
                    return resultModel;
                }

                resultModel.Message = "Promotion deleted successfully";
                return resultModel;
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.InternalServerError;
                resultModel.Message = $"Error deleting promotion: {ex.Message}";
                return resultModel;
            }
        }

        public async Task<ResultModel> GetActivePromotionsAsync(string? token)
        {
            try
            {
                var promotions = await _promoRepo.GetActivePromotionsAsync();
                return new ResultModel
                {
                    IsSuccess = true,
                    Code = 200,
                    Data = promotions,
                    Message = "Active promotions retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = $"Error retrieving active promotions: {ex.Message}"
                };
            }
        }
    }
} 