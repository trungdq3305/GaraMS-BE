using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Data.ViewModels.WarrantyHistoryModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.WarrantyHistoryService
{
    public interface IWarrantyHistoryService
    {
		Task<ResultModel> GetAllWarrantyHistoriesAsync(string? token);
		Task<ResultModel> GetWarrantyHistoryByIdAsync(string? token, int id);
		Task<ResultModel> CreateWarrantyHistoryAsync(string? token, WarrantyHistoryModel model);
		Task<ResultModel> UpdateWarrantyHistoryAsync(string? token, int id, WarrantyHistoryModel model);
		Task<ResultModel> DeleteWarrantyHistoryAsync(string? token, int id);
		Task<ResultModel> CreateWarrantyPeriodForAppointmentAsync(string? token, int appointmentId);
		Task<ResultModel> GetByLoginAsync(string? token);

    }
}
