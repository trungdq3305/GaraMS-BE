using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.ViewModels.VehicleModel
{
    public class EditVehicle
    {
        public int VehicleId { get; set; }
        public string PlateNumber { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }
    }
}
