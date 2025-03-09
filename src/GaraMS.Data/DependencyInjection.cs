using GaraMS.Data.Repositories.AppointmentRepo;
using GaraMS.Data.Repositories.InventoryRepo;
using GaraMS.Data.Repositories.PromotionRepo;
using GaraMS.Data.Repositories.ServiceRepo;
using GaraMS.Data.Repositories.SupplierRepo;
using GaraMS.Data.Repositories.UserRepo;
using GaraMS.Data.Repositories.VehicleRepo;
using GaraMS.Data.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepository(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            services.AddScoped<IUserRepo, UserRepo>();
			services.AddScoped<IAppointmentRepo, AppointmentRepo>();
			services.AddScoped<IServiceRepo, ServiceRepo>();
            services.AddScoped<IVehicleRepo, VehicleRepo>();
            services.AddScoped<IPromoRepo, PromoRepo>();
            services.AddScoped<ISupplierRepo, SupplierRepo>();
            services.AddScoped<IInventoryRepo, InventoryRepo>();
            return services;
        }
    }
}
