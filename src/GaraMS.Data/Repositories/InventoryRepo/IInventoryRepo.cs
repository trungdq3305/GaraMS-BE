using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.InventoryModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.InventoryRepo
{
    public interface IInventoryRepo
    {
		Task<List<InventoryModel>> GetAllInventoriesAsync();
		Task<Inventory> GetInventoryByIdAsync(int id);
		Task<Inventory> CreateInventoryAsync(InventoryModel model);
		Task<Inventory> UpdateInventoryAsync(int id, InventoryModel model);
		Task<Inventory> DeleteInventoryAsync(int id);
	}
}
