using GaraMS.Data.Repositories.InventoryRepo;
using GaraMS.Data.ViewModels.InventoryModel;
using GaraMS.Data.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.InventoryService
{
    public class InventoryService : IInventoryService
    {
		private readonly IInventoryRepo _inventoryRepo;

		public InventoryService(IInventoryRepo inventoryRepo) 
        {
            _inventoryRepo = inventoryRepo;
		}

		public async Task<ResultModel> CreateInventoryAsync(string token, InventoryModel model)
		{
			var inventory = await _inventoryRepo.CreateInventoryAsync(model);
			if (inventory == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to create inventory" };

			return new ResultModel { IsSuccess = true, Code = 201, Data = inventory, Message = "Inventory created successfully" };
		}

		public async Task<ResultModel> DeleteInventoryAsync(string token, int id)
		{
			var inventory = await _inventoryRepo.DeleteInventoryAsync(id);
			if (inventory == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to delete inventory" };

			return new ResultModel { IsSuccess = true, Code = 200, Message = "Inventory deleted successfully" };
		}

		public async Task<ResultModel> GetAllInventoriesAsync()
		{
			var inventories = await _inventoryRepo.GetAllInventoriesAsync();
			return new ResultModel { IsSuccess = true, Code = 200, Data = inventories, Message = "Inventories retrieved successfully" };
		}

		public async Task<ResultModel> GetInventoryByIdAsync(int id)
		{
			var inventory = await _inventoryRepo.GetInventoryByIdAsync(id);
			if (inventory == null)
				return new ResultModel { IsSuccess = false, Code = 404, Message = "Inventory not found" };

			return new ResultModel { IsSuccess = true, Code = 200, Data = inventory, Message = "Inventory retrieved successfully" };
		}

		public async Task<ResultModel> UpdateInventoryAsync(string token, int id, InventoryModel model)
		{
			var inventory = await _inventoryRepo.UpdateInventoryAsync(id, model);
			if (inventory == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to update inventory" };

			return new ResultModel { IsSuccess = true, Code = 200, Data = inventory, Message = "Inventory updated successfully" };
		}
	}
}
