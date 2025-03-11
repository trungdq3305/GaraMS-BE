using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.SupplierService
{
    public interface ISupplierService
    {
		Task<ResultModel> GetAllSupplierAsync();
		Task<ResultModel> GetSupplierByIdAsync(int id);
		Task<ResultModel> CreateSupplierAsync(string token, SupplierModel supplierModel);
		Task<ResultModel> UpdateSupplierAsync(string token, int id, SupplierModel supplierModel);
		Task<ResultModel> DeleteSupplierAsync(string token, int id);

		Task<ResultModel> GetAllInventorySuppliersAsync();
		Task<ResultModel> AssignInventoryToSupplierAsync(InventorySupplierModel model);
	}
}
