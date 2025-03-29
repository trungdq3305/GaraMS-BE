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

CREATE TABLE Manager (
    ManagerId INT IDENTITY(1,1) PRIMARY KEY,
    Gender NVARCHAR(10),
    Salary DECIMAL(18,2),
    UserId INT FOREIGN KEY REFERENCES Users(UserId)
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
);
CREATE TABLE Invoices (
    InvoiceId INT IDENTITY(1,1) PRIMARY KEY,
    AppointmentId INT UNIQUE FOREIGN KEY REFERENCES Appointments(AppointmentId),
    CustomerId INT FOREIGN KEY REFERENCES Customers(CustomerId),
    Date DATETIME DEFAULT GETDATE(),
    PaymentMethod NVARCHAR(50),
    TotalAmount DECIMAL(18,2) DEFAULT 0,
    Status NVARCHAR(50) CHECK (Status IN ('Unpaid', 'Paid', 'Cancelled'))
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
    AppointmentId INT FOREIGN KEY REFERENCES Appointments(AppointmentId),
	EmployeeId INT FOREIGN KEY REFERENCES Employees(EmployeeId)
);

CREATE TABLE WarrantyHistory (
    WarrantyHistoryId INT IDENTITY(1,1) PRIMARY KEY,
    StartDay DATETIME,
    EndDay DATETIME,
    Note NVARCHAR(MAX),
    Status BIT DEFAULT 1,
    ServiceId INT FOREIGN KEY REFERENCES Services(ServiceId)
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
SET IDENTITY_INSERT UserRoles ON;

INSERT INTO UserRoles (RoleId, RoleName) VALUES 
(1, 'Customer'),
(2, 'Employee'),
(3, 'Manager'),
(4, 'Admin');

SET IDENTITY_INSERT UserRoles OFF;
GO
CREATE TRIGGER trg_CreateInvoiceOnAppointmentAccepted
ON Appointments
AFTER UPDATE
AS
BEGIN
    DECLARE @AppointmentId INT;
    DECLARE @CustomerId INT;
    DECLARE @InvoiceId INT;
    DECLARE @TotalAmount DECIMAL(18,2);

    SELECT @AppointmentId = inserted.AppointmentId
    FROM inserted
    JOIN deleted ON inserted.AppointmentId = deleted.AppointmentId
    WHERE inserted.Status = 'Accept' AND deleted.Status <> 'Accept';
    IF @AppointmentId IS NOT NULL
    BEGIN

        SELECT @CustomerId = c.CustomerId
        FROM Appointments a
        JOIN Vehicles v ON a.VehicleId = v.VehicleId
        JOIN Customers c ON v.CustomerId = c.CustomerId
        WHERE a.AppointmentId = @AppointmentId;

        INSERT INTO Invoices (AppointmentId, CustomerId, PaymentMethod, TotalAmount, Status)
        VALUES (@AppointmentId, @CustomerId, 'Cash', 0, 'Unpaid');


        SET @InvoiceId = SCOPE_IDENTITY();
        SELECT @TotalAmount = COALESCE(SUM(s.TotalPrice), 0)
        FROM AppointmentService aps
        JOIN Services s ON aps.ServiceId = s.ServiceId
        WHERE aps.AppointmentId = @AppointmentId;

        UPDATE Invoices
        SET TotalAmount = @TotalAmount
        WHERE InvoiceId = @InvoiceId;

        PRINT 'Hóa đơn đã được tạo và cập nhật tổng tiền!';
    END;
END;
GO

-- T?o b?ng InventoryInvoice
CREATE TABLE InventoryInvoice (
    InventoryInvoiceId INT IDENTITY(1,1) PRIMARY KEY,
    Price DECIMAL(18,2),
    DiliverType NVARCHAR(100),
    PaymentMethod NVARCHAR(100),
    TotalAmount DECIMAL(18,2),
    Status NVARCHAR(50) ,
    Date DATETIME DEFAULT GETDATE()
);

-- T?o b?ng InventoryInvoiceDetail
CREATE TABLE InventoryInvoiceDetail (
    InventoryInvoiceDetailId INT IDENTITY(1,1) PRIMARY KEY,
    InventoryId INT FOREIGN KEY REFERENCES Inventories(InventoryId),
    InventoryInvoiceId INT FOREIGN KEY REFERENCES InventoryInvoice(InventoryInvoiceId),
    Price DECIMAL(18,2)
);

ALTER TABLE InventoryInvoice
ADD UserId INT;

ALTER TABLE InventoryInvoice
ADD CONSTRAINT FK_InventoryInvoice_User
FOREIGN KEY (UserId) REFERENCES Users(UserId);


-- Thêm bảng InventoryWarranty
CREATE TABLE InventoryWarranty (
    InventoryWarrantyId INT IDENTITY(1,1) PRIMARY KEY,
    StartDay DATETIME NOT NULL,
    EndDay DATETIME NOT NULL,
    Status BIT DEFAULT 1,
    InventoryInvoiceDetailId INT FOREIGN KEY REFERENCES InventoryInvoiceDetail(InventoryInvoiceDetailId)
);

-- Thêm cột warrantyperiod vào bảng Inventories
ALTER TABLE Inventories
ADD WarrantyPeriod INT;

ALTER TABLE InventoryWarranty
ADD AppointmentId INT;

-- Thiết lập khóa ngoại cho AppointmentId tham chiếu đến bảng Appointments
ALTER TABLE InventoryWarranty
ADD CONSTRAINT FK_InventoryWarranty_Appointment
FOREIGN KEY (AppointmentId) REFERENCES Appointments(AppointmentId);

ALTER TABLE Appointments
ADD EmployeeId INT;

ALTER TABLE Appointments
ADD CONSTRAINT FK_Appointments_Employee FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId);

CREATE TABLE Shift (
    ShiftId INT IDENTITY(1,1) PRIMARY KEY,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL
);

CREATE TABLE EmployeeShift (
    EmployeeShiftId INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT FOREIGN KEY REFERENCES Employees(EmployeeId),
    ShiftId INT FOREIGN KEY REFERENCES Shift(ShiftId),
    Month INT NOT NULL
);

