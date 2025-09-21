CREATE DATABASE CAR_EFULL;
GO

USE CAR_EFULL;
GO

-- =========================================
-- Tabla de Impuestos
-- =========================================
CREATE TABLE Impuestos (
    ImpuestoID INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(20) NOT NULL UNIQUE,       
    Nombre NVARCHAR(100) NOT NULL,             
    TasaDecimal DECIMAL(6,4) NOT NULL,         
    EsPorcentual BIT NOT NULL DEFAULT 1,       
    Activo BIT NOT NULL DEFAULT 1,             
    VigenteDesde DATE NOT NULL,               
    VigenteHasta DATE NULL                      
);

-- =========================================
-- Tabla de Propietarios
-- =========================================
CREATE TABLE Propietarios (
    ID_Propietario INT IDENTITY(1,1) PRIMARY KEY,
    Cedula NVARCHAR(20) NOT NULL UNIQUE,
    Nombre NVARCHAR(50) NOT NULL,
    Apellido NVARCHAR(50) NOT NULL,
    Telefono NVARCHAR(20) NULL DEFAULT '',
    Correo NVARCHAR(100) NULL,
    Direccion NVARCHAR(200) NULL,
    Estado NVARCHAR(10) NULL DEFAULT 'Activo',
    FechaRegistro DATETIME NULL DEFAULT GETDATE()
);

-- =========================================
-- Tabla de Tipos de Vehículo
-- =========================================
CREATE TABLE TipoVehiculo (
    TipoID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(30) NOT NULL UNIQUE
);

-- =========================================
-- Tabla de Marcas de Vehículo
-- =========================================
CREATE TABLE MarcaVehiculo (
    MarcaID INT IDENTITY(1,1) PRIMARY KEY,
    TipoID INT NOT NULL,
    Nombre NVARCHAR(50) NOT NULL,
    CONSTRAINT UQ_MarcaVehiculo UNIQUE (TipoID, Nombre),
    CONSTRAINT FK_MarcaVehiculo_TipoVehiculo FOREIGN KEY (TipoID)
        REFERENCES TipoVehiculo(TipoID)
);

-- =========================================
-- Tabla de Modelos de Vehículo
-- =========================================
CREATE TABLE ModeloVehiculo (
    ModeloID INT IDENTITY(1,1) PRIMARY KEY,
    MarcaID INT NOT NULL,
    Nombre NVARCHAR(80) NOT NULL,
    CONSTRAINT UQ_ModeloVehiculo UNIQUE (MarcaID, Nombre),
    CONSTRAINT FK_ModeloVehiculo_MarcaVehiculo FOREIGN KEY (MarcaID)
        REFERENCES MarcaVehiculo(MarcaID)
);

-- =========================================
-- Tabla de Modelos por Año
-- =========================================
CREATE TABLE ModeloAnio (
    ModeloAnioID INT IDENTITY(1,1) PRIMARY KEY,
    ModeloID INT NOT NULL,
    Anio INT NOT NULL,
    CONSTRAINT UQ_ModeloAnio UNIQUE (ModeloID, Anio),
    CONSTRAINT FK_ModeloAnio_ModeloVehiculo FOREIGN KEY (ModeloID)
        REFERENCES ModeloVehiculo(ModeloID)
);

-- =========================================
-- Tabla de Repuestos
-- =========================================
CREATE TABLE Repuestos (
    RepuestoID INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(30) NOT NULL UNIQUE,
    Nombre NVARCHAR(150) NOT NULL,
    Categoria NVARCHAR(100) NULL,
    Marca NVARCHAR(100) NULL,
    Modelo NVARCHAR(100) NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    ImpuestoID_Default INT NULL,
    Stock INT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Repuestos_ImpuestoDefault FOREIGN KEY (ImpuestoID_Default)
        REFERENCES Impuestos(ImpuestoID)
);

