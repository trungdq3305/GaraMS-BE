using GaraMS.Service.Services.AccountService;
using GaraMS.Service.Services.AppointmentService;
using GaraMS.Service.Services.AutheticateService;
using GaraMS.Service.Services.ServiceService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddService(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuthenticateService, AuthenticateService>();
			services.AddScoped<IAppointmentService, AppointmentService>();
			services.AddScoped<IServiceService, ServiceService>();
			return services;
        }
    }
}
