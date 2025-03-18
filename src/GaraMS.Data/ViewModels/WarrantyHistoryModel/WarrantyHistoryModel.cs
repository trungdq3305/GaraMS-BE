using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.ViewModels.WarrantyHistoryModel
{
    public class WarrantyHistoryModel
    {
		public int WarrantyHistoryId { get; set; }
		public DateTime? StartDay { get; set; }
		public DateTime? EndDay { get; set; }
		public string Note { get; set; }
		public bool? Status { get; set; }
		public int? ServiceId { get; set; }
	}
}
