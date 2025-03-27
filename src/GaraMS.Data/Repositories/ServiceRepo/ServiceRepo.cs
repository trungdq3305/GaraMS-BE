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
			var service = await _context.Services.Include(s => s.ServicePromotions).ThenInclude(s => s.Promotion).FirstOrDefaultAsync(s => s.ServiceId == serviceId);
			var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.InventoryId == inventoryId);
			service.InventoryPrice += inventory.Price;
			var servicePromorion = await _context.ServicePromotions.Include(si => si.Promotion)
				.FirstOrDefaultAsync(si => si.ServiceId == serviceId);
			service.Promotion = (service.ServicePrice + service.InventoryPrice) * (servicePromorion.Promotion.DiscountPercent / 100);

			service.TotalPrice = (service.ServicePrice + service.InventoryPrice) - service.Promotion;
			_context.Services.Update(service);
			await _context.SaveChangesAsync();
			await UpdateInventoryPriceAsync(serviceId);
			return true;
		}

		public async Task<Service> CreateServiceAsync(ServiceModel model)
		{
			var service = new Service
			{
				ServiceName = model.ServiceName,
				ServicePrice = model.ServicePrice,
				InventoryPrice = 0,
				TotalPrice = model.ServicePrice ?? 0,
				Description = model.Description,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow,
				WarrantyPeriod = model.WarrantyPeriod
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
			return await _context.Services
		.Include(s => s.ServiceInventories)
			.ThenInclude(si => si.Inventory)
		.FirstOrDefaultAsync(s => s.ServiceId == id);
		}

		public async Task<bool> RemoveInventoryFromServiceAsync(int inventoryId, int serviceId)
		{
			var serviceInventory = await _context.ServiceInventories
								.FirstOrDefaultAsync(si => si.InventoryId == inventoryId && si.ServiceId == serviceId);

			if (serviceInventory == null)
				return false;

			_context.ServiceInventories.Remove(serviceInventory);
			await _context.SaveChangesAsync();

			// Cập nhật InventoryPrice sau khi xóa Inventory
			await UpdateInventoryPriceAsync(serviceId);

			return true;
		}

		public async Task<Service> RemoveServiceAsync(int id)
		{
			var service = await _context.Services.FindAsync(id);
			if (service == null) return null;

			_context.Services.Remove(service);
			await _context.SaveChangesAsync();
			return service;
		}

		public async Task UpdateInventoryPriceAsync(int serviceId)
		{
			var service = await _context.Services
						.Include(s => s.ServiceInventories)
						.ThenInclude(si => si.Inventory)
						.FirstOrDefaultAsync(s => s.ServiceId == serviceId);

			if (service != null)
			{
				decimal totalInventoryPrice = service.ServiceInventories
					.Sum(si => si.Inventory.Price ?? 0);

				service.InventoryPrice = totalInventoryPrice;
				service.TotalPrice = (service.ServicePrice ?? 0) + totalInventoryPrice;
				await _context.SaveChangesAsync();
			}
		}

		public async Task<Service> UpdateServiceAsync(int id, ServiceModel model)
		{
			var service = await _context.Services.FindAsync(id);
			if (service == null) return null;
			var percent = service.Promotion / service.TotalPrice;

			service.ServiceName = model.ServiceName;
			service.ServicePrice = model.ServicePrice;
			service.Description = model.Description;
			service.UpdatedAt = DateTime.UtcNow;

			service.TotalPrice = ((service.ServicePrice ?? 0) + (service.InventoryPrice ?? 0))*(1-percent) ;

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
