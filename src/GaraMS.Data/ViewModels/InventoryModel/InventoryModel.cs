using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.ViewModels.InventoryModel
{
    public class InventoryModel
    {
		public string Name { get; set; }
		public string Description { get; set; }
		public string Unit { get; set; }
		public decimal? Price { get; set; }
		public bool? Status { get; set; }
	}
}
