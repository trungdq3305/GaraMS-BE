using GaraMS.Data.Models;
using GaraMS.Data.Repositories.VehicleRepo;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Data.ViewModels.VehicleModel;
using GaraMS.Service.Services.AccountService;
using GaraMS.Service.Services.AutheticateService;
using GaraMS.Service.Services.TokenService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.VehicleService
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepo _vehicleRepo;
        private readonly IAccountService _accountService;
        private readonly IAuthenticateService _authentocateService;
        private readonly ITokenService _token;
        public VehicleService(IVehicleRepo vehicleRepo, IAuthenticateService authenticateService, IAccountService accountService, ITokenService tokenService)
        {
            _vehicleRepo = vehicleRepo;
            _token = tokenService;
            _accountService = accountService;
            _authentocateService = authenticateService;
        }

        public async Task<ResultModel> CreateVehicle(string? token, Vehicle vehicle)
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
                resultModel.Message = "You don't have permission to perform this action.";

                return resultModel;
            }

            var createdVehicle = await _vehicleRepo.createVehicle(vehicle);
            if (createdVehicle == null)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.InternalServerError;
                resultModel.Message = "Vehicle creation failed.";
                return resultModel;
            }

            resultModel.Data = createdVehicle;
            resultModel.Message = "Vehicle created successfully.";
            return resultModel;
        }

        public async Task<ResultModel> EditVehicle(string? token, Vehicle vehicle)
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
                resultModel.Message = "You don't have permission to perform this action.";

                return resultModel;
            }

            var updatedVehicle = await _vehicleRepo.updateVehicle(new EditVehicle
            {
                PlateNumber = vehicle.PlateNumber,
                Brand = vehicle.Brand,
                Model = vehicle.Model
            });

            if (updatedVehicle == null)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.InternalServerError;
                resultModel.Message = "Vehicle update failed.";
                return resultModel;
            }

            resultModel.Data = updatedVehicle;
            resultModel.Message = "Vehicle updated successfully.";
            return resultModel;
        }

        public async Task<ResultModel> ViewListVehicle(string? token, VehicleSearch vehicleSearch)
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
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 1,3 });
            if (!isValidRole)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.Forbidden;
                resultModel.Message = "You don't have permission to perform this action.";

                return resultModel;
            }
            var vehicleList = await _vehicleRepo.searchVehicle(vehicleSearch);
            if (vehicleList == null)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.NotFound;
                resultModel.Message = "No vehicles found.";
                return resultModel;
            }

            resultModel.Data = vehicleList;
            resultModel.Message = "Vehicles retrieved successfully.";
            return resultModel;
        }

        public async Task<ResultModel> ViewListVehicleByLogin(string? token, Vehicle vehicle)
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
            var vehicleList = await _vehicleRepo.GetVehicleByUserId(userId);
            if (vehicleList == null)
            {
                resultModel.IsSuccess = false;
                resultModel.Code = (int)HttpStatusCode.NotFound;
                resultModel.Message = "No vehicles found.";
                return resultModel;
            }

            resultModel.Data = vehicleList;
            resultModel.Message = "Vehicles retrieved successfully.";
            return resultModel;
        }
    }
}
