﻿using System;
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
		public decimal? Price { get; set; }
		public string Description { get; set; }
	}

	public class AssignInventoryToServiceModel
	{
		public int InventoryId { get; set; }
		public int ServiceId { get; set; }
	}
}
