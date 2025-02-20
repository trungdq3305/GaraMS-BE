using GaraMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.ServiceRepo
{
	public class ServiceRepo : IServiceRepo
	{
		private readonly GaraManagementSystemContext _context;

		public ServiceRepo(GaraManagementSystemContext context)
		{
			_context = context;
		}
		public async Task<int> CreateAsync(Service service)
		{
			_context.Services.Add(service);
			return await _context.SaveChangesAsync();
		}

		public async Task<List<Service>> GetAllAsync()
		{
			return await _context.Services.ToListAsync();
		}

		public async Task<Service> GetByIdAsync(int id)
		{
			return await _context.Services.FindAsync(id);
		}

		public async Task<bool> RemoveAsync(int id)
		{
			var service = await _context.Services.FindAsync(id);
			if (service == null) return false;

			_context.Services.Remove(service);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<int> UpdateAsync(Service service)
		{
			_context.Services.Update(service);
			return await _context.SaveChangesAsync();
		}
	}
}
