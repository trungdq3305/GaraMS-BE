using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.DashboardModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private readonly GaraManagementSystemContext _context;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(GaraManagementSystemContext context, ILogger<DashboardService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<DashboardModel> GetDashboardDataAsync()
        {
            try
            {
                var dashboardData = new DashboardModel
                {
                    TotalServices = await _context.Services.CountAsync(),
                    TotalCategories = await _context.Categories.CountAsync(),
                    TotalActiveServices = await _context.Services.CountAsync(s => s.Status == true),
                    TotalActiveCategories = await _context.Categories.CountAsync(c => c.Status == true),
                    TotalAppointments = await _context.Appointments.CountAsync(),
                    TotalPendingAppointments = await _context.Appointments
                    .CountAsync(a => a.Status == "Pending"),
                    TotalCompletedAppointments = await _context.Appointments
                    .CountAsync(a => a.Status == "Completed"),
                    TotalCustomers = await _context.Customers.CountAsync(),
                    TotalEmployees = await _context.Employees.CountAsync(),
                    TotalRevenue = await GetTotalRevenueAsync(),
                    RecentAppointments = await GetRecentAppointmentsAsync(),
                    TopServices = await GetTopServicesAsync()
                };
                return dashboardData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard data");
                throw;
            }
        }

        public async Task<List<RecentAppointmentDTO>> GetRecentAppointmentsAsync(int count = 5)
        {
            return await _context.Appointments
                .Include(a => a.Vehicle)
                    .ThenInclude(v => v.Customer)
                        .ThenInclude(c => c.User)
                .OrderByDescending(a => a.Date)
                .Take(count)
                .Select(a => new RecentAppointmentDTO
                {
                    AppointmentId = a.AppointmentId,
                    CustomerName = a.Vehicle.Customer.User.FullName,
                    Date = (DateTime)a.Date,
                    Status = a.Status,
                    TotalAmount = (decimal)a.Invoice.TotalAmount
                })
                .ToListAsync();
        }

        public async Task<List<TopServiceDTO>> GetTopServicesAsync(int count = 5)
        {
            return await _context.Services
                .Include(s => s.AppointmentServices)
                .Select(s => new TopServiceDTO
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName,
                    BookingCount = s.AppointmentServices.Count,
                    Revenue = (decimal)s.AppointmentServices
                        .Sum(a => a.Appointment.Invoice.TotalAmount)
                })
                .OrderByDescending(s => s.BookingCount)
                .Take(count)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Invoices.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(i => i.Date >= startDate);

            if (endDate.HasValue)
                query = query.Where(i => i.Date <= endDate);

            return (decimal)await query.SumAsync(i => i.TotalAmount);
        }
    }
}
