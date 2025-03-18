using GaraMS.Data.Repositories.UserRepo;
using GaraMS.Data.Repositories.WarrantyHistoryRepo;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Data.ViewModels.WarrantyHistoryModel;
using GaraMS.Service.Services.AccountService;
using GaraMS.Service.Services.TokenService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.WarrantyHistoryService
{
	public class WarrantyHistoryService : IWarrantyHistoryService
	{
		private readonly IWarrantyHistoryRepo _warrantyHistoryRepo;
		private readonly IUserRepo _userRepo;
		private readonly ITokenService _tokenService;
		private readonly IAccountService _accountService;

		public WarrantyHistoryService(IUserRepo userRepo, ITokenService tokenService, IWarrantyHistoryRepo warrantyHistoryRepo, IAccountService accountService) 
		{
			_warrantyHistoryRepo = warrantyHistoryRepo;
			_userRepo = userRepo;
			_tokenService = tokenService;
			_accountService = accountService;
		}

		private ResultModel UnauthorizedResult => new()
		{
			IsSuccess = false,
			Code = (int)HttpStatusCode.Unauthorized,
			Message = "Invalid token."
		};

		private async Task<ResultModel> ValidateToken(string? token, List<int> allowedRoles)
		{
			var decodeModel = _tokenService.decode(token);
			var isValidRole = _accountService.IsValidRole(decodeModel.role, allowedRoles);

			if (!isValidRole)
			{
				return new ResultModel
				{
					IsSuccess = false,
					Code = (int)HttpStatusCode.Forbidden,
					Message = "You don't have permission to perform this action."
				};
			}

			if (!int.TryParse(decodeModel.userid, out int userId) || userId <= 0)
			{
				return UnauthorizedResult;
			}

			return new ResultModel { IsSuccess = true, Data = userId };
		}

		public async Task<ResultModel> CreateWarrantyHistoryAsync(string? token, WarrantyHistoryModel model)
		{
			var validationResult = await ValidateToken(token, new List<int> { 3 });
			if (!validationResult.IsSuccess)
				return validationResult;

			var warrantyHistory = await _warrantyHistoryRepo.CreateWarrantyHistoryAsync(model);
			if (warrantyHistory == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to create warranty history" };

			return new ResultModel { IsSuccess = true, Code = 201, Data = warrantyHistory, Message = "Warranty history created successfully" };
		}

		public async Task<ResultModel> DeleteWarrantyHistoryAsync(string? token, int id)
		{
			var validationResult = await ValidateToken(token, new List<int> { 3 });
			if (!validationResult.IsSuccess)
				return validationResult;

			var warrantyHistory = await _warrantyHistoryRepo.DeleteWarrantyHistoryAsync(id);
			if (warrantyHistory == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to delete warranty history" };

			return new ResultModel { IsSuccess = true, Code = 200, Message = "Warranty history deleted successfully" };
		}

		public async Task<ResultModel> GetAllWarrantyHistoriesAsync(string? token)
		{
			var validationResult = await ValidateToken(token, new List<int> { 1, 2, 3 });
			if (!validationResult.IsSuccess)
				return validationResult;

			var warrantyHistories = await _warrantyHistoryRepo.GetAllWarrantyHistoriesAsync();
			return new ResultModel { IsSuccess = true, Code = 200, Data = warrantyHistories, Message = "Warranty histories retrieved successfully" };
		}

		public async Task<ResultModel> GetWarrantyHistoryByIdAsync(string? token, int id)
		{
			var validationResult = await ValidateToken(token, new List<int> { 1, 2, 3 });
			if (!validationResult.IsSuccess)
				return validationResult;

			var warrantyHistory = await _warrantyHistoryRepo.GetWarrantyHistoryByIdAsync(id);
			if (warrantyHistory == null)
				return new ResultModel { IsSuccess = false, Code = 404, Message = "Warranty history not found" };

			return new ResultModel { IsSuccess = true, Code = 200, Data = warrantyHistory, Message = "Warranty history retrieved successfully" };
		}

		public async Task<ResultModel> UpdateWarrantyHistoryAsync(string? token, int id, WarrantyHistoryModel model)
		{
			var validationResult = await ValidateToken(token, new List<int> { 3 });
			if (!validationResult.IsSuccess)
				return validationResult;

			var warrantyHistory = await _warrantyHistoryRepo.UpdateWarrantyHistoryAsync(id, model);
			if (warrantyHistory == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to update warranty history" };

			return new ResultModel { IsSuccess = true, Code = 200, Data = warrantyHistory, Message = "Warranty history updated successfully" };
		}
	}
}
