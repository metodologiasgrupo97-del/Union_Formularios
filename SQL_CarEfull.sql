-- =========================================
-- 1) CREACIÓN DE BASE DE DATOS
-- =========================================
IF DB_ID(N'CAR_EFULL') IS NULL
BEGIN
    CREATE DATABASE CAR_EFULL;
END
GO

USE CAR_EFULL;
GO

-- =========================================
-- 2) TABLAS BASE (sin dependencias o con mínimas)
-- =========================================

-- Impuestos
IF OBJECT_ID(N'dbo.Impuestos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Impuestos (
        ImpuestoID    INT IDENTITY(1,1) PRIMARY KEY,
        Codigo        NVARCHAR(20)  NOT NULL UNIQUE,
        Nombre        NVARCHAR(100) NOT NULL,
        TasaDecimal   DECIMAL(6,4)  NOT NULL,
        EsPorcentual  BIT NOT NULL DEFAULT 1,
        Activo        BIT NOT NULL DEFAULT 1,
        VigenteDesde  DATE NOT NULL,
        VigenteHasta  DATE NULL
    );
END
GO

-- Propietarios
IF OBJECT_ID(N'dbo.Propietarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Propietarios (
        ID_Propietario INT IDENTITY(1,1) PRIMARY KEY,
        Cedula         NVARCHAR(20)  NOT NULL UNIQUE,
        Nombre         NVARCHAR(50)  NOT NULL,
        Apellido       NVARCHAR(50)  NOT NULL,
        Telefono       NVARCHAR(20)  NULL DEFAULT '',
        Correo         NVARCHAR(100) NULL,
        Direccion      NVARCHAR(200) NULL,
        Estado         NVARCHAR(10)  NULL DEFAULT N'Activo',
        FechaRegistro  DATETIME      NULL DEFAULT GETDATE()
    );
END
GO

