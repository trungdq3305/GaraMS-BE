using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.ViewModels.DashboardModel
{
    public class DashboardModel
    {
        public int TotalServices { get; set; }
        public int TotalCategories { get; set; }
        public int TotalActiveServices { get; set; }
        public int TotalActiveCategories { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalPendingAppointments { get; set; }
        public int TotalCompletedAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalEmployees { get; set; }
        public List<RecentAppointmentDTO> RecentAppointments { get; set; }
        public List<TopServiceDTO> TopServices { get; set; }
    }

    public class RecentAppointmentDTO
    {
        public int AppointmentId { get; set; }
        public string CustomerName { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class TopServiceDTO
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int BookingCount { get; set; }
    }
}
