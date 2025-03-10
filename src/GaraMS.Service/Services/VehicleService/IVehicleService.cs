using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Data.ViewModels.VehicleModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.VehicleService
{
    public interface IVehicleService
    {
        public Task<ResultModel> ViewListVehicleByLogin(string? token, Vehicle vehicle);
        public Task<ResultModel> ViewListVehicle(string? token, VehicleSearch vehicleSearch);
        public Task<ResultModel> CreateVehicle(string? token, CreateVehicle vehicle);
        public Task<ResultModel> EditVehicle(string? token, EditVehicle vehicle);
    }
}
