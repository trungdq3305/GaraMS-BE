using GaraMS.Data.Models;
using GaraMS.Data.ViewModels;
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

		public async Task<Service> CreateServiceAsync(ServiceModel model)
		{
			var service = new Service
			{
				ServiceName = model.ServiceName,
				TotalPrice = model.Price,
				Description = model.Description,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};

			_context.Services.Add(service);
			await _context.SaveChangesAsync();
			return service;
		}

		public async Task<List<Service>> GetAllAsync()
		{
			return await _context.Services.ToListAsync();
		}

		public async Task<Service> GetServiceByIdAsync(int id)
		{
			return await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == id);
		}

		public async Task<Service> RemoveServiceAsync(int id)
		{
			var service = await _context.Services.FindAsync(id);
			if (service == null) return null;

			_context.Services.Remove(service);
			await _context.SaveChangesAsync();
			return service;
		}

		public async Task<Service> UpdateServiceAsync(int id, ServiceModel model)
		{
			var service = await _context.Services.FindAsync(id);

			service.ServiceName = model.ServiceName;
			service.TotalPrice = model.Price;
			service.Description = model.Description;
			service.UpdatedAt = DateTime.UtcNow;

			_context.Services.Update(service);
			await _context.SaveChangesAsync();
			return service;
		}

        public async Task<bool> UpdateServicePriceAsync(int serviceId, decimal? totalPrice, decimal? promotion)
        {
            try
            {
                var service = await _context.Services.FindAsync(serviceId);
                if (service == null) return false;

                service.TotalPrice = totalPrice;
                service.Promotion = promotion;
                service.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateServicePromotionAsync(int serviceId, decimal? promotionAmount)
        {
            try
            {
                var service = await _context.Services.FindAsync(serviceId);
                if (service == null) return false;

                // Only update the promotion amount, keep totalPrice as base price
                service.Promotion = promotionAmount;
                service.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
