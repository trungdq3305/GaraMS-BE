﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GaraMS.Data.Entities;

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

    public virtual DbSet<InventorySupplier> InventorySuppliers { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceEmployee> ServiceEmployees { get; set; }

    public virtual DbSet<ServiceInventory> ServiceInventories { get; set; }

    public virtual DbSet<ServicePromotion> ServicePromotions { get; set; }

    public virtual DbSet<Specialized> Specializeds { get; set; }

    public virtual DbSet<Staff> Staffs { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<WarrantyHistory> WarrantyHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=GaraManagementSystem;Uid=sa;Pwd=12345;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCC26A410717");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.AppointmentStatus).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.AppointmentStatusId)
                .HasConstraintName("FK__Appointme__Appoi__59063A47");

            entity.HasOne(d => d.Invoice).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK__Appointme__Invoi__59FA5E80");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.VehicleId)
                .HasConstraintName("FK__Appointme__Vehic__5812160E");
        });

        modelBuilder.Entity<AppointmentService>(entity =>
        {
            entity.HasKey(e => e.AppointmentServiceId).HasName("PK__Appointm__3B38F396AB667DD6");

            entity.ToTable("AppointmentService");

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentServices)
                .HasForeignKey(d => d.AppointmentId)
                .HasConstraintName("FK__Appointme__Appoi__6D0D32F4");

            entity.HasOne(d => d.Service).WithMany(p => p.AppointmentServices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__Appointme__Servi__6C190EBB");
        });

        modelBuilder.Entity<AppointmentStatus>(entity =>
        {
            entity.HasKey(e => e.AppointmentStatusId).HasName("PK__Appointm__A619B660BE35D577");

            entity.ToTable("AppointmentStatus");

            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0B97B22172");

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D859599F6F");

            entity.Property(e => e.Gender).HasMaxLength(10);

            entity.HasOne(d => d.User).WithMany(p => p.Customers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Customers__UserI__46E78A0C");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F11D9A22153");

            entity.Property(e => e.Salary).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Specialized).WithMany(p => p.Employees)
                .HasForeignKey(d => d.SpecializedId)
                .HasConstraintName("FK__Employees__Speci__75A278F5");

            entity.HasOne(d => d.User).WithMany(p => p.Employees)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Employees__UserI__76969D2E");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDD695A08CC4");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Gara>(entity =>
        {
            entity.HasKey(e => e.GaraId).HasName("PK__Gara__E6DDAA6048D827DE");

            entity.ToTable("Gara");

            entity.HasIndex(e => e.GaraNumber, "UQ__Gara__8CEDF0473C2FF164").IsUnique();

            entity.Property(e => e.GaraNumber).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Garas)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Gara__UserId__412EB0B6");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PK__Inventor__F5FDE6B3F68EDE55");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<InventorySupplier>(entity =>
        {
            entity.HasKey(e => e.InventorySupplierId).HasName("PK__Inventor__92729B621A969F52");

            entity.ToTable("InventorySupplier");

            entity.HasOne(d => d.Inventory).WithMany(p => p.InventorySuppliers)
                .HasForeignKey(d => d.InventoryId)
                .HasConstraintName("FK__Inventory__Inven__0E6E26BF");

            entity.HasOne(d => d.Supplier).WithMany(p => p.InventorySuppliers)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK__Inventory__Suppl__0F624AF8");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAB5FC829FFC");

            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Invoices__Custom__52593CB8");

            entity.HasOne(d => d.Feedback).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.FeedbackId)
                .HasConstraintName("FK__Invoices__Feedba__5441852A");
        });

        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.InvoiceDetailId).HasName("PK__InvoiceD__1F1578115F366114");

            entity.ToTable("InvoiceDetail");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK__InvoiceDe__Invoi__5CD6CB2B");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("PK__Promotio__52C42FCF77633D52");

            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.PromotionName).HasMaxLength(100);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Reports__D5BD48057C508D00");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.Reports)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Reports__Custome__60A75C0F");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB00A3FA35907");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.InventoryPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Promotion).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ServiceName).HasMaxLength(100);
            entity.Property(e => e.ServicePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.Services)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Services__Catego__693CA210");
        });

        modelBuilder.Entity<ServiceEmployee>(entity =>
        {
            entity.HasKey(e => e.ServiceEmployeeId).HasName("PK__ServiceE__3FAF6506510E45F1");

            entity.ToTable("ServiceEmployee");

            entity.HasOne(d => d.Employee).WithMany(p => p.ServiceEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__ServiceEm__Emplo__7A672E12");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceEmployees)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__ServiceEm__Servi__797309D9");
        });

        modelBuilder.Entity<ServiceInventory>(entity =>
        {
            entity.HasKey(e => e.ServiceInventoryId).HasName("PK__ServiceI__0EF8880364F8158F");

            entity.ToTable("ServiceInventory");

            entity.HasOne(d => d.Inventory).WithMany(p => p.ServiceInventories)
                .HasForeignKey(d => d.InventoryId)
                .HasConstraintName("FK__ServiceIn__Inven__06CD04F7");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceInventories)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__ServiceIn__Servi__07C12930");
        });

        modelBuilder.Entity<ServicePromotion>(entity =>
        {
            entity.HasKey(e => e.ServicePromotionId).HasName("PK__ServiceP__E30198394E99EFE4");

            entity.ToTable("ServicePromotion");

            entity.HasOne(d => d.Promotion).WithMany(p => p.ServicePromotions)
                .HasForeignKey(d => d.PromotionId)
                .HasConstraintName("FK__ServicePr__Promo__00200768");

            entity.HasOne(d => d.Service).WithMany(p => p.ServicePromotions)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__ServicePr__Servi__7F2BE32F");
        });

        modelBuilder.Entity<Specialized>(entity =>
        {
            entity.HasKey(e => e.SpecializedId).HasName("PK__Speciali__D22EFDA34E7AC793");

            entity.ToTable("Specialized");

            entity.Property(e => e.SpecializedName).HasMaxLength(100);
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__Staffs__96D4AB17AC35C885");

            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Salary).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.Staff)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Staffs__UserId__440B1D61");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666B49B2258F7");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C826DE910");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053486E64AE9").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__Users__C9F28456EAA4B192").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__RoleID__3D5E1FD2");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__UserRole__8AFACE1AFD06BA09");

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicles__476B5492296C6C67");

            entity.HasIndex(e => e.PlateNumber, "UQ__Vehicles__03692624D3470661").IsUnique();

            entity.Property(e => e.Brand).HasMaxLength(50);
            entity.Property(e => e.Model).HasMaxLength(50);
            entity.Property(e => e.PlateNumber).HasMaxLength(20);

            entity.HasOne(d => d.Customer).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Vehicles__Custom__4AB81AF0");
        });

        modelBuilder.Entity<WarrantyHistory>(entity =>
        {
            entity.HasKey(e => e.WarrantyHistoryId).HasName("PK__Warranty__5D65AB48AD0E3172");

            entity.ToTable("WarrantyHistory");

            entity.Property(e => e.EndDay).HasColumnType("datetime");
            entity.Property(e => e.StartDay).HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Service).WithMany(p => p.WarrantyHistories)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__WarrantyH__Servi__70DDC3D8");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
