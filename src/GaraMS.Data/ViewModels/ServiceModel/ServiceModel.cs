using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.ViewModels
{
	public class ServiceModel
	{
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
		public decimal? ServicePrice { get; set; }
		public decimal? InventoryPrice { get; set; }
		public string Description { get; set; }
		public List<int> InventoryIds { get; set; } = new List<int>();
		public int? WarrantyPeriod { get; set; }
	}

	public class AssignInventoryToServiceModel
	{
		public int InventoryId { get; set; }
		public int ServiceId { get; set; }
	}
}
