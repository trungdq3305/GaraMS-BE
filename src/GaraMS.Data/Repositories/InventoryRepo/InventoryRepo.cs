using GaraMS.Data.Models;
using GaraMS.Data.ViewModels;
using GaraMS.Data.ViewModels.InventoryModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.InventoryRepo
{
	public class InventoryRepo : IInventoryRepo
	{
		private readonly GaraManagementSystemContext _context;

		public InventoryRepo(GaraManagementSystemContext context) 
		{
			_context = context;	
		}

		public async Task<Inventory> AddInventoryUnitAsync(int inventoryId, int amount)
		{
			var inventory = await _context.Inventories.FindAsync(inventoryId);
			if (inventory == null) return null;

			var currentUnit = int.TryParse(inventory.Unit, out int current) ? current : 0;

			inventory.Unit = (currentUnit + amount).ToString();
			inventory.Status = true;
			inventory.UpdatedAt = DateTime.UtcNow;

			_context.Entry(inventory).State = EntityState.Modified;

			await _context.SaveChangesAsync();
			return inventory;
		}

		public async Task<Inventory> CreateInventoryAsync(InventoryModel model)
		{
			var inventory = new Inventory
			{
				Name = model.Name,
				Description = model.Description,
				Unit = model.Unit,
				Price = model.InventoryPrice,
				Status = int.TryParse(model.Unit, out int unit) && unit > 0,
				WarrantyPeriod = model.WarrantyPeriod,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};

			_context.Inventories.Add(inventory);
			await _context.SaveChangesAsync();
			return inventory;
		}

		public async Task<Inventory> DeleteInventoryAsync(int id)
		{
			var inventory = await _context.Inventories.FindAsync(id);
			if (inventory == null) return null;

			_context.Inventories.Remove(inventory);
			await _context.SaveChangesAsync();
			return inventory;
		}

		public async Task<List<InventoryModel>> GetAllInventoriesAsync()
		{
			return await _context.Inventories
				.Include(i => i.InventorySuppliers)
					.ThenInclude(isup => isup.Supplier)
				.Include(i => i.ServiceInventories)
					.ThenInclude(sinv => sinv.Service)
				.Select(i => new InventoryModel
				{
					InventoryId = i.InventoryId,
                    Name = i.Name,
					Description = i.Description,
					Unit = i.Unit,
					InventoryPrice = i.Price,
					Status = i.Status,
					WarrantyPeriod = i.WarrantyPeriod,
					InventorySuppliers = i.InventorySuppliers.Select(isup => new SupplierModel
					{
						SupplierId = isup.Supplier.SupplierId,
						Name = isup.Supplier.Name
					}).ToList(),
					ServiceInventories = i.ServiceInventories.Select(sinv => new ServiceModel
					{
						ServiceId = sinv.Service.ServiceId,
						ServiceName = sinv.Service.ServiceName
					}).ToList()
				})
				.ToListAsync();
		}


		public async Task<Inventory> GetInventoryByIdAsync(int id)
		{
			return await _context.Inventories.Include(i => i.InventorySuppliers).ThenInclude(isup => isup.Supplier)
											 .Include(i => i.ServiceInventories).ThenInclude(si => si.Service)
											 .FirstOrDefaultAsync(i => i.InventoryId == id);
		}

		public async Task<Inventory> UpdateInventoryAsync(int id, InventoryModel model)
		{
			var inventory = await _context.Inventories.FindAsync(id);
			if (inventory == null) return null;

			inventory.Name = model.Name;
			inventory.Description = model.Description;
			inventory.Unit = model.Unit;
			inventory.Price = model.InventoryPrice;
			inventory.Status = int.TryParse(model.Unit, out int unit) && unit > 0;
			inventory.WarrantyPeriod = model.WarrantyPeriod;
			inventory.UpdatedAt = DateTime.UtcNow;

			_context.Inventories.Update(inventory);
			await _context.SaveChangesAsync();

			var serviceIds = await _context.ServiceInventories
							.Where(si => si.InventoryId == id)
							.Select(si => si.ServiceId)
							.ToListAsync();

			foreach (var serviceId in serviceIds)
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
				}
			}

			await _context.SaveChangesAsync();
			return inventory;
		}

		public async Task<Inventory> UseInventoryAsync(int inventoryId)
		{
			var inventory = await _context.Inventories.FindAsync(inventoryId);
			if (inventory == null) return null;

			if (!int.TryParse(inventory.Unit, out int currentUnit) || currentUnit <= 0)
				return null;

			inventory.Unit = (currentUnit - 1).ToString();
			inventory.Status = currentUnit - 1 > 0;
			inventory.UpdatedAt = DateTime.UtcNow;

			_context.Entry(inventory).State = EntityState.Modified;

			await _context.SaveChangesAsync();
			return inventory;
		}
	}
}
