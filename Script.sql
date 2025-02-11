-- Create Database
CREATE DATABASE GaraManagementSystem
GO

USE GaraManagementSystem
GO

-- Create Tables
CREATE TABLE UserRoles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL
);

CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) UNIQUE,
    PhoneNumber NVARCHAR(20),
    FullName NVARCHAR(100),
    Address NVARCHAR(255),
    Status BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    RoleID INT FOREIGN KEY REFERENCES UserRoles(RoleId)
);

CREATE TABLE Gara (
    GaraId INT IDENTITY(1,1) PRIMARY KEY,
    GaraNumber NVARCHAR(50) NOT NULL UNIQUE,
    UserId INT FOREIGN KEY REFERENCES Users(UserId)
);

CREATE TABLE Staffs (
    StaffId INT IDENTITY(1,1) PRIMARY KEY,
    Gender NVARCHAR(10),
    Salary DECIMAL(18,2),
    UserId INT FOREIGN KEY REFERENCES Users(UserId)
);

CREATE TABLE Customers (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    Gender NVARCHAR(10),
    Note NVARCHAR(MAX),
    UserId INT FOREIGN KEY REFERENCES Users(UserId)
);

CREATE TABLE Vehicles (
    VehicleId INT IDENTITY(1,1) PRIMARY KEY,
    PlateNumber NVARCHAR(20) NOT NULL UNIQUE,
    Brand NVARCHAR(50),
    Model NVARCHAR(50),
    CustomerId INT FOREIGN KEY REFERENCES Customers(CustomerId)
);

CREATE TABLE AppointmentStatus (
    AppointmentStatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusName NVARCHAR(50) NOT NULL
);

CREATE TABLE Feedbacks (
    FeedbackId INT IDENTITY(1,1) PRIMARY KEY,
    Comment NVARCHAR(MAX),
    Rating INT,
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Invoices (
    InvoiceId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT FOREIGN KEY REFERENCES Customers(CustomerId),
    Date DATETIME DEFAULT GETDATE(),
    PaymentMethod NVARCHAR(50),
    TotalAmount DECIMAL(18,2),
    Status NVARCHAR(50),
    FeedbackId INT FOREIGN KEY REFERENCES Feedbacks(FeedbackId)
);

CREATE TABLE Appointments (
    AppointmentId INT IDENTITY(1,1) PRIMARY KEY,
    Date DATETIME,
    Note NVARCHAR(MAX),
    Status NVARCHAR(50),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    WaitingTime INT,
    RejectReason NVARCHAR(MAX),
    VehicleId INT FOREIGN KEY REFERENCES Vehicles(VehicleId),
    AppointmentStatusId INT FOREIGN KEY REFERENCES AppointmentStatus(AppointmentStatusId),
    InvoiceId INT FOREIGN KEY REFERENCES Invoices(InvoiceId)
);

CREATE TABLE InvoiceDetail (
    InvoiceDetailId INT IDENTITY(1,1) PRIMARY KEY,
    Price DECIMAL(18,2),
    InvoiceId INT FOREIGN KEY REFERENCES Invoices(InvoiceId)
);

CREATE TABLE Reports (
    ReportId INT IDENTITY(1,1) PRIMARY KEY,
    Problem NVARCHAR(MAX),
    Title NVARCHAR(255),
    Description NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    CustomerId INT FOREIGN KEY REFERENCES Customers(CustomerId)
);

CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    Status BIT DEFAULT 1
);

CREATE TABLE Services (
    ServiceId INT IDENTITY(1,1) PRIMARY KEY,
    ServiceName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    ServicePrice DECIMAL(18,2),
    InventoryPrice DECIMAL(18,2),
    Promotion DECIMAL(18,2),
    TotalPrice DECIMAL(18,2),
    EstimatedTime INT,
    Status BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    WarrantyPeriod INT,
    CategoryId INT FOREIGN KEY REFERENCES Categories(CategoryId)
);

CREATE TABLE AppointmentService (
    AppointmentServiceId INT IDENTITY(1,1) PRIMARY KEY,
    ServiceId INT FOREIGN KEY REFERENCES Services(ServiceId),
    AppointmentId INT FOREIGN KEY REFERENCES Appointments(AppointmentId)
);

CREATE TABLE WarrantyHistory (
    WarrantyHistoryId INT IDENTITY(1,1) PRIMARY KEY,
    StartDay DATETIME,
    EndDay DATETIME,
    Note NVARCHAR(MAX),
    Status BIT DEFAULT 1,
    ServiceId INT FOREIGN KEY REFERENCES Services(ServiceId)
);

CREATE TABLE Specialized (
    SpecializedId INT IDENTITY(1,1) PRIMARY KEY,
    SpecializedName NVARCHAR(100) NOT NULL
);

CREATE TABLE Employees (
    EmployeeId INT IDENTITY(1,1) PRIMARY KEY,
    Salary DECIMAL(18,2),
    SpecializedId INT FOREIGN KEY REFERENCES Specialized(SpecializedId),
    UserId INT FOREIGN KEY REFERENCES Users(UserId)
);

CREATE TABLE ServiceEmployee (
    ServiceEmployeeId INT IDENTITY(1,1) PRIMARY KEY,
    ServiceId INT FOREIGN KEY REFERENCES Services(ServiceId),
    EmployeeId INT FOREIGN KEY REFERENCES Employees(EmployeeId)
);

CREATE TABLE Promotions (
    PromotionId INT IDENTITY(1,1) PRIMARY KEY,
    PromotionName NVARCHAR(100) NOT NULL,
    StartDate DATETIME,
    EndDate DATETIME,
    DiscountPercent DECIMAL(5,2)
);

CREATE TABLE ServicePromotion (
    ServicePromotionId INT IDENTITY(1,1) PRIMARY KEY,
    ServiceId INT FOREIGN KEY REFERENCES Services(ServiceId),
    PromotionId INT FOREIGN KEY REFERENCES Promotions(PromotionId)
);

CREATE TABLE Inventories (
    InventoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    Unit NVARCHAR(50),
    Price DECIMAL(18,2),
    Status BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME
);

CREATE TABLE ServiceInventory (
    ServiceInventoryId INT IDENTITY(1,1) PRIMARY KEY,
    InventoryId INT FOREIGN KEY REFERENCES Inventories(InventoryId),
    ServiceId INT FOREIGN KEY REFERENCES Services(ServiceId)
);

CREATE TABLE Suppliers (
    SupplierId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    Address NVARCHAR(255),
    Status BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME
);

CREATE TABLE InventorySupplier (
    InventorySupplierId INT IDENTITY(1,1) PRIMARY KEY,
    InventoryId INT FOREIGN KEY REFERENCES Inventories(InventoryId),
    SupplierId INT FOREIGN KEY REFERENCES Suppliers(SupplierId)
);
