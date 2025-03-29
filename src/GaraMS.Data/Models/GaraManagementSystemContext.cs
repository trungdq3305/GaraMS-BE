﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GaraMS.Data.Models;

public partial class GaraManagementSystemContext : DbContext
{
    public GaraManagementSystemContext()
    {
    }

    public GaraManagementSystemContext(DbContextOptions<GaraManagementSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentService> AppointmentServices { get; set; }

    public virtual DbSet<AppointmentStatus> AppointmentStatuses { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Gara> Garas { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<InventoryInvoice> InventoryInvoices { get; set; }

    public virtual DbSet<InventoryInvoiceDetail> InventoryInvoiceDetails { get; set; }

    public virtual DbSet<InventorySupplier> InventorySuppliers { get; set; }

    public virtual DbSet<InventoryWarranty> InventoryWarranties { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Manager> Managers { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceEmployee> ServiceEmployees { get; set; }

    public virtual DbSet<ServiceInventory> ServiceInventories { get; set; }

    public virtual DbSet<ServicePromotion> ServicePromotions { get; set; }

    public virtual DbSet<Specialized> Specializeds { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<WarrantyHistory> WarrantyHistories { get; set; }

    public static string GetConnectionString(string connectionStringName)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = config.GetConnectionString(connectionStringName);
        return connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCC211D5B523");

            entity.ToTable(tb => tb.HasTrigger("trg_CreateInvoiceOnAppointmentAccepted"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.AppointmentStatus).WithMany(p => p.Appointments).HasConstraintName("FK__Appointme__Appoi__7F2BE32F");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Appointments).HasConstraintName("FK__Appointme__Vehic__7E37BEF6");
        });

        modelBuilder.Entity<AppointmentService>(entity =>
        {
            entity.HasKey(e => e.AppointmentServiceId).HasName("PK__Appointm__3B38F396E7D71073");

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentServices).HasConstraintName("FK__Appointme__Appoi__17036CC0");

            entity.HasOne(d => d.Employee).WithMany(p => p.AppointmentServices).HasConstraintName("FK__Appointme__Emplo__17F790F9");

            entity.HasOne(d => d.Service).WithMany(p => p.AppointmentServices).HasConstraintName("FK__Appointme__Servi__160F4887");
        });

        modelBuilder.Entity<AppointmentStatus>(entity =>
        {
            entity.HasKey(e => e.AppointmentStatusId).HasName("PK__Appointm__A619B6607CB15C50");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0BDF752DCD");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D87FFD68AD");

            entity.HasOne(d => d.User).WithMany(p => p.Customers).HasConstraintName("FK__Customers__UserI__71D1E811");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F116CA3B201");

            entity.HasOne(d => d.Specialized).WithMany(p => p.Employees).HasConstraintName("FK__Employees__Speci__6E01572D");

            entity.HasOne(d => d.User).WithMany(p => p.Employees).HasConstraintName("FK__Employees__UserI__6EF57B66");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDD671BA8173");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Gara>(entity =>
        {
            entity.HasKey(e => e.GaraId).HasName("PK__Gara__E6DDAA6082930AD0");

            entity.HasOne(d => d.User).WithMany(p => p.Garas).HasConstraintName("FK__Gara__UserId__66603565");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PK__Inventor__F5FDE6B354245159");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<InventoryInvoice>(entity =>
        {
            entity.HasKey(e => e.InventoryInvoiceId).HasName("PK__Inventor__7678E42FBCBB7C9E");

            entity.Property(e => e.Date).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.InventoryInvoices).HasConstraintName("FK_InventoryInvoice_User");
        });

        modelBuilder.Entity<InventoryInvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.InventoryInvoiceDetailId).HasName("PK__Inventor__1B9F5647CA4B0DA8");

            entity.HasOne(d => d.Inventory).WithMany(p => p.InventoryInvoiceDetails).HasConstraintName("FK__Inventory__Inven__65370702");

            entity.HasOne(d => d.InventoryInvoice).WithMany(p => p.InventoryInvoiceDetails).HasConstraintName("FK__Inventory__Inven__662B2B3B");
        });

