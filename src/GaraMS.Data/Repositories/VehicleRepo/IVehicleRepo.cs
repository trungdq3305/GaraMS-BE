using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.VehicleModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.VehicleRepo
{
    public interface IVehicleRepo
    {
        Task<Vehicle> GetVehicleByUserId(int id);
        Task<Vehicle> searchVehicle(VehicleSearch vehicleSearch);
        Task<Vehicle> createVehicle(Vehicle vehicle);
        Task<Vehicle> updateVehicle(EditVehicle vehicle);
    }
}
