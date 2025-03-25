using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GaraMS.Data.ViewModels;

namespace GaraMS.Data.ViewModels.InventoryModel
{
    public class InventoryModel
    {
        public int InventoryId { get; set; }
        public string Name { get; set; }
		public string Description { get; set; }
		public string Unit { get; set; }
		public decimal? InventoryPrice { get; set; }
		public bool? Status { get; set; }
		public List<SupplierModel> InventorySuppliers { get; set; } = new List<SupplierModel>();
		public List<ServiceModel> ServiceInventories { get; set; } = new List<ServiceModel>();
	}
}
