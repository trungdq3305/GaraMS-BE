using GaraMS.Data.Models;
using GaraMS.Data.Repositories.AppointmentRepo;
using GaraMS.Data.Repositories.UserRepo;
using GaraMS.Data.ViewModels.AppointmentModel;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Service.Services.AccountService;
using GaraMS.Service.Services.TokenService;
using RTools_NTS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.AppointmentService
{
	public class AppointmentService : IAppointmentService
	{
		private	readonly IUserRepo _userRepo;
        private readonly ITokenService _token;
        private readonly IAppointmentRepo _appointmentRepo;
		private readonly IAccountService _accountService;
		private readonly ITokenService _tokenService;

		public AppointmentService(IUserRepo userRepo,ITokenService token,IAppointmentRepo appointmentRepo, IAccountService accountService, ITokenService tokenService)
		{
			_userRepo = userRepo;
            _token = token;
            _appointmentRepo = appointmentRepo;
			_accountService = accountService;
			_tokenService = tokenService;
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

		public async Task<ResultModel> CreateAppointmentAsync(string? token, AppointmentModel model)
		{
			var validationResult = await ValidateToken(token, new List<int> { 1 });
			if (!validationResult.IsSuccess)
				return validationResult;

			var appointment = await _appointmentRepo.CreateAppointmentAsync(model);
			if (appointment == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to create appointment" };

			return new ResultModel { IsSuccess = true, Code = 201, Data = appointment, Message = "Appointment created successfully" };
		}

		public async Task<ResultModel> DeleteAppointmentAsync(string? token, int id)
		{
			var validationResult = await ValidateToken(token, new List<int> { 3 });
			if (!validationResult.IsSuccess)
				return validationResult;

			var appointment = await _appointmentRepo.DeleteAppointmentAsync(id);
			if (appointment == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to delete appointment" };

			return new ResultModel { IsSuccess = true, Code = 200, Message = "Appointment deleted successfully" };
		}

		public async Task<ResultModel> GetAllAppointmentsAsync(string? token)
		{
			var validationResult = await ValidateToken(token, new List<int> { 1, 3 });
			if (!validationResult.IsSuccess)
				return validationResult;

			var appointments = await _appointmentRepo.GetAllAppointmentsAsync();
			return new ResultModel { IsSuccess = true, Code = 200, Data = appointments, Message = "Appointments retrieved successfully" };
		}

		public async Task<ResultModel> GetAppointmentByIdAsync(string? token, int id)
		{
			var validationResult = await ValidateToken(token, new List<int> { 1 , 3 });
			if (!validationResult.IsSuccess)
				return validationResult;

			var appointment = await _appointmentRepo.GetAppointmentByIdAsync(id);
			if (appointment == null)
				return new ResultModel { IsSuccess = false, Code = 404, Message = "Appointment not found" };

			return new ResultModel { IsSuccess = true, Code = 200, Data = appointment, Message = "Appointment retrieved successfully" };
		}

		public async Task<ResultModel> UpdateAppointmentAsync(string? token, int id, AppointmentModel model)
		{
			var validationResult = await ValidateToken(token, new List<int> { 1 , 3 });
			if (!validationResult.IsSuccess)
				return validationResult;

			var appointment = await _appointmentRepo.UpdateAppointmentAsync(id, model);
			if (appointment == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to update appointment" };

			return new ResultModel { IsSuccess = true, Code = 200, Data = appointment, Message = "Appointment updated successfully" };
		}

		public async Task<ResultModel> UpdateAppointmentStatusAsync(string? token, int id, string status, string reason)
		{
			var validationResult = await ValidateToken(token, new List<int> { 3 }); // Example: Only Managers can update status
			if (!validationResult.IsSuccess)
				return validationResult;

			var appointment = await _appointmentRepo.UpdateAppointmentStatusAsync(id, status, reason);
			if (appointment == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to update appointment status" };

			return new ResultModel { IsSuccess = true, Code = 200, Data = appointment, Message = "Appointment status updated successfully" };
		}

        public async Task<ResultModel> GetAppointmentByLogin(string? token, Appointment appointment)
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
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 1 });
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
            var customerId = await _userRepo.GetCustomerIdByUserIdAsync(userId);
            if (customerId == null || customerId <= 0)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.NotFound;
                resultModel.Message = "Customer not found.";
                return resultModel;
            }
            var appointmentList = await _appointmentRepo.GetAppointmentsByUserId(customerId);
            if (appointmentList == null)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.NotFound;
                resultModel.Message = "No appointment found.";
                return resultModel;
            }

            resultModel.Data = appointmentList;
            resultModel.Message = "Appointment retrieved successfully.";
            return resultModel;
        }
    }
}
