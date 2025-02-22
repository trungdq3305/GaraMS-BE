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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GaraMS.Service.Services.VehicleService;

namespace GaraMS.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddService(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IValidateService, ValidateService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
			services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IVehicleService, VehicleService>();
            return services;
        }
    }
}
