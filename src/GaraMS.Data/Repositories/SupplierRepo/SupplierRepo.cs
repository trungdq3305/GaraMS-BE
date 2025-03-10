using GaraMS.Data.Models;
using GaraMS.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.SupplierRepo
{
	public class SupplierRepo : ISupplierRepo
	{
		private readonly GaraManagementSystemContext _context;

		public SupplierRepo(GaraManagementSystemContext context) 
		{
			_context = context;
		}

		public async Task<bool> AssignInventoryToSupplierAsync(int inventoryId, int supplierId)
		{
			var inventorySupplier = await _context.InventorySuppliers
				.FirstOrDefaultAsync(isup => isup.InventoryId == inventoryId && isup.SupplierId == supplierId);

			if (inventorySupplier != null)
				return true;

			var newInventorySupplier = new InventorySupplier
			{
				InventoryId = inventoryId,
				SupplierId = supplierId
			};

			_context.InventorySuppliers.Add(newInventorySupplier);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<Supplier> CreateSupplierAsync(SupplierModel supplierModel)
		{
			var supplier = new Supplier
			{
				Name = supplierModel.Name,
				Phone = supplierModel.Phone,
				Email = supplierModel.Email,
			};

			_context.Suppliers.Add(supplier);
			await _context.SaveChangesAsync();
			return supplier;
		}

		public async Task<Supplier> DeleteSupplierAsync(int id)
		{
			var supplier = await _context.Suppliers.FindAsync(id);
			if (supplier == null) return null;
			
			_context.Suppliers.Remove(supplier);
			await _context.SaveChangesAsync();
			return supplier;
		}

		public async Task<List<InventorySupplier>> GetAllInventorySuppliersAsync()
		{
			return await _context.InventorySuppliers.ToListAsync();
		}

		public async Task<List<Supplier>> GetAllSupplierAsync()
		{
			return await _context.Suppliers.ToListAsync();
		}

		public async Task<Supplier> GetSupplierByIdAsync(int id)
		{
			return await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id);
		}

		public async Task<Supplier> UpdateSupplierAsync(int id, SupplierModel supplierModel)
		{
			var supplier = await _context.Suppliers.FindAsync(id);
			if (supplier == null) return null;

			supplier.Name = supplierModel.Name;
			supplier.Phone = supplierModel.Phone;
			supplier.Email = supplierModel.Email;
			supplier.UpdatedAt = DateTime.UtcNow;

			_context.Suppliers.Update(supplier);
			await _context.SaveChangesAsync();
			return supplier;
		}
	}
}
