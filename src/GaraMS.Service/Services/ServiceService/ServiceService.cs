using GaraMS.Data.Repositories.ServiceRepo;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Data.ViewModels;
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

        public async Task<ResultModel> UpdateServicePromotionAsync(string? token, int serviceId, decimal promotionAmount)
        {
            try
            {
                var service = await _serviceRepo.GetServiceByIdAsync(serviceId);
                if (service == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 404,
                        Message = $"Service with ID {serviceId} not found"
                    };
                }

                if (promotionAmount < 0 || promotionAmount > (service.ServicePrice + service.InventoryPrice ?? 0))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "Invalid promotion amount"
                    };
                }

                var updatedService = await _serviceRepo.UpdateServicePromotionAsync(serviceId, promotionAmount);
                if (updatedService == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "Failed to update service promotion"
                    };
                }

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = 200,
                    Message = "Service promotion updated successfully",
                    Data = new
                    {
                        ServiceId = updatedService.ServiceId,
                        ServiceName = updatedService.ServiceName,
                        OriginalPrice = updatedService.ServicePrice + updatedService.InventoryPrice,
                        Promotion = updatedService.Promotion,
                        FinalPrice = updatedService.TotalPrice
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = $"Error updating service promotion: {ex.Message}"
                };
            }
        }

		public async Task<ResultModel> ApplyPromotionToServiceAsync(string? token, int serviceId, decimal discountPercent)
		{
			try
			{
				var service = await _serviceRepo.GetServiceByIdAsync(serviceId);
				if (service == null)
				{
					return new ResultModel
					{
						IsSuccess = false,
						Code = 404,
						Message = $"Service with ID {serviceId} not found"
					};
				}
				
				var result = await _serviceRepo.UpdateServicePromotionDirectlyAsync(serviceId, discountPercent);
				if (!result)
				{
					return new ResultModel
					{
						IsSuccess = false,
						Code = 400,
						Message = "Failed to apply promotion to service"
					};
				}
				
				// Get the updated service
				var updatedService = await _serviceRepo.GetServiceByIdAsync(serviceId);
				
				return new ResultModel
				{
					IsSuccess = true,
					Code = 200,
					Message = "Promotion applied successfully",
					Data = updatedService
				};
			}
			catch (Exception ex)
			{
				return new ResultModel
				{
					IsSuccess = false,
					Code = 500,
					Message = $"Error applying promotion: {ex.Message}"
				};
			}
		}
    }
}
