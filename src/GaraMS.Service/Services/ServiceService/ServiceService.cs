using GaraMS.Data.Repositories.ServiceRepo;
using GaraMS.Data.ViewModels.ServiceDTO;
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

		public async Task<bool> CreateServiceAsync(ServiceDTO serviceDto)
		{
			var service = new GaraMS.Data.Models.Service
			{
				ServiceName = serviceDto.ServiceName,
				TotalPrice = serviceDto.Price,
				Description = serviceDto.Description
			};

			return await _serviceRepo.CreateAsync(service) > 0;
		}

		public async Task<bool> DeleteServiceAsync(int id)
		{
			return await _serviceRepo.RemoveAsync(id);
		}

		public async Task<List<ServiceDTO>> GetAllServicesAsync()
		{
			var services = await _serviceRepo.GetAllAsync();
			return services.Select(s => new ServiceDTO
			{
				ServiceName = s.ServiceName,
				Price = (decimal)s.TotalPrice, // Change from s.Price to s.TotalPrice
				Description = s.Description
			}).ToList();
		}

		public async Task<ServiceDTO> GetServiceByIdAsync(int id)
		{
			var service = await _serviceRepo.GetByIdAsync(id);
			if (service == null) return null;

			return new ServiceDTO
			{
				ServiceName = service.ServiceName,
				Price = service.TotalPrice,
				Description = service.Description
			};
		}

		public async Task<bool> UpdateServiceAsync(int id, ServiceDTO serviceDto)
		{
			var service = await _serviceRepo.GetByIdAsync(id);
			if (service == null) return false;

			service.ServiceName = serviceDto.ServiceName;
			service.TotalPrice = serviceDto.Price;
			service.Description = serviceDto.Description;
			service.UpdatedAt = DateTime.UtcNow;

			return await _serviceRepo.UpdateAsync(service) > 0;
		}
	}
}
