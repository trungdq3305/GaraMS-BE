using GaraMS.Data.ViewModels.DashboardModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.DashboardService
{
    public interface IDashboardService
    {
        Task<DashboardModel> GetDashboardDataAsync();
        Task<List<TopServiceDTO>> GetTopServicesAsync(int count = 5);
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<RecentAppointmentDTO>> GetRecentAppointmentsAsync(int count = 5);
    }
}
