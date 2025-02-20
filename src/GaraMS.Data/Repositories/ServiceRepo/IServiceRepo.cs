using GaraMS.Data.Models;
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
		Task<Service> GetByIdAsync(int id);
		Task<int> CreateAsync(Service service);
		Task<int> UpdateAsync(Service service);
		Task<bool> RemoveAsync(int id);
	}
}