-- TipoVehiculo
IF OBJECT_ID(N'dbo.TipoVehiculo', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TipoVehiculo (
        TipoID INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(30) NOT NULL UNIQUE
    );
END
GO

-- MarcaVehiculo (depende de TipoVehiculo)
IF OBJECT_ID(N'dbo.MarcaVehiculo', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MarcaVehiculo (
        MarcaID INT IDENTITY(1,1) PRIMARY KEY,
        TipoID  INT NOT NULL,
        Nombre  NVARCHAR(50) NOT NULL,
        CONSTRAINT UQ_MarcaVehiculo UNIQUE (TipoID, Nombre),
        CONSTRAINT FK_MarcaVehiculo_TipoVehiculo
            FOREIGN KEY (TipoID) REFERENCES dbo.TipoVehiculo(TipoID)
    );
END
GO

-- ModeloVehiculo (depende de MarcaVehiculo)
IF OBJECT_ID(N'dbo.ModeloVehiculo', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ModeloVehiculo (
        ModeloID INT IDENTITY(1,1) PRIMARY KEY,
        MarcaID  INT NOT NULL,
        Nombre   NVARCHAR(80) NOT NULL,
        CONSTRAINT UQ_ModeloVehiculo UNIQUE (MarcaID, Nombre),
        CONSTRAINT FK_ModeloVehiculo_MarcaVehiculo
            FOREIGN KEY (MarcaID) REFERENCES dbo.MarcaVehiculo(MarcaID)
    );
END
GO

-- ModeloAnio (depende de ModeloVehiculo)
IF OBJECT_ID(N'dbo.ModeloAnio', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ModeloAnio (
        ModeloAnioID INT IDENTITY(1,1) PRIMARY KEY,
        ModeloID     INT NOT NULL,
        Anio         INT NOT NULL,
        CONSTRAINT UQ_ModeloAnio UNIQUE (ModeloID, Anio),
        CONSTRAINT FK_ModeloAnio_ModeloVehiculo
            FOREIGN KEY (ModeloID) REFERENCES dbo.ModeloVehiculo(ModeloID)
    );
END
GO

-- Repuestos (depende de Impuestos por ImpuestoID_Default)
IF OBJECT_ID(N'dbo.Repuestos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Repuestos (
        RepuestoID        INT IDENTITY(1,1) PRIMARY KEY,
        Codigo            NVARCHAR(30)  NOT NULL UNIQUE,
        Nombre            NVARCHAR(150) NOT NULL,
        Categoria         NVARCHAR(100) NULL,
        Marca             NVARCHAR(100) NULL,
        Modelo            NVARCHAR(100) NULL,
        PrecioUnitario    DECIMAL(10,2) NOT NULL,
        ImpuestoID_Default INT NULL,
        Stock             INT NULL,
        Activo            BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK_Repuestos_ImpuestoDefault
            FOREIGN KEY (ImpuestoID_Default) REFERENCES dbo.Impuestos(ImpuestoID)
    );
END
GO

-- Users
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        UserID            INT IDENTITY(1,1) PRIMARY KEY,
        LoginName         NVARCHAR(100) NOT NULL UNIQUE,
        Password          NVARCHAR(100) NOT NULL,
        FirstName         NVARCHAR(100) NOT NULL,
        LastName          NVARCHAR(100) NOT NULL,
        Position          NVARCHAR(100) NOT NULL,
        Email             NVARCHAR(100) NOT NULL,
        FotoPerfil        VARBINARY(MAX) NULL,
        Telefono          NVARCHAR(100) NOT NULL DEFAULT '',
        TelefonoSecundario VARCHAR(15)  NULL
    );
END
GO

-- Empresa
IF OBJECT_ID(N'dbo.Empresa', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Empresa (
        EmpresaID         INT IDENTITY(1,1) PRIMARY KEY,
        RazonSocial       NVARCHAR(150) NULL,
        NombreComercial   NVARCHAR(150) NULL,
        RUC               NVARCHAR(20)  NULL,
        Direccion         NVARCHAR(200) NULL,
        Telefono          NVARCHAR(30)  NULL,
        ColorPrimarioHex  NVARCHAR(9)   NULL,
        ColorSecundarioHex NVARCHAR(9)  NULL,
        Logo              VARBINARY(MAX) NULL,
        LogoMimeType      NVARCHAR(50)   NULL,
        LogoUpdatedAt     DATETIME2 NOT NULL DEFAULT SYSDATETIME()
    );
END
GO

-- =========================================
-- 3) TABLAS QUE DEPENDEN DE LAS ANTERIORES
-- =========================================

-- Vehiculos (depende de Propietarios, TipoVehiculo, MarcaVehiculo, ModeloVehiculo, ModeloAnio)
IF OBJECT_ID(N'dbo.Vehiculos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Vehiculos (
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
        CONSTRAINT FK_Vehiculos_TipoVehiculo   FOREIGN KEY (TipoID)         REFERENCES dbo.TipoVehiculo(TipoID),
        CONSTRAINT FK_Vehiculos_MarcaVehiculo  FOREIGN KEY (MarcaID)        REFERENCES dbo.MarcaVehiculo(MarcaID),
        CONSTRAINT FK_Vehiculos_ModeloVehiculo FOREIGN KEY (ModeloID)       REFERENCES dbo.ModeloVehiculo(ModeloID),
        CONSTRAINT FK_Vehiculos_ModeloAnio     FOREIGN KEY (ModeloAnioID)   REFERENCES dbo.ModeloAnio(ModeloAnioID)
    );
END
GO

-- Mantenimientos (depende de Vehiculos)
IF OBJECT_ID(N'dbo.Mantenimientos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Mantenimientos (
        MantenimientoID   INT IDENTITY(1,1) PRIMARY KEY,
        VehicleID         INT NOT NULL,
        FechaMantenimiento DATE NOT NULL DEFAULT GETDATE(),
        Descripcion       NVARCHAR(255) NULL,
        CostoTotal        DECIMAL(12,2) NULL,
        CONSTRAINT FK_Mantenimientos_Vehiculos
            FOREIGN KEY (VehicleID) REFERENCES dbo.Vehiculos(VehicleID)
            ON UPDATE CASCADE
            ON DELETE CASCADE
    );
END
GO

-- Mantenimiento_DetalleRepuesto (depende de Mantenimientos y Repuestos)
IF OBJECT_ID(N'dbo.Mantenimiento_DetalleRepuesto', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Mantenimiento_DetalleRepuesto (
        DetalleID      INT IDENTITY(1,1) PRIMARY KEY,
        MantenimientoID INT NOT NULL,
        RepuestoID     INT NOT NULL,
        Cantidad       DECIMAL(12,4) NOT NULL,
        PrecioUnitario DECIMAL(12,4) NOT NULL,
        ImpuestoCodigo NVARCHAR(20)  NOT NULL,
        ImpuestoNombre NVARCHAR(100) NOT NULL,
        ImpuestoTasa   DECIMAL(6,4)  NOT NULL,
        Subtotal       AS (Cantidad * PrecioUnitario) PERSISTED,
        MontoImpuesto  AS ((Cantidad * PrecioUnitario) * ImpuestoTasa) PERSISTED,
        TotalLinea     AS ((Cantidad * PrecioUnitario) + ((Cantidad * PrecioUnitario) * ImpuestoTasa)) PERSISTED,
        CONSTRAINT FK_Detalle_Mantenimientos
            FOREIGN KEY (MantenimientoID) REFERENCES dbo.Mantenimientos(MantenimientoID),
        CONSTRAINT FK_Detalle_Repuestos
            FOREIGN KEY (RepuestoID)      REFERENCES dbo.Repuestos(RepuestoID)
    );
END
GO

-- Facturas (depende de Propietarios, Vehiculos, Users)
IF OBJECT_ID(N'dbo.Facturas', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Facturas (
        FacturaID        INT IDENTITY(1,1) PRIMARY KEY,
        CodigoFactura    NVARCHAR(50)  NOT NULL UNIQUE,
        Fecha            DATETIME      NOT NULL DEFAULT GETDATE(),
        ID_Propietario   INT           NOT NULL,
        VehicleID        INT           NOT NULL,
        MetodoPago       NVARCHAR(50)  NOT NULL,
        FormaPago        NVARCHAR(50)  NOT NULL,
        Moneda           NVARCHAR(10)  NOT NULL DEFAULT 'USD',
        Subtotal         DECIMAL(12,2) NOT NULL,
        IVA              DECIMAL(12,2) NOT NULL,
        Total            DECIMAL(12,2) NOT NULL,
        Logo             VARBINARY(MAX) NULL,
        FechaMantenimiento DATE NOT NULL,
        TipoServicio     NVARCHAR(100) NULL,
        UserID           INT NULL,
        Observaciones    NVARCHAR(MAX) NULL,
        FechaRegistro    DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Facturas_Propietarios FOREIGN KEY (ID_Propietario) REFERENCES dbo.Propietarios(ID_Propietario),
        CONSTRAINT FK_Facturas_Vehiculos    FOREIGN KEY (VehicleID)      REFERENCES dbo.Vehiculos(VehicleID),
        CONSTRAINT FK_Facturas_Users        FOREIGN KEY (UserID)         REFERENCES dbo.Users(UserID)
    );
END
GO

-- FacturaDetalle (depende de Facturas y Repuestos)
IF OBJECT_ID(N'dbo.FacturaDetalle', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FacturaDetalle (
        FacturaDetalleID INT IDENTITY(1,1) PRIMARY KEY,
        FacturaID        INT NOT NULL,
        RepuestoID       INT NOT NULL,
        Cantidad         DECIMAL(12,2) NOT NULL,
        PrecioUnitario   DECIMAL(12,2) NOT NULL,
        ClaveUnidad      NVARCHAR(50)  NULL,
        Descripcion      NVARCHAR(255) NULL,
        Subtotal         AS (Cantidad * PrecioUnitario) PERSISTED,
        IVA              DECIMAL(12,2) NULL,
        TotalLinea       AS ((Cantidad * PrecioUnitario) + ISNULL(IVA,0)) PERSISTED,
        CONSTRAINT FK_FacturaDetalle_Facturas
            FOREIGN KEY (FacturaID)  REFERENCES dbo.Facturas(FacturaID) ON DELETE CASCADE,
        CONSTRAINT FK_FacturaDetalle_Repuestos
            FOREIGN KEY (RepuestoID) REFERENCES dbo.Repuestos(RepuestoID)
    );
END
GO

-- TrabajadorVehiculo (depende de Users y Vehiculos)
IF OBJECT_ID(N'dbo.TrabajadorVehiculo', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TrabajadorVehiculo (
        UserID         INT NOT NULL,
        VehicleID      INT NOT NULL,
        FechaAsignacion DATE NOT NULL,
        CONSTRAINT PK_TrabajadorVehiculo PRIMARY KEY (UserID, VehicleID),
        CONSTRAINT FK_TrabajadorVehiculo_Users
            FOREIGN KEY (UserID)    REFERENCES dbo.Users(UserID),
        CONSTRAINT FK_TrabajadorVehiculo_Vehiculos
            FOREIGN KEY (VehicleID) REFERENCES dbo.Vehiculos(VehicleID)
    );
END
GO

-- =========================================
-- 4) DATOS INICIALES
-- =========================================

-- Usuario inicial
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE LoginName = N'Vodruk')
BEGIN
    INSERT INTO dbo.Users
        (LoginName, Password, FirstName, LastName, Position, Email, FotoPerfil, Telefono, TelefonoSecundario)
    VALUES
        (N'Vodruk', N'123gasc', N'Sánchez', N'', N'Administrador', N'gascornejo885@gmail.com', NULL, N'0967747273', NULL);
END
GO