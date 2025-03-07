using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.DashboardModel;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Service.Services.AccountService;
using GaraMS.Service.Services.TokenService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private readonly GaraManagementSystemContext _context;
        private readonly ILogger<DashboardService> _logger;
        private readonly ITokenService _token;
        private readonly IAccountService _accountService;

        public DashboardService(
            GaraManagementSystemContext context,
            ILogger<DashboardService> logger,
            ITokenService tokenService,
            IAccountService accountService)
        {
            _context = context;
            _logger = logger;
            _token = tokenService;
            _accountService = accountService;
        }

        public async Task<ResultModel> GetDashboardDataAsync(string? token)
        {
            var resultModel = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.Unauthorized,
                Message = "Invalid token."
            };
            try
            {
                var decodeModel = _token.decode(token);
                var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 4 });
                if (!isValidRole)
                {
                    resultModel.Code = (int)HttpStatusCode.Forbidden;
                    resultModel.Message = "You don't have permission to perform this action.";
                    return resultModel;
                }

                if (!int.TryParse(decodeModel.userid, out int userId))
                {
                    return resultModel;
                }
                if (userId <= 0)
                {
                    return resultModel;
                }

                var totalRevenueResult = await GetTotalRevenueAsync();
                var recentAppointmentsResult = await GetRecentAppointmentsAsync();
                var topServicesResult = await GetTopServicesAsync();

                if (!totalRevenueResult.IsSuccess || !recentAppointmentsResult.IsSuccess || !topServicesResult.IsSuccess)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 500,
                        Message = "Error retrieving dashboard data"
                    };
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
                    TotalRevenue = (decimal)(totalRevenueResult.Data ?? 0m),
                    RecentAppointments = (List<RecentAppointmentDTO>)(recentAppointmentsResult.Data ?? new List<RecentAppointmentDTO>()),
                    TopServices = (List<TopServiceDTO>)(topServicesResult.Data ?? new List<TopServiceDTO>())
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
                    .Include(a => a.Invoice)  // Include Invoice
                    .OrderByDescending(a => a.Date)
                    .Take(count)
                    .Select(a => new RecentAppointmentDTO
                    {
                        AppointmentId = a.AppointmentId,
                        CustomerName = a.Vehicle != null &&
                                     a.Vehicle.Customer != null &&
                                     a.Vehicle.Customer.User != null &&
                                     a.Vehicle.Customer.User.FullName != null
                                     ? a.Vehicle.Customer.User.FullName
                                     : "Unknown",
                        Date = a.Date ?? DateTime.Now,  // Provide default value for nullable DateTime
                        Status = a.Status ?? "Pending",  // Provide default value for nullable string
                        TotalAmount = a.Invoice != null && a.Invoice.TotalAmount.HasValue
                                     ? a.Invoice.TotalAmount.Value
                                     : 0m  // Provide default value for nullable decimal
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
                        .ThenInclude(a => a.Appointment)
                            .ThenInclude(a => a.Invoice)
                    .Select(s => new TopServiceDTO
                    {
                        ServiceId = s.ServiceId,
                        ServiceName = s.ServiceName ?? "Unknown",
                        BookingCount = s.AppointmentServices.Count,
                        Revenue = s.AppointmentServices
                    .Where(a => a.Appointment != null &&
                               a.Appointment.Invoice != null &&
                               a.Appointment.Invoice.TotalAmount.HasValue &&
                               a.Appointment.Invoice.Status == "1")  // Only include invoices with status "1"
                    .Sum(a => a.Appointment.Invoice.TotalAmount ?? 0m)
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

                var totalRevenue = await query
                   .Where(i => i.TotalAmount.HasValue && i.Status == "1")  // Only include invoices with status "1"
                   .SumAsync(i => i.TotalAmount ?? 0m);

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
