using GaraMS.Data.Repositories.ServiceRepo;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Data.ViewModels.ServiceDTO;
using GaraMS.Service.Services.TokenService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.ServiceService
{
	public class ServiceService : IServiceService
	{
		private readonly IServiceRepo _serviceRepo;
		private readonly ITokenService _tokenService;

		public ServiceService(IServiceRepo serviceRepo, ITokenService tokenService)
		{
			_serviceRepo = serviceRepo;
			_tokenService = tokenService;
		}

		public async Task<ResultModel> CreateServiceAsync(string token, ServiceDTO serviceDto)
		{
			var user = _tokenService.decode(token);
			if (user.role != "Admin")
				return new ResultModel { IsSuccess = false, Code = 403, Message = "Unauthorized" };

			var service = new GaraMS.Data.Models.Service
			{
				ServiceName = serviceDto.ServiceName,
				TotalPrice = serviceDto.Price,
				Description = serviceDto.Description
			};

			bool success = await _serviceRepo.CreateAsync(service) > 0;
			return new ResultModel { IsSuccess = success, Code = success ? 201 : 500, Message = success ? "Service created successfully" : "Failed to create service" };
		}

		public async Task<ResultModel> DeleteServiceAsync(string token, int id)
		{
			var user = _tokenService.decode(token);
			if (user.role != "Admin")
				return new ResultModel { IsSuccess = false, Code = 403, Message = "Unauthorized" };

			int rm = await _serviceRepo.RemoveAsync(id);
			bool success = rm > 0;

			return new ResultModel
			{
				IsSuccess = success,
				Code = success ? 200 : 500,
				Message = success ? "Service deleted successfully" : "Failed to delete service"
			};
		}

		public async Task<ResultModel> GetAllServicesAsync()
		{
			var services = await _serviceRepo.GetAllAsync();
			return new ResultModel
			{
				IsSuccess = true,
				Code = 200,
				Data = services.Select(s => new ServiceDTO
				{
					ServiceName = s.ServiceName,
					Price = (decimal)s.TotalPrice,
					Description = s.Description
				}).ToList()
			};
		}

		public async Task<ResultModel> GetServiceByIdAsync(int id)
		{
			var service = await _serviceRepo.GetByIdAsync(id);
			if (service == null)
				return new ResultModel { IsSuccess = false, Code = 404, Message = "Service not found" };

			return new ResultModel
			{
				IsSuccess = true,
				Code = 200,
				Data = new ServiceDTO
				{
					ServiceName = service.ServiceName,
					Price = service.TotalPrice,
					Description = service.Description
				}
			};
		}

		public async Task<ResultModel> UpdateServiceAsync(string token, int id, ServiceDTO serviceDto)
		{
			var user = _tokenService.decode(token);
			if (user.role != "Admin")
				return new ResultModel { IsSuccess = false, Code = 403, Message = "Unauthorized" };

			var service = await _serviceRepo.GetByIdAsync(id);
			if (service == null)
				return new ResultModel { IsSuccess = false, Code = 404, Message = "Service not found" };

			service.ServiceName = serviceDto.ServiceName;
			service.TotalPrice = serviceDto.Price;
			service.Description = serviceDto.Description;
			service.UpdatedAt = DateTime.UtcNow;

			bool success = await _serviceRepo.UpdateAsync(service) > 0;
			return new ResultModel { IsSuccess = success, Code = success ? 200 : 500, Message = success ? "Service updated successfully" : "Failed to update service" };
		}
	}
}
