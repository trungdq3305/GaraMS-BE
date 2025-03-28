﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.ViewModels
{
    public class SupplierModel
    {
		public int SupplierId { get; set; }

		public string Name { get; set; }

		public string Phone { get; set; }

		public string Email { get; set; }
	}

	public class InventorySupplierModel
	{
		public int InventoryId { get; set; }
		public int SupplierId { get; set; }
	}
}
