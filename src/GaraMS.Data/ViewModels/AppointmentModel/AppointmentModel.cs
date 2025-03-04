using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.ViewModels.AppointmentModel
{
	public class AppointmentModel
	{
		public int? AppointmentId { get; set; }
		public DateTime? Date { get; set; }
		public string Note { get; set; }
		public string Status { get; set; }
		public int? VehicleId { get; set; }
		public List<int> ServiceIds { get; set; } = new List<int>();
	}
}
