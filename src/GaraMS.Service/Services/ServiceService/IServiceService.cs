using GaraMS.Data.ViewModels.ServiceDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.ServiceService
{
	public interface IServiceService
	{
		Task<List<ServiceDTO>> GetAllServicesAsync();
		Task<ServiceDTO> GetServiceByIdAsync(int id);
		Task<bool> CreateServiceAsync(ServiceDTO serviceDto);
		Task<bool> UpdateServiceAsync(int id, ServiceDTO serviceDto);
		Task<bool> DeleteServiceAsync(int id);
	}
}
