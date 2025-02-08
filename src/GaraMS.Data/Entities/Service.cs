﻿using System;
using System.Collections.Generic;

namespace GaraMS.Data.Entities;

public partial class Service
{
    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? ServicePrice { get; set; }

    public decimal? InventoryPrice { get; set; }

    public decimal? Promotion { get; set; }

    public decimal? TotalPrice { get; set; }

    public int? EstimatedTime { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? WarrantyPeriod { get; set; }

    public int? CategoryId { get; set; }

    public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<ServiceEmployee> ServiceEmployees { get; set; } = new List<ServiceEmployee>();

    public virtual ICollection<ServiceInventory> ServiceInventories { get; set; } = new List<ServiceInventory>();

    public virtual ICollection<ServicePromotion> ServicePromotions { get; set; } = new List<ServicePromotion>();

    public virtual ICollection<WarrantyHistory> WarrantyHistories { get; set; } = new List<WarrantyHistory>();
}