        modelBuilder.Entity<InventorySupplier>(entity =>
        {
            entity.HasKey(e => e.InventorySupplierId).HasName("PK__Inventor__92729B627EA37B83");

            entity.HasOne(d => d.Inventory).WithMany(p => p.InventorySuppliers).HasConstraintName("FK__Inventory__Inven__339FAB6E");

            entity.HasOne(d => d.Supplier).WithMany(p => p.InventorySuppliers).HasConstraintName("FK__Inventory__Suppl__3493CFA7");
        });

        modelBuilder.Entity<InventoryWarranty>(entity =>
        {
            entity.HasKey(e => e.InventoryWarrantyId).HasName("PK__Inventor__1C8DF49A7592AE77");

            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Appointment).WithMany(p => p.InventoryWarranties).HasConstraintName("FK_InventoryWarranty_Appointment");

            entity.HasOne(d => d.InventoryInvoiceDetail).WithMany(p => p.InventoryWarranties).HasConstraintName("FK__Inventory__Inven__6BE40491");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAB5AD69767E");

            entity.Property(e => e.Date).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TotalAmount).HasDefaultValue(0m);

            entity.HasOne(d => d.Appointment).WithOne(p => p.Invoice).HasConstraintName("FK__Invoices__Appoin__02FC7413");

            entity.HasOne(d => d.Customer).WithMany(p => p.Invoices).HasConstraintName("FK__Invoices__Custom__03F0984C");
        });

        modelBuilder.Entity<Manager>(entity =>
        {
            entity.HasKey(e => e.ManagerId).HasName("PK__Manager__3BA2AAE1B77399BF");

            entity.HasOne(d => d.User).WithMany(p => p.Managers).HasConstraintName("FK__Manager__UserId__693CA210");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("PK__Promotio__52C42FCFE5A94500");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Reports__D5BD480525B5CCF6");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Customer).WithMany(p => p.Reports).HasConstraintName("FK__Reports__Custome__0A9D95DB");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB00AD9C0C41E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Category).WithMany(p => p.Services).HasConstraintName("FK__Services__Catego__1332DBDC");
        });

        modelBuilder.Entity<ServiceEmployee>(entity =>
        {
            entity.HasKey(e => e.ServiceEmployeeId).HasName("PK__ServiceE__3FAF650601D812E3");

            entity.HasOne(d => d.Employee).WithMany(p => p.ServiceEmployees).HasConstraintName("FK__ServiceEm__Emplo__1F98B2C1");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceEmployees).HasConstraintName("FK__ServiceEm__Servi__1EA48E88");
        });

        modelBuilder.Entity<ServiceInventory>(entity =>
        {
            entity.HasKey(e => e.ServiceInventoryId).HasName("PK__ServiceI__0EF88803D8FE9B41");

            entity.HasOne(d => d.Inventory).WithMany(p => p.ServiceInventories).HasConstraintName("FK__ServiceIn__Inven__2BFE89A6");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceInventories).HasConstraintName("FK__ServiceIn__Servi__2CF2ADDF");
        });

        modelBuilder.Entity<ServicePromotion>(entity =>
        {
            entity.HasKey(e => e.ServicePromotionId).HasName("PK__ServiceP__E3019839C5C30186");

            entity.HasOne(d => d.Promotion).WithMany(p => p.ServicePromotions).HasConstraintName("FK__ServicePr__Promo__25518C17");

            entity.HasOne(d => d.Service).WithMany(p => p.ServicePromotions).HasConstraintName("FK__ServicePr__Servi__245D67DE");
        });

        modelBuilder.Entity<Specialized>(entity =>
        {
            entity.HasKey(e => e.SpecializedId).HasName("PK__Speciali__D22EFDA3C634A7EC");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666B41335548B");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CC5BC7928");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasConstraintName("FK__Users__RoleID__628FA481");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__UserRole__8AFACE1A74DDEC10");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicles__476B549211F28E96");

            entity.HasOne(d => d.Customer).WithMany(p => p.Vehicles).HasConstraintName("FK__Vehicles__Custom__75A278F5");
        });

        modelBuilder.Entity<WarrantyHistory>(entity =>
        {
            entity.HasKey(e => e.WarrantyHistoryId).HasName("PK__Warranty__5D65AB4818F0D9DE");

            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Service).WithMany(p => p.WarrantyHistories).HasConstraintName("FK__WarrantyH__Servi__1BC821DD");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}