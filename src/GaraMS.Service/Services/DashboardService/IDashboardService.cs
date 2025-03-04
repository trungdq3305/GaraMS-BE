using GaraMS.Data.ViewModels.DashboardModel;
using GaraMS.Data.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.DashboardService
{
    public interface IDashboardService
    {
        Task<ResultModel> GetDashboardDataAsync(string? token);
        Task<ResultModel> GetTopServicesAsync(int count = 5);
        Task<ResultModel> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<ResultModel> GetRecentAppointmentsAsync(int count = 5);
    }
}
