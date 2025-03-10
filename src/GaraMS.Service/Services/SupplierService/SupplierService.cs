using GaraMS.Data.Models;
using GaraMS.Data.Repositories.SupplierRepo;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.SupplierService
{
	public class SupplierService : ISupplierService
	{
		private readonly ISupplierRepo _supplierRepo;

		public SupplierService(ISupplierRepo supplierRepo) 
		{
			_supplierRepo = supplierRepo;
		}
		public async Task<ResultModel> CreateSupplierAsync(string token, SupplierModel supplierModel)
		{
			var supplier = await _supplierRepo.CreateSupplierAsync(supplierModel);
			if (supplier == null) 
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to create supplier" };

			return new ResultModel { IsSuccess = true, Code = 201, Data = supplier, Message = "Supplier created successfully" };
		}

		public async Task<ResultModel> DeleteSupplierAsync(string token, int id)
		{
			var supplier = await _supplierRepo.DeleteSupplierAsync(id);
			if (supplier == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to delete supplier" };

			return new ResultModel { IsSuccess = true, Code = 200, Message = "Supplier deleted successfully" };
		}

		public async Task<ResultModel> GetAllSupplierAsync()
		{
			var suppliers = await _supplierRepo.GetAllSupplierAsync();
			return new ResultModel { IsSuccess = true, Code = 200, Data = suppliers, Message = "Suppliers retrieved successfully" };
		}

		public async Task<ResultModel> GetSupplierByIdAsync(int id)
		{
			var supplier = await _supplierRepo.GetSupplierByIdAsync(id);
			if (supplier == null)
				return new ResultModel { IsSuccess = false, Code = 404, Message = "Supplier not found" };

			return new ResultModel { IsSuccess = true, Code = 200, Data = supplier, Message = "Supplier retrieved successfully" };
		}

		public async Task<ResultModel> UpdateSupplierAsync(string token, int id, SupplierModel supplierModel)
		{
			var supplier = await _supplierRepo.UpdateSupplierAsync(id, supplierModel);
			if (supplier == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to update supplier" };

			return new ResultModel { IsSuccess = true, Code = 200, Data = supplier, Message = "Supplier updated successfully" };
		}
	}
}
