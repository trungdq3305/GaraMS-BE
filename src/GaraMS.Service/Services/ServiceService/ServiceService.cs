using GaraMS.Data.Repositories.ServiceRepo;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Data.ViewModels.ServiceModel;
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

		public ServiceService(IServiceRepo serviceRepo)
		{
			_serviceRepo = serviceRepo;
		}

		public async Task<ResultModel> CreateServiceAsync(string token, ServiceModel model)
		{
			var service = await _serviceRepo.CreateServiceAsync(model);
			if (service == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to create service" };

			return new ResultModel { IsSuccess = true, Code = 201, Data = service, Message = "Service created successfully" };
		}

		public async Task<ResultModel> DeleteServiceAsync(string token, int id)
		{
			var service = await _serviceRepo.RemoveServiceAsync(id);
			if (service == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to delete service" };

			return new ResultModel { IsSuccess = true, Code = 200, Message = "Service deleted successfully" };
		}

		public async Task<ResultModel> GetAllServicesAsync()
		{
			var services = await _serviceRepo.GetAllAsync();
			return new ResultModel { IsSuccess = true, Code = 200, Data = services, Message = "Services retrieved successfully" };
		}

		public async Task<ResultModel> GetServiceByIdAsync(int id)
		{
			var service = await _serviceRepo.GetServiceByIdAsync(id);
			if (service == null)
				return new ResultModel { IsSuccess = false, Code = 404, Message = "Service not found" };

			return new ResultModel { IsSuccess = true, Code = 200, Data = service, Message = "Service retrieved successfully" };
		}

		public async Task<ResultModel> UpdateServiceAsync(string token, int id, ServiceModel model)
		{
			var service = await _serviceRepo.UpdateServiceAsync(id, model);
			if (service == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to update service" };

			return new ResultModel { IsSuccess = true, Code = 200, Data = service, Message = "Service updated successfully" };
		}
	}
}
