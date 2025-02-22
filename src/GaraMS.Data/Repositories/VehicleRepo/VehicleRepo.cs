using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.VehicleModel;
using Microsoft.EntityFrameworkCore;
using Skincare.Repositories.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.VehicleRepo
{
    class VehicleRepo : GenericRepository<Vehicle>, IVehicleRepo
    {
        private readonly GaraManagementSystemContext _context;
        public VehicleRepo(GaraManagementSystemContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Vehicle> GetVehicleByUserId(int id)
        {
            return await _context.Vehicles.FirstOrDefaultAsync(v => v.CustomerId == id);
        }

        public async Task<Vehicle> searchVehicle(VehicleSearch vehicleSearch)
        {
            return await _context.Vehicles
                .Where(v => (vehicleSearch.VehicleId == 0 || v.VehicleId == vehicleSearch.VehicleId) &&
                            (string.IsNullOrEmpty(vehicleSearch.PlateNumber) || v.PlateNumber.Contains(vehicleSearch.PlateNumber)) &&
                            (string.IsNullOrEmpty(vehicleSearch.Brand) || v.Brand.Contains(vehicleSearch.Brand)) &&
                            (string.IsNullOrEmpty(vehicleSearch.Model) || v.Model.Contains(vehicleSearch.Model)))
                .FirstOrDefaultAsync();
        }
    }
}
