using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.InventoryModel;
using GaraMS.Data.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.InventoryService
{
    public interface IInventoryService
    {
		Task<ResultModel> GetAllInventoriesAsync();
		Task<ResultModel> GetInventoryByIdAsync(int id);
		Task<ResultModel> CreateInventoryAsync(string token, InventoryModel model);
		Task<ResultModel> UpdateInventoryAsync(string token, int id, InventoryModel model);
		Task<ResultModel> DeleteInventoryAsync(string token, int id);
	}
}