-- =========================================
-- Tabla de Usuarios
-- =========================================
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    LoginName NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Position NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    FotoPerfil VARBINARY(MAX) NULL,
    Telefono NVARCHAR(100) NOT NULL DEFAULT '',
    TelefonoSecundario VARCHAR(15) NULL
);
INSERT INTO Users
    (LoginName, Password, FirstName, LastName, Position, Email, FotoPerfil, Telefono, TelefonoSecundario)
VALUES
    ('Vodruk', '123gasc', 'Sánchez', '', 'Administrador', 'gascornejo885@gmail.com', NULL, '0967747273', NULL);

-- =========================================
-- Tabla de Empresa
-- =========================================
CREATE TABLE Empresa (
    EmpresaID INT IDENTITY(1,1) PRIMARY KEY,
    RazonSocial NVARCHAR(150) NULL,
    NombreComercial NVARCHAR(150) NULL,
    RUC NVARCHAR(20) NULL,
    Direccion NVARCHAR(200) NULL,
    Telefono NVARCHAR(30) NULL,
    ColorPrimarioHex NVARCHAR(9) NULL, 
    ColorSecundarioHex NVARCHAR(9) NULL,
    Logo VARBINARY(MAX) NULL,
    LogoMimeType NVARCHAR(50) NULL,
    LogoUpdatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

-- =========================================
-- Tabla de Vehículos
-- =========================================
CREATE TABLE Vehiculos (
    VehicleID       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Vehiculos PRIMARY KEY,
    Placa           NVARCHAR(10) NOT NULL,
    TipoID          INT NOT NULL,
    MarcaID         INT NOT NULL,
    ModeloID        INT NOT NULL,
    ModeloAnioID    INT NOT NULL,
    NumeroMotor     NVARCHAR(100) NOT NULL,
    NumeroChasis    NVARCHAR(100) NOT NULL,
    Color           NVARCHAR(30)  NOT NULL,
    Combustible     NVARCHAR(30)  NOT NULL,
    Kilometraje     INT           NOT NULL CONSTRAINT DF_Vehiculos_Km DEFAULT (0),
    Estado          NVARCHAR(10)  NOT NULL,
    ID_Propietario  INT           NOT NULL,
    CONSTRAINT UQ_Vehiculos_Placa UNIQUE (Placa),
    CONSTRAINT CK_Vehiculos_PlacaFormato CHECK (Placa LIKE '[A-Z][A-Z][A-Z]-[0-9][0-9][0-9][0-9]'),
    CONSTRAINT CK_Vehiculos_Estado CHECK (Estado IN (N'Activo', N'Inactivo')),
    CONSTRAINT FK_Vehiculos_Propietarios   FOREIGN KEY (ID_Propietario) REFERENCES dbo.Propietarios(ID_Propietario),
    CONSTRAINT FK_Vehiculos_TipoVehiculo   FOREIGN KEY (TipoID)        REFERENCES dbo.TipoVehiculo(TipoID),
    CONSTRAINT FK_Vehiculos_MarcaVehiculo  FOREIGN KEY (MarcaID)       REFERENCES dbo.MarcaVehiculo(MarcaID),
    CONSTRAINT FK_Vehiculos_ModeloVehiculo FOREIGN KEY (ModeloID)      REFERENCES dbo.ModeloVehiculo(ModeloID),
    CONSTRAINT FK_Vehiculos_ModeloAnio     FOREIGN KEY (ModeloAnioID)  REFERENCES dbo.ModeloAnio(ModeloAnioID)
);

-- =========================================
-- Tabla de Mantenimientos
-- =========================================
CREATE TABLE Mantenimientos (
    MantenimientoID INT IDENTITY(1,1) PRIMARY KEY,
    VehicleID INT NOT NULL,
    FechaMantenimiento DATE NOT NULL DEFAULT GETDATE(),
    Descripcion NVARCHAR(255) NULL,
    CostoTotal DECIMAL(12,2) NULL,
    CONSTRAINT FK_Mantenimientos_Vehiculos FOREIGN KEY (VehicleID)
        REFERENCES Vehiculos(VehicleID)
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

-- =========================================
-- Tabla de Facturas
-- =========================================
CREATE TABLE Facturas (
    FacturaID INT IDENTITY(1,1) PRIMARY KEY,
    CodigoFactura NVARCHAR(50) NOT NULL UNIQUE, 
    Fecha DATETIME NOT NULL DEFAULT GETDATE(), 
    ID_Propietario INT NOT NULL,               
    VehicleID INT NOT NULL,                     
    MetodoPago NVARCHAR(50) NOT NULL,           
    FormaPago NVARCHAR(50) NOT NULL,            
    Moneda NVARCHAR(10) NOT NULL DEFAULT 'USD', 
    Subtotal DECIMAL(12,2) NOT NULL,
    IVA DECIMAL(12,2) NOT NULL,
    Total DECIMAL(12,2) NOT NULL,
    Logo VARBINARY(MAX) NULL,
    FechaMantenimiento DATE NOT NULL,
    TipoServicio NVARCHAR(100) NULL,
    UserID INT NULL,                           
    Observaciones NVARCHAR(MAX) NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Facturas_Propietarios FOREIGN KEY (ID_Propietario)
        REFERENCES Propietarios(ID_Propietario),
    CONSTRAINT FK_Facturas_Vehiculos FOREIGN KEY (VehicleID)
        REFERENCES Vehiculos(VehicleID),
    CONSTRAINT FK_Facturas_Users FOREIGN KEY (UserID)
        REFERENCES Users(UserID)
);

-- =========================================
-- Tabla de Detalle de Facturas
-- =========================================
CREATE TABLE FacturaDetalle (
    FacturaDetalleID INT IDENTITY(1,1) PRIMARY KEY,
    FacturaID INT NOT NULL,
    RepuestoID INT NOT NULL,
    Cantidad DECIMAL(12,2) NOT NULL,
    PrecioUnitario DECIMAL(12,2) NOT NULL,
    ClaveUnidad NVARCHAR(50) NULL,
    Descripcion NVARCHAR(255) NULL,
    Subtotal AS (Cantidad * PrecioUnitario) PERSISTED,
    IVA DECIMAL(12,2) NULL,
    TotalLinea AS ((Cantidad * PrecioUnitario) + ISNULL(IVA,0)) PERSISTED,
    CONSTRAINT FK_FacturaDetalle_Facturas FOREIGN KEY (FacturaID)
        REFERENCES Facturas(FacturaID)
        ON DELETE CASCADE,
    CONSTRAINT FK_FacturaDetalle_Repuestos FOREIGN KEY (RepuestoID)
        REFERENCES Repuestos(RepuestoID)
);

------------------------------------------------------------

INSERT INTO TipoVehiculo (Nombre) VALUES (N'automovil');
INSERT INTO TipoVehiculo (Nombre) VALUES (N'camioneta');
INSERT INTO TipoVehiculo (Nombre) VALUES (N'motocicleta');


INSERT INTO MarcaVehiculo (TipoID, Nombre) VALUES (1, N'Toyota');
INSERT INTO MarcaVehiculo (TipoID, Nombre) VALUES (2, N'Chevrolet');
INSERT INTO MarcaVehiculo (TipoID, Nombre) VALUES (3, N'Yamaha');
INSERT INTO MarcaVehiculo (TipoID, Nombre) VALUES (3, N'Honda');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (1, N'Corolla');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (1, N'Hilux');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (1, N'RAV4');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (1, N'Yaris');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (1, N'Fortuner');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (2, N'Aveo');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (2, N'Onix');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (2, N'Sail');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (2, N'Spark GT');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (2, N'Tracker');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (3, N'FZ-S');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (3, N'MT-15');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (3, N'R15');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (3, N'XSR155');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (3, N'YBR125');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (4, N'CB125F');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (4, N'CB190R');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (4, N'Dio');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (4, N'Elite 125');
INSERT INTO ModeloVehiculo (MarcaID, Nombre) VALUES (4, N'XR150L');


INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (1, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (1, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (1, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (1, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (1, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (2, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (2, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (2, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (2, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (2, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (3, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (3, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (3, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (3, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (3, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (4, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (4, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (4, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (4, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (4, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (5, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (5, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (5, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (5, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (5, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (6, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (6, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (6, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (6, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (6, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (7, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (7, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (7, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (7, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (7, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (8, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (8, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (8, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (8, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (8, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (9, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (9, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (9, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (9, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (9, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (10, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (10, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (10, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (10, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (10, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (11, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (11, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (11, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (11, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (11, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (12, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (12, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (12, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (12, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (12, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (13, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (13, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (13, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (13, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (13, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (14, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (14, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (14, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (14, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (14, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (15, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (15, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (15, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (15, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (15, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (16, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (16, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (16, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (16, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (16, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (17, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (17, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (17, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (17, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (17, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (18, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (18, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (18, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (18, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (18, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (19, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (19, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (19, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (19, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (19, 2024);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (20, 2020);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (20, 2021);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (20, 2022);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (20, 2023);
INSERT INTO ModeloAnio (ModeloID, Anio) VALUES (20, 2024);


INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2001', N'Filtro de aceite', N'Motor', N'Toyota', N'Corolla', 19.00, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2002', N'Filtro de aire', N'Motor', N'Toyota', N'Corolla', 23.40, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2003', N'Pastillas de freno', N'Frenos', N'Toyota', N'Corolla', 35.50, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2004', N'Aceite 10W-40 1L', N'Lubricantes', N'Toyota', N'Corolla', 10.00, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2005', N'Bujía de encendido', N'Motor', N'Toyota', N'Corolla', 10.40, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2006', N'Filtro de aceite', N'Motor', N'Toyota', N'Hilux', 19.50, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2007', N'Filtro de aire', N'Motor', N'Toyota', N'Hilux', 23.90, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2008', N'Pastillas de freno', N'Frenos', N'Toyota', N'Hilux', 36.00, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2009', N'Aceite 10W-40 1L', N'Lubricantes', N'Toyota', N'Hilux', 10.50, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2010', N'Bujía de encendido', N'Motor', N'Toyota', N'Hilux', 10.90, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2011', N'Filtro de aceite', N'Motor', N'Toyota', N'RAV4', 20.00, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2012', N'Filtro de aire', N'Motor', N'Toyota', N'RAV4', 24.40, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2013', N'Pastillas de freno', N'Frenos', N'Toyota', N'RAV4', 36.50, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2014', N'Aceite 10W-40 1L', N'Lubricantes', N'Toyota', N'RAV4', 11.00, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2015', N'Bujía de encendido', N'Motor', N'Toyota', N'RAV4', 11.40, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2016', N'Filtro de aceite', N'Motor', N'Toyota', N'Yaris', 20.50, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2017', N'Filtro de aire', N'Motor', N'Toyota', N'Yaris', 24.90, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2018', N'Pastillas de freno', N'Frenos', N'Toyota', N'Yaris', 37.00, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2019', N'Aceite 10W-40 1L', N'Lubricantes', N'Toyota', N'Yaris', 11.50, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2020', N'Bujía de encendido', N'Motor', N'Toyota', N'Yaris', 11.90, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2021', N'Filtro de aceite', N'Motor', N'Toyota', N'Fortuner', 18.50, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2022', N'Filtro de aire', N'Motor', N'Toyota', N'Fortuner', 22.90, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2023', N'Pastillas de freno', N'Frenos', N'Toyota', N'Fortuner', 35.00, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2024', N'Aceite 10W-40 1L', N'Lubricantes', N'Toyota', N'Fortuner', 9.50, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2025', N'Bujía de encendido', N'Motor', N'Toyota', N'Fortuner', 9.90, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2026', N'Filtro de aceite', N'Motor', N'Chevrolet', N'Aveo', 19.00, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2027', N'Filtro de aire', N'Motor', N'Chevrolet', N'Aveo', 23.40, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2028', N'Pastillas de freno', N'Frenos', N'Chevrolet', N'Aveo', 35.50, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2029', N'Aceite 10W-40 1L', N'Lubricantes', N'Chevrolet', N'Aveo', 10.00, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2030', N'Bujía de encendido', N'Motor', N'Chevrolet', N'Aveo', 10.40, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2031', N'Filtro de aceite', N'Motor', N'Chevrolet', N'Onix', 19.50, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2032', N'Filtro de aire', N'Motor', N'Chevrolet', N'Onix', 23.90, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2033', N'Pastillas de freno', N'Frenos', N'Chevrolet', N'Onix', 36.00, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2034', N'Aceite 10W-40 1L', N'Lubricantes', N'Chevrolet', N'Onix', 10.50, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2035', N'Bujía de encendido', N'Motor', N'Chevrolet', N'Onix', 10.90, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2036', N'Filtro de aceite', N'Motor', N'Chevrolet', N'Sail', 20.00, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2037', N'Filtro de aire', N'Motor', N'Chevrolet', N'Sail', 24.40, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2038', N'Pastillas de freno', N'Frenos', N'Chevrolet', N'Sail', 36.50, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2039', N'Aceite 10W-40 1L', N'Lubricantes', N'Chevrolet', N'Sail', 11.00, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2040', N'Bujía de encendido', N'Motor', N'Chevrolet', N'Sail', 11.40, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2041', N'Filtro de aceite', N'Motor', N'Chevrolet', N'Spark GT', 20.50, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2042', N'Filtro de aire', N'Motor', N'Chevrolet', N'Spark GT', 24.90, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2043', N'Pastillas de freno', N'Frenos', N'Chevrolet', N'Spark GT', 37.00, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2044', N'Aceite 10W-40 1L', N'Lubricantes', N'Chevrolet', N'Spark GT', 11.50, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2045', N'Bujía de encendido', N'Motor', N'Chevrolet', N'Spark GT', 11.90, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2046', N'Filtro de aceite', N'Motor', N'Chevrolet', N'Tracker', 18.50, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2047', N'Filtro de aire', N'Motor', N'Chevrolet', N'Tracker', 22.90, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2048', N'Pastillas de freno', N'Frenos', N'Chevrolet', N'Tracker', 35.00, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2049', N'Aceite 10W-40 1L', N'Lubricantes', N'Chevrolet', N'Tracker', 9.50, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2050', N'Bujía de encendido', N'Motor', N'Chevrolet', N'Tracker', 9.90, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2051', N'Filtro de aceite', N'Motor', N'Yamaha', N'FZ-S', 19.00, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2052', N'Filtro de aire', N'Motor', N'Yamaha', N'FZ-S', 23.40, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2053', N'Pastillas de freno', N'Frenos', N'Yamaha', N'FZ-S', 35.50, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2054', N'Aceite 10W-40 1L', N'Lubricantes', N'Yamaha', N'FZ-S', 10.00, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2055', N'Bujía de encendido', N'Motor', N'Yamaha', N'FZ-S', 10.40, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2056', N'Filtro de aceite', N'Motor', N'Yamaha', N'MT-15', 19.50, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2057', N'Filtro de aire', N'Motor', N'Yamaha', N'MT-15', 23.90, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2058', N'Pastillas de freno', N'Frenos', N'Yamaha', N'MT-15', 36.00, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2059', N'Aceite 10W-40 1L', N'Lubricantes', N'Yamaha', N'MT-15', 10.50, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2060', N'Bujía de encendido', N'Motor', N'Yamaha', N'MT-15', 10.90, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2061', N'Filtro de aceite', N'Motor', N'Yamaha', N'R15', 20.00, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2062', N'Filtro de aire', N'Motor', N'Yamaha', N'R15', 24.40, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2063', N'Pastillas de freno', N'Frenos', N'Yamaha', N'R15', 36.50, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2064', N'Aceite 10W-40 1L', N'Lubricantes', N'Yamaha', N'R15', 11.00, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2065', N'Bujía de encendido', N'Motor', N'Yamaha', N'R15', 11.40, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2066', N'Filtro de aceite', N'Motor', N'Yamaha', N'XSR155', 20.50, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2067', N'Filtro de aire', N'Motor', N'Yamaha', N'XSR155', 24.90, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2068', N'Pastillas de freno', N'Frenos', N'Yamaha', N'XSR155', 37.00, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2069', N'Aceite 10W-40 1L', N'Lubricantes', N'Yamaha', N'XSR155', 11.50, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2070', N'Bujía de encendido', N'Motor', N'Yamaha', N'XSR155', 11.90, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2071', N'Filtro de aceite', N'Motor', N'Yamaha', N'YBR125', 18.50, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2072', N'Filtro de aire', N'Motor', N'Yamaha', N'YBR125', 22.90, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2073', N'Pastillas de freno', N'Frenos', N'Yamaha', N'YBR125', 35.00, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2074', N'Aceite 10W-40 1L', N'Lubricantes', N'Yamaha', N'YBR125', 9.50, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2075', N'Bujía de encendido', N'Motor', N'Yamaha', N'YBR125', 9.90, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2076', N'Filtro de aceite', N'Motor', N'Honda', N'CB125F', 19.00, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2077', N'Filtro de aire', N'Motor', N'Honda', N'CB125F', 23.40, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2078', N'Pastillas de freno', N'Frenos', N'Honda', N'CB125F', 35.50, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2079', N'Aceite 10W-40 1L', N'Lubricantes', N'Honda', N'CB125F', 10.00, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2080', N'Bujía de encendido', N'Motor', N'Honda', N'CB125F', 10.40, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2081', N'Filtro de aceite', N'Motor', N'Honda', N'CB190R', 19.50, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2082', N'Filtro de aire', N'Motor', N'Honda', N'CB190R', 23.90, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2083', N'Pastillas de freno', N'Frenos', N'Honda', N'CB190R', 36.00, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2084', N'Aceite 10W-40 1L', N'Lubricantes', N'Honda', N'CB190R', 10.50, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2085', N'Bujía de encendido', N'Motor', N'Honda', N'CB190R', 10.90, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2086', N'Filtro de aceite', N'Motor', N'Honda', N'Dio', 20.00, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2087', N'Filtro de aire', N'Motor', N'Honda', N'Dio', 24.40, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2088', N'Pastillas de freno', N'Frenos', N'Honda', N'Dio', 36.50, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2089', N'Aceite 10W-40 1L', N'Lubricantes', N'Honda', N'Dio', 11.00, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2090', N'Bujía de encendido', N'Motor', N'Honda', N'Dio', 11.40, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2091', N'Filtro de aceite', N'Motor', N'Honda', N'Elite 125', 20.50, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2092', N'Filtro de aire', N'Motor', N'Honda', N'Elite 125', 24.90, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2093', N'Pastillas de freno', N'Frenos', N'Honda', N'Elite 125', 37.00, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2094', N'Aceite 10W-40 1L', N'Lubricantes', N'Honda', N'Elite 125', 11.50, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2095', N'Bujía de encendido', N'Motor', N'Honda', N'Elite 125', 11.90, NULL, 200, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2096', N'Filtro de aceite', N'Motor', N'Honda', N'XR150L', 18.50, NULL, 120, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2097', N'Filtro de aire', N'Motor', N'Honda', N'XR150L', 22.90, NULL, 110, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2098', N'Pastillas de freno', N'Frenos', N'Honda', N'XR150L', 35.00, NULL, 90, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2099', N'Aceite 10W-40 1L', N'Lubricantes', N'Honda', N'XR150L', 9.50, NULL, 100, 1);
INSERT INTO Repuestos (Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, ImpuestoID_Default, Stock, Activo) VALUES ('RPT-2100', N'Bujía de encendido', N'Motor', N'Honda', N'XR150L', 9.90, NULL, 200, 1);