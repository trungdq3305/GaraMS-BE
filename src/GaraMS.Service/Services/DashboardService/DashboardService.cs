using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.DashboardModel;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Service.Services.TokenService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private readonly GaraManagementSystemContext _context;
        private readonly ILogger<DashboardService> _logger;
        private readonly ITokenService _tokenService;

        public DashboardService(GaraManagementSystemContext context, ILogger<DashboardService> logger, ITokenService tokenService)
        {
            _context = context;
            _logger = logger;
            _tokenService = tokenService;
        }

        public async Task<ResultModel> GetDashboardDataAsync()
        {
            try
            {
                var totalRevenueResult = await GetTotalRevenueAsync();
                if (!totalRevenueResult.IsSuccess)
                {
                    return totalRevenueResult;
                }

                var recentAppointmentsResult = await GetRecentAppointmentsAsync();
                if (!recentAppointmentsResult.IsSuccess)
                {
                    return recentAppointmentsResult;
                }

                var topServicesResult = await GetTopServicesAsync();
                if (!topServicesResult.IsSuccess)
                {
                    return topServicesResult;
                }

                var dashboardData = new DashboardModel
                {
                    TotalServices = await _context.Services.CountAsync(),
                    TotalCategories = await _context.Categories.CountAsync(),
                    TotalActiveServices = await _context.Services.CountAsync(s => s.Status == true),
                    TotalActiveCategories = await _context.Categories.CountAsync(c => c.Status == true),
                    TotalAppointments = await _context.Appointments.CountAsync(),
                    TotalPendingAppointments = await _context.Appointments.CountAsync(a => a.Status == "Pending"),
                    TotalCompletedAppointments = await _context.Appointments.CountAsync(a => a.Status == "Completed"),
                    TotalCustomers = await _context.Customers.CountAsync(),
                    TotalEmployees = await _context.Employees.CountAsync(),
                    TotalRevenue = (decimal)totalRevenueResult.Data,
                    RecentAppointments = (List<RecentAppointmentDTO>)recentAppointmentsResult.Data,
                    TopServices = (List<TopServiceDTO>)topServicesResult.Data
                };

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = 200,
                    Message = "Dashboard data retrieved successfully.",
                    Data = dashboardData
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard data");
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = "Internal server error"
                };
            }
        }

        public async Task<ResultModel> GetRecentAppointmentsAsync(int count = 5)
        {
            try
            {
                var appointments = await _context.Appointments
                    .Include(a => a.Vehicle)
                        .ThenInclude(v => v.Customer)
                            .ThenInclude(c => c.User)
                    .OrderByDescending(a => a.Date)
                    .Take(count)
                    .Select(a => new RecentAppointmentDTO
                    {
                        AppointmentId = a.AppointmentId,
                        CustomerName = a.Vehicle != null && a.Vehicle.Customer != null && a.Vehicle.Customer.User != null
                        ? a.Vehicle.Customer.User.FullName
                     : "Unknown", // Check for null
                        Date = (DateTime)a.Date, // Ensure a.Date is not null
                        Status = a.Status,
                        TotalAmount = (decimal)(a.Invoice != null ? a.Invoice.TotalAmount : 0) // Check for null
                    })
                    .ToListAsync();

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = 200,
                    Message = "Recent appointments retrieved successfully.",
                    Data = appointments
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent appointments");
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = "Internal server error"
                };
            }
        }

        public async Task<ResultModel> GetTopServicesAsync(int count = 5)
        {
            try
            {
                var topServices = await _context.Services
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

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = 200,
                    Message = "Top services retrieved successfully.",
                    Data = topServices
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top services");
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = "Internal server error"
                };
            }
        }

        public async Task<ResultModel> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.Invoices.AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(i => i.Date >= startDate);

                if (endDate.HasValue)
                    query = query.Where(i => i.Date <= endDate);

                var totalRevenue = await query.SumAsync(i => i.TotalAmount);

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = 200,
                    Message = "Total revenue retrieved successfully.",
                    Data = totalRevenue
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving total revenue");
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = "Internal server error"
                };
            }
        }
    }
}