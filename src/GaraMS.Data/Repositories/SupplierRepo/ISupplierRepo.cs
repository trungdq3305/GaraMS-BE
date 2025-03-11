using GaraMS.Data.Models;
using GaraMS.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.SupplierRepo
{
    public interface ISupplierRepo
    {
        Task<List<Supplier>> GetAllSupplierAsync();
        Task<Supplier> GetSupplierByIdAsync(int id);
        Task<Supplier> CreateSupplierAsync(SupplierModel supplierModel);
        Task<Supplier> UpdateSupplierAsync(int id, SupplierModel supplierModel);
        Task<Supplier> DeleteSupplierAsync(int id);

		Task<List<InventorySupplier>> GetAllInventorySuppliersAsync();
		Task<bool> AssignInventoryToSupplierAsync(int inventoryId, int supplierId);
	}
}
