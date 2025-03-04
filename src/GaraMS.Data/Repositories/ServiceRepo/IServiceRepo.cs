using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.ServiceRepo
{
	public interface IServiceRepo
	{
		Task<List<Service>> GetAllAsync();
		Task<Service> GetServiceByIdAsync(int id);
		Task<Service> CreateServiceAsync(ServiceModel model);
		Task<Service> UpdateServiceAsync(int id, ServiceModel model);
		Task<Service> RemoveServiceAsync(int id);
	}
}
