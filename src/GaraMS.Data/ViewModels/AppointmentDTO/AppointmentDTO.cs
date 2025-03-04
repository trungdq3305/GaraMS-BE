using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.ViewModels.AppointmentDTO
{
	public class AppointmentDTO
	{
		public int? AppointmentId { get; set; }
		public DateTime? Date { get; set; }
		public string Note { get; set; }
		public string Status { get; set; }
		public int? VehicleId { get; set; }
		public List<int> ServiceIds { get; set; } = new List<int>();
	}

    public class RecentAppointmentDTO
    {
        public int AppointmentId { get; set; }
        public DateTime Date { get; set; }  // Changed from DateTime? to DateTime
        public string Status { get; set; }
        public string VehiclePlate { get; set; }  // Added this property
        public List<string> Services { get; set; }  // Added this property
        public string StatusName { get; set; }  // Added this property

        public RecentAppointmentDTO()
        {
            Status = "Pending";
            VehiclePlate = "N/A";
            Services = new List<string>();
            StatusName = "Unknown";
        }
    }
}
