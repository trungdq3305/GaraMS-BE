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

		public async Task<bool> AssignInventoryToServiceAsync(int inventoryId, int serviceId)
		{
			var serviceInventory = await _context.ServiceInventories
				.FirstOrDefaultAsync(si => si.InventoryId == inventoryId && si.ServiceId == serviceId);

			if (serviceInventory != null)
				return true;

			var newServiceInventory = new ServiceInventory
			{
				InventoryId = inventoryId,
				ServiceId = serviceId
			};

			_context.ServiceInventories.Add(newServiceInventory);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<Service> CreateServiceAsync(ServiceModel model)
		{
			var service = new Service
			{
				ServiceName = model.ServiceName,
				ServicePrice = model.ServicePrice,
				InventoryPrice = model.InventoryPrice,
				TotalPrice = model.ServicePrice + model.InventoryPrice,
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
			return await _context.Services.Include(c => c.ServiceEmployees).ThenInclude(b => b.Employee).ThenInclude(b => b.User)
				.Include(c => c.ServiceInventories).ThenInclude(b => b.Inventory)
                .Include(c => c.ServicePromotions).ThenInclude(b => b.Promotion).
				ToListAsync();
		}

		public async Task<List<ServiceInventory>> GetAllServiceInventoriesAsync()
		{
			return await _context.ServiceInventories.ToListAsync();
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
			service.ServicePrice = model.ServicePrice;
			service.InventoryPrice = model.InventoryPrice;
			service.TotalPrice = model.ServicePrice + model.InventoryPrice;
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

                service.TotalPrice = service.ServicePrice + service.InventoryPrice;
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

		public async Task<Service> UpdateServicePromotionAsync(int serviceId, decimal promotionAmount)
		{
				var service = await _context.Services.FindAsync(serviceId);
				if (service == null)
					return null;

				service.Promotion = promotionAmount;
				service.TotalPrice = (service.ServicePrice ?? 0) + (service.InventoryPrice ?? 0) - promotionAmount;
				service.UpdatedAt = DateTime.Now;

				await _context.SaveChangesAsync();
				return service;  // Return the updated service object
		}

        public async Task<bool> UpdateServicePromotionDirectlyAsync(int serviceId, decimal discountPercent)
        {
            try
            {
                var service = await _context.Services.FindAsync(serviceId);
                if (service == null)
                    return false;

                decimal originalPrice = (service.ServicePrice ?? 0) + (service.InventoryPrice ?? 0);
                decimal discountAmount = (originalPrice * discountPercent) / 100;

                service.Promotion = discountAmount;
                service.TotalPrice = originalPrice - discountAmount;
                service.UpdatedAt = DateTime.Now;

                _context.Entry(service).State = EntityState.Modified;
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
