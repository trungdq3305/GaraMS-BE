using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.WarrantyHistoryModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.WarrantyHistoryRepo
{
    public interface IWarrantyHistoryRepo
    {
		Task<List<WarrantyHistoryModel>> GetAllWarrantyHistoriesAsync();
		Task<WarrantyHistory> GetWarrantyHistoryByIdAsync(int id);
		Task<WarrantyHistory> CreateWarrantyHistoryAsync(WarrantyHistoryModel model);
		Task<WarrantyHistory> UpdateWarrantyHistoryAsync(int id, WarrantyHistoryModel model);
		Task<WarrantyHistory> DeleteWarrantyHistoryAsync(int id);
	}
}
