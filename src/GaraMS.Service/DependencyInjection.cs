using GaraMS.Data.Repositories.UserRepo;
using GaraMS.Service.Services.AccountService;
using GaraMS.Service.Services.AutheticateService;
using GaraMS.Service.Services.DashboardService;
using GaraMS.Service.Services.TokenService;
using GaraMS.Service.Services.UserService;
using GaraMS.Service.Services.Validate;
using GaraMS.Service.Services.AppointmentService;
using GaraMS.Service.Services.ServiceService;
using Microsoft.Extensions.DependencyInjection;
using GaraMS.Service.Services.VehicleService;
using GaraMS.Service.Services.Email;
using Microsoft.Extensions.Configuration;
using GaraMS.Service.Services.PromotionService;
using GaraMS.Service.Services.SupplierService;
using GaraMS.Service.Services.InventoryService;
using GaraMS.Service.Services.InvoicesService;
using GaraMS.Service.Services.EmployeeService;
using GaraMS.Service.Services.ReportService;

namespace GaraMS.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IValidateService, ValidateService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IPromotionService, PromotionService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IInvoicesService, InvoiceService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddSingleton(configuration);
            services.AddScoped<IEmailService, EmailService>(provider =>
            {
                return new EmailService(configuration);
            });

            return services;
        }
    }
}
