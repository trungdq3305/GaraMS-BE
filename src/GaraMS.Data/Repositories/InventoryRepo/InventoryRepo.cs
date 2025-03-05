using GaraMS.Data.Models;
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
		public async Task<Inventory> CreateInventoryAsync(InventoryModel model)
		{
			var inventory = new Inventory
			{
				Name = model.Name,
				Description = model.Description,
				Unit = model.Unit,
				Price = model.Price,
				Status = model.Status,
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

		public async Task<List<Inventory>> GetAllInventoriesAsync()
		{
			return await _context.Inventories.Include(i => i.InventorySuppliers)
											 .Include(i => i.ServiceInventories)
											 .ToListAsync();
		}

		public async Task<Inventory> GetInventoryByIdAsync(int id)
		{
			return await _context.Inventories.Include(i => i.InventorySuppliers)
											 .Include(i => i.ServiceInventories)
											 .FirstOrDefaultAsync(i => i.InventoryId == id);
		}

		public async Task<Inventory> UpdateInventoryAsync(int id, InventoryModel model)
		{
			var inventory = await _context.Inventories.FindAsync(id);
			if (inventory == null) return null;

			inventory.Name = model.Name;
			inventory.Description = model.Description;
			inventory.Unit = model.Unit;
			inventory.Price = model.Price;
			inventory.Status = model.Status;
			inventory.UpdatedAt = DateTime.UtcNow;

			_context.Inventories.Update(inventory);
			await _context.SaveChangesAsync();
			return inventory;
		}
	}
}
