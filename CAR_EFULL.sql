-- ============================
-- CREAR BD (solo si no existe)
-- ============================
IF DB_ID(N'CAR_EFULL') IS NULL
BEGIN
    CREATE DATABASE CAR_EFULL;
END
GO
USE CAR_EFULL;
GO

-- ============================
-- TABLAS BASE
-- ============================

-- Tipos de vehículo
IF OBJECT_ID('dbo.TipoVehiculo') IS NULL
CREATE TABLE dbo.TipoVehiculo(
    TipoID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(30) NOT NULL UNIQUE
);
GO

-- Marcas (depende de TipoVehiculo)
IF OBJECT_ID('dbo.MarcaVehiculo') IS NULL
CREATE TABLE dbo.MarcaVehiculo(
    MarcaID INT IDENTITY(1,1) PRIMARY KEY,
    TipoID INT NOT NULL,
    Nombre NVARCHAR(50) NOT NULL,
    Estado NVARCHAR(10) NOT NULL DEFAULT 'Activo',
    CONSTRAINT UQ_MarcaVehiculo UNIQUE(TipoID, Nombre),
    CONSTRAINT FK_MarcaVehiculo_TipoVehiculo
        FOREIGN KEY (TipoID) REFERENCES dbo.TipoVehiculo(TipoID)
);
GO

-- Modelos (depende de MarcaVehiculo)
IF OBJECT_ID('dbo.ModeloVehiculo') IS NULL
CREATE TABLE dbo.ModeloVehiculo(
    ModeloID INT IDENTITY(1,1) PRIMARY KEY,
    MarcaID INT NOT NULL,
    Nombre NVARCHAR(80) NOT NULL,
    Estado NVARCHAR(10) NOT NULL DEFAULT 'Activo',
    CONSTRAINT UQ_ModeloVehiculo UNIQUE(MarcaID, Nombre),
    CONSTRAINT FK_ModeloVehiculo_MarcaVehiculo
        FOREIGN KEY (MarcaID) REFERENCES dbo.MarcaVehiculo(MarcaID)
);
GO

-- Años de modelo (depende de ModeloVehiculo)
IF OBJECT_ID('dbo.ModeloAnio') IS NULL
CREATE TABLE dbo.ModeloAnio(
    ModeloAnioID INT IDENTITY(1,1) PRIMARY KEY,
    ModeloID INT NOT NULL,
    Anio INT NOT NULL,
    CONSTRAINT UQ_ModeloAnio UNIQUE(ModeloID, Anio),
    CONSTRAINT FK_ModeloAnio_ModeloVehiculo
        FOREIGN KEY (ModeloID) REFERENCES dbo.ModeloVehiculo(ModeloID)
);
GO

-- Propietarios
IF OBJECT_ID('dbo.Propietarios') IS NULL
CREATE TABLE dbo.Propietarios(
    ID_Propietario INT IDENTITY(1,1) PRIMARY KEY,
    Cedula NVARCHAR(20) NOT NULL UNIQUE,
    Nombre NVARCHAR(50) NOT NULL,
    Apellido NVARCHAR(50) NOT NULL,
    Telefono NVARCHAR(20) DEFAULT '',
    Correo NVARCHAR(100),
    Direccion NVARCHAR(200),
    Estado NVARCHAR(10) DEFAULT 'Activo',
    FechaRegistro DATETIME DEFAULT GETDATE()
);
GO

-- Usuarios
IF OBJECT_ID('dbo.Users') IS NULL
CREATE TABLE dbo.Users(
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    LoginName NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Position NVARCHAR(100) NOT NULL,  -- 'Administrador' | 'Trabajador'
    Email NVARCHAR(100) NOT NULL,
    FotoPerfil VARBINARY(MAX),
    Telefono NVARCHAR(100) DEFAULT '',
    TelefonoSecundario VARCHAR(15),
    Estado NVARCHAR(20) DEFAULT 'Activo'
);
GO

-- Impuestos
IF OBJECT_ID('dbo.Impuestos') IS NULL
CREATE TABLE dbo.Impuestos(
    ImpuestoID INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(20) NOT NULL UNIQUE,
    Nombre NVARCHAR(100) NOT NULL,
    TasaDecimal DECIMAL(6,4) NOT NULL CHECK(TasaDecimal>=0),
    EsPorcentual BIT NOT NULL DEFAULT 1,
    Activo BIT NOT NULL DEFAULT 1,
    VigenteDesde DATE NOT NULL,
    VigenteHasta DATE NULL,
    CONSTRAINT CK_Impuestos_Porc CHECK((EsPorcentual=1 AND TasaDecimal BETWEEN 0 AND 1) OR EsPorcentual=0),
    CONSTRAINT CK_Impuestos_Vigencia CHECK(VigenteHasta IS NULL OR VigenteHasta >= VigenteDesde)
);
GO

-- Repuestos (depende de TipoVehiculo, MarcaVehiculo, ModeloVehiculo, Impuestos)
IF OBJECT_ID('dbo.Repuestos') IS NULL
CREATE TABLE dbo.Repuestos(
    RepuestoID INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(30) NOT NULL UNIQUE CHECK(Codigo LIKE 'RPT-[0-9][0-9][0-9]%'),
    Nombre NVARCHAR(150) NOT NULL,
    Categoria NVARCHAR(100),
    TipoRepuesto VARCHAR(100) NOT NULL CHECK(TipoRepuesto IN('Repuestos genéricos','Repuestos originales')),
    -- Columnas espejo para compatibilidad UI (nombre de texto)
    Marca NVARCHAR(100),
    Modelo NVARCHAR(100),
    -- FKs
    TipoID INT NULL,
    MarcaID INT NULL,
    ModeloID INT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL CHECK(PrecioUnitario>=0),
    ImpuestoID_Default INT NULL,
    Stock INT NULL CHECK(Stock IS NULL OR Stock>=0),
    Activo BIT DEFAULT 1,
    CONSTRAINT FK_Repuestos_TipoVehiculo
        FOREIGN KEY (TipoID) REFERENCES dbo.TipoVehiculo(TipoID),
    CONSTRAINT FK_Repuestos_MarcaVehiculo
        FOREIGN KEY (MarcaID) REFERENCES dbo.MarcaVehiculo(MarcaID),
    CONSTRAINT FK_Repuestos_ModeloVehiculo
        FOREIGN KEY (ModeloID) REFERENCES dbo.ModeloVehiculo(ModeloID),
    CONSTRAINT FK_Repuestos_ImpuestoDefault
        FOREIGN KEY (ImpuestoID_Default) REFERENCES dbo.Impuestos(ImpuestoID)
);
GO

-- Categorías de repuesto (para mapear a servicios)
IF OBJECT_ID('dbo.CategoriaRepuesto') IS NULL
CREATE TABLE dbo.CategoriaRepuesto(
    CategoriaID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL UNIQUE,
    Activo BIT DEFAULT 1
);
GO

-- Empresa
IF OBJECT_ID('dbo.Empresa') IS NULL
CREATE TABLE dbo.Empresa(
    EmpresaID INT IDENTITY(1,1) PRIMARY KEY,
    RazonSocial NVARCHAR(150),
    NombreComercial NVARCHAR(150),
    RUC NVARCHAR(13),
    Direccion NVARCHAR(200),
    Correo NVARCHAR(100),
    Telefono NVARCHAR(10),
    ColorPrimarioHex NVARCHAR(9),
    ColorSecundarioHex NVARCHAR(9),
    Logo VARBINARY(MAX),
    LogoMimeType NVARCHAR(50),
    LogoUpdatedAt DATETIME2 DEFAULT SYSDATETIME()
);
GO

-- Tipos de servicio
IF OBJECT_ID('dbo.TipoServicio') IS NULL
CREATE TABLE dbo.TipoServicio(
    TipoServicioID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL UNIQUE,
    Descripcion NVARCHAR(255),
    PrecioServicio DECIMAL(10,2) DEFAULT 0,
    Activo BIT DEFAULT 1
);
GO

-- Relación Servicio - CategoríaRepuesto
IF OBJECT_ID('dbo.ServicioCategoriaRepuesto') IS NULL
CREATE TABLE dbo.ServicioCategoriaRepuesto(
    ServicioCategoriaID INT IDENTITY(1,1) PRIMARY KEY,
    TipoServicioID INT NOT NULL,
    CategoriaID INT NOT NULL,
    CONSTRAINT UQ_ServicioCategoria UNIQUE(TipoServicioID, CategoriaID),
    CONSTRAINT FK_ServCat_TipoServicio
        FOREIGN KEY (TipoServicioID) REFERENCES dbo.TipoServicio(TipoServicioID),
    CONSTRAINT FK_ServCat_Categoria
        FOREIGN KEY (CategoriaID) REFERENCES dbo.CategoriaRepuesto(CategoriaID)
);
GO

-- Vehículos (depende de Tipo/Marca/Modelo/ModeloAnio, Propietarios)
IF OBJECT_ID('dbo.Vehiculos') IS NULL
CREATE TABLE dbo.Vehiculos(
    VehicleID INT IDENTITY(1,1) PRIMARY KEY,
    Placa NVARCHAR(10) NOT NULL UNIQUE,
    TipoID INT NOT NULL,
    MarcaID INT NOT NULL,
    ModeloID INT NOT NULL,
    ModeloAnioID INT NOT NULL,
    NumeroMotor NVARCHAR(100) NOT NULL,
    NumeroChasis NVARCHAR(100) NOT NULL,
    Color NVARCHAR(30) NOT NULL,
    Combustible NVARCHAR(30) NOT NULL,
    Kilometraje INT DEFAULT 0,
    Estado NVARCHAR(10) NOT NULL CHECK(Estado IN('Activo','Inactivo')),
    ID_Propietario INT NOT NULL,
    CONSTRAINT CK_PlacaFormato CHECK(Placa LIKE '[A-Z][A-Z][A-Z]-[0-9][0-9][0-9][0-9]'),
    CONSTRAINT FK_Veh_Tipo    FOREIGN KEY (TipoID) REFERENCES dbo.TipoVehiculo(TipoID),
    CONSTRAINT FK_Veh_Marca   FOREIGN KEY (MarcaID) REFERENCES dbo.MarcaVehiculo(MarcaID),
    CONSTRAINT FK_Veh_Modelo  FOREIGN KEY (ModeloID) REFERENCES dbo.ModeloVehiculo(ModeloID),
    CONSTRAINT FK_Veh_Anio    FOREIGN KEY (ModeloAnioID) REFERENCES dbo.ModeloAnio(ModeloAnioID),
    CONSTRAINT FK_Veh_Prop    FOREIGN KEY (ID_Propietario) REFERENCES dbo.Propietarios(ID_Propietario)
);
GO

-- Mantenimientos (depende de Vehículos)
IF OBJECT_ID('dbo.Mantenimientos') IS NULL
CREATE TABLE dbo.Mantenimientos(
    MantenimientoID INT IDENTITY(1,1) PRIMARY KEY,
    VehicleID INT NOT NULL,
    FechaMantenimiento DATE DEFAULT GETDATE(),
    Descripcion NVARCHAR(255),
    CostoTotal DECIMAL(12,2),
    CONSTRAINT FK_Mant_Veh FOREIGN KEY (VehicleID) REFERENCES dbo.Vehiculos(VehicleID) ON DELETE CASCADE ON UPDATE CASCADE
);
GO

-- Facturas (depende de Propietarios, Vehículos, TipoServicio, Users)
IF OBJECT_ID('dbo.Facturas') IS NULL
CREATE TABLE dbo.Facturas(
    FacturaID INT IDENTITY(1,1) PRIMARY KEY,
    CodigoFactura NVARCHAR(50) NOT NULL UNIQUE,
    Fecha DATETIME DEFAULT GETDATE(),
    ID_Propietario INT NOT NULL,
    VehicleID INT NOT NULL,
    MetodoPago NVARCHAR(50) NOT NULL,
    FormaPago NVARCHAR(50) NOT NULL,
    Moneda NVARCHAR(10) DEFAULT 'USD',
    Subtotal DECIMAL(12,2) NOT NULL,
    IVA DECIMAL(12,2) NOT NULL,
    Total DECIMAL(12,2) NOT NULL,
    Logo VARBINARY(MAX),
    FechaMantenimiento DATE NOT NULL,
    TipoServicioID INT NOT NULL,
    UserID INT NULL,
    Observaciones NVARCHAR(MAX),
    FechaRegistro DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Fac_Prop  FOREIGN KEY (ID_Propietario) REFERENCES dbo.Propietarios(ID_Propietario),
    CONSTRAINT FK_Fac_Veh   FOREIGN KEY (VehicleID) REFERENCES dbo.Vehiculos(VehicleID),
    CONSTRAINT FK_Fac_TServ FOREIGN KEY (TipoServicioID) REFERENCES dbo.TipoServicio(TipoServicioID),
    CONSTRAINT FK_Fac_User  FOREIGN KEY (UserID) REFERENCES dbo.Users(UserID)
);
GO

-- Detalle de factura (depende de Facturas y Repuestos)
IF OBJECT_ID('dbo.FacturaDetalle') IS NULL
CREATE TABLE dbo.FacturaDetalle(
    FacturaDetalleID INT IDENTITY(1,1) PRIMARY KEY,
    FacturaID INT NOT NULL,
    RepuestoID INT NOT NULL,
    Cantidad DECIMAL(12,2) NOT NULL CHECK(Cantidad>=0),
    PrecioUnitario DECIMAL(12,2) NOT NULL CHECK(PrecioUnitario>=0),
    ClaveUnidad NVARCHAR(50),
    Descripcion NVARCHAR(255),
    Subtotal AS (Cantidad*PrecioUnitario) PERSISTED,
    IVA DECIMAL(12,2) CHECK(IVA IS NULL OR IVA>=0),
    TotalLinea AS (Cantidad*PrecioUnitario+ISNULL(IVA,0)) PERSISTED,
    CONSTRAINT FK_FDet_Fac FOREIGN KEY (FacturaID) REFERENCES dbo.Facturas(FacturaID) ON DELETE CASCADE,
    CONSTRAINT FK_FDet_Rep FOREIGN KEY (RepuestoID) REFERENCES dbo.Repuestos(RepuestoID)
);
GO

-- ============================
-- VISTA para RepuestosDAO
-- ============================
CREATE OR ALTER VIEW dbo.vw_RepuestosFull
AS
SELECT
    r.RepuestoID,
    r.Codigo,
    r.Nombre,
    r.Categoria,
    r.TipoRepuesto,
    tv.Nombre AS TipoVehiculo,
    COALESCE(mv.Nombre, r.Marca)  AS Marca,
    COALESCE(mo.Nombre, r.Modelo) AS Modelo,
    r.PrecioUnitario,
    r.Stock,
    r.Activo,
    r.TipoID,
    r.MarcaID,
    r.ModeloID
FROM dbo.Repuestos r
LEFT JOIN dbo.TipoVehiculo  tv ON tv.TipoID  = r.TipoID
LEFT JOIN dbo.MarcaVehiculo mv ON mv.MarcaID = r.MarcaID
LEFT JOIN dbo.ModeloVehiculo mo ON mo.ModeloID = r.ModeloID;
GO

-- ============================
-- ÍNDICES RECOMENDADOS
-- ============================
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Repuestos_TipoID' AND object_id = OBJECT_ID('dbo.Repuestos'))
    CREATE INDEX IX_Repuestos_TipoID  ON dbo.Repuestos(TipoID);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Repuestos_MarcaID' AND object_id = OBJECT_ID('dbo.Repuestos'))
    CREATE INDEX IX_Repuestos_MarcaID ON dbo.Repuestos(MarcaID);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Repuestos_ModeloID' AND object_id = OBJECT_ID('dbo.Repuestos'))
    CREATE INDEX IX_Repuestos_ModeloID ON dbo.Repuestos(ModeloID);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Repuestos_Categoria' AND object_id = OBJECT_ID('dbo.Repuestos'))
    CREATE INDEX IX_Repuestos_Categoria ON dbo.Repuestos(Categoria);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Repuestos_TipoRepuesto' AND object_id = OBJECT_ID('dbo.Repuestos'))
    CREATE INDEX IX_Repuestos_TipoRepuesto ON dbo.Repuestos(TipoRepuesto);
GO

-- ============================
-- DATOS INICIALES
-- ============================

-- Tipos de vehículo fijos
IF NOT EXISTS (SELECT 1 FROM dbo.TipoVehiculo WHERE Nombre = N'Automóvil')
    INSERT INTO dbo.TipoVehiculo (Nombre) VALUES (N'Automóvil');
IF NOT EXISTS (SELECT 1 FROM dbo.TipoVehiculo WHERE Nombre = N'Camioneta')
    INSERT INTO dbo.TipoVehiculo (Nombre) VALUES (N'Camioneta');
IF NOT EXISTS (SELECT 1 FROM dbo.TipoVehiculo WHERE Nombre = N'Motocicleta')
    INSERT INTO dbo.TipoVehiculo (Nombre) VALUES (N'Motocicleta');

DECLARE @TipoAuto INT = (SELECT TipoID FROM dbo.TipoVehiculo WHERE Nombre = N'Automóvil');

-- Marcas base (Automóvil)
IF NOT EXISTS (SELECT 1 FROM dbo.MarcaVehiculo WHERE TipoID=@TipoAuto AND Nombre=N'Chevrolet')
    INSERT INTO dbo.MarcaVehiculo (TipoID, Nombre, Estado) VALUES (@TipoAuto, N'Chevrolet', N'Activo');
IF NOT EXISTS (SELECT 1 FROM dbo.MarcaVehiculo WHERE TipoID=@TipoAuto AND Nombre=N'Toyota')
    INSERT INTO dbo.MarcaVehiculo (TipoID, Nombre, Estado) VALUES (@TipoAuto, N'Toyota', N'Activo');

DECLARE @MarcaChevrolet INT = (SELECT MarcaID FROM dbo.MarcaVehiculo WHERE TipoID=@TipoAuto AND Nombre=N'Chevrolet');
DECLARE @MarcaToyota    INT = (SELECT MarcaID FROM dbo.MarcaVehiculo WHERE TipoID=@TipoAuto AND Nombre=N'Toyota');

-- Modelos base
IF NOT EXISTS (SELECT 1 FROM dbo.ModeloVehiculo WHERE MarcaID=@MarcaChevrolet AND Nombre=N'Aveo')
    INSERT INTO dbo.ModeloVehiculo (MarcaID, Nombre, Estado) VALUES (@MarcaChevrolet, N'Aveo', N'Activo');
IF NOT EXISTS (SELECT 1 FROM dbo.ModeloVehiculo WHERE MarcaID=@MarcaChevrolet AND Nombre=N'Spark')
    INSERT INTO dbo.ModeloVehiculo (MarcaID, Nombre, Estado) VALUES (@MarcaChevrolet, N'Spark', N'Activo');
IF NOT EXISTS (SELECT 1 FROM dbo.ModeloVehiculo WHERE MarcaID=@MarcaToyota AND Nombre=N'Corolla')
    INSERT INTO dbo.ModeloVehiculo (MarcaID, Nombre, Estado) VALUES (@MarcaToyota, N'Corolla', N'Activo');
IF NOT EXISTS (SELECT 1 FROM dbo.ModeloVehiculo WHERE MarcaID=@MarcaToyota AND Nombre=N'Hilux')
    INSERT INTO dbo.ModeloVehiculo (MarcaID, Nombre, Estado) VALUES (@MarcaToyota, N'Hilux', N'Activo');

DECLARE @ModeloAveo    INT = (SELECT ModeloID FROM dbo.ModeloVehiculo WHERE MarcaID=@MarcaChevrolet AND Nombre=N'Aveo');
DECLARE @ModeloSpark   INT = (SELECT ModeloID FROM dbo.ModeloVehiculo WHERE MarcaID=@MarcaChevrolet AND Nombre=N'Spark');
DECLARE @ModeloCorolla INT = (SELECT ModeloID FROM dbo.ModeloVehiculo WHERE MarcaID=@MarcaToyota AND Nombre=N'Corolla');
DECLARE @ModeloHilux   INT = (SELECT ModeloID FROM dbo.ModeloVehiculo WHERE MarcaID=@MarcaToyota AND Nombre=N'Hilux');

-- Años 2000-2025 para los modelos base
;WITH A(anio) AS (
    SELECT 2000 UNION ALL SELECT 2001 UNION ALL SELECT 2002 UNION ALL SELECT 2003 UNION ALL SELECT 2004 UNION ALL
    SELECT 2005 UNION ALL SELECT 2006 UNION ALL SELECT 2007 UNION ALL SELECT 2008 UNION ALL SELECT 2009 UNION ALL
    SELECT 2010 UNION ALL SELECT 2011 UNION ALL SELECT 2012 UNION ALL SELECT 2013 UNION ALL SELECT 2014 UNION ALL
    SELECT 2015 UNION ALL SELECT 2016 UNION ALL SELECT 2017 UNION ALL SELECT 2018 UNION ALL SELECT 2019 UNION ALL
    SELECT 2020 UNION ALL SELECT 2021 UNION ALL SELECT 2022 UNION ALL SELECT 2023 UNION ALL SELECT 2024 UNION ALL
    SELECT 2025
)
INSERT INTO dbo.ModeloAnio (ModeloID, Anio)
SELECT M.ModeloID, A.anio
FROM (VALUES (@ModeloAveo),(@ModeloSpark),(@ModeloCorolla),(@ModeloHilux)) AS M(ModeloID)
CROSS JOIN A
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.ModeloAnio MA WHERE MA.ModeloID = M.ModeloID AND MA.Anio = A.anio
);

-- Impuestos base (histórico 12%, vigente 15%)
IF NOT EXISTS (SELECT 1 FROM dbo.Impuestos WHERE Codigo=N'IVA12')
    INSERT INTO dbo.Impuestos (Codigo, Nombre, TasaDecimal, EsPorcentual, Activo, VigenteDesde, VigenteHasta)
    VALUES (N'IVA12', N'IVA Ecuador 12%', 0.12, 1, 0, '2016-06-01', '2023-12-31');

IF NOT EXISTS (SELECT 1 FROM dbo.Impuestos WHERE Codigo=N'IVA15')
    INSERT INTO dbo.Impuestos (Codigo, Nombre, TasaDecimal, EsPorcentual, Activo, VigenteDesde, VigenteHasta)
    VALUES (N'IVA15', N'IVA Ecuador 15%', 0.15, 1, 1, '2024-01-01', NULL);

DECLARE @IVA15 INT = (SELECT ImpuestoID FROM dbo.Impuestos WHERE Codigo=N'IVA15');

-- Repuestos genéricos (solo por TipoVehiculo = Automóvil)
IF NOT EXISTS (SELECT 1 FROM dbo.Repuestos WHERE Codigo=N'RPT-201')
    INSERT INTO dbo.Repuestos (Codigo, Nombre, Categoria, TipoRepuesto, TipoID, PrecioUnitario, ImpuestoID_Default, Stock, Activo)
    VALUES (N'RPT-201', N'Filtro de aceite genérico', N'Motor', 'Repuestos genéricos', @TipoAuto, 10.50, @IVA15, 50, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Repuestos WHERE Codigo=N'RPT-202')
    INSERT INTO dbo.Repuestos (Codigo, Nombre, Categoria, TipoRepuesto, TipoID, PrecioUnitario, ImpuestoID_Default, Stock, Activo)
    VALUES (N'RPT-202', N'Filtro de aire genérico', N'Motor', 'Repuestos genéricos', @TipoAuto, 12.00, @IVA15, 40, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Repuestos WHERE Codigo=N'RPT-203')
    INSERT INTO dbo.Repuestos (Codigo, Nombre, Categoria, TipoRepuesto, TipoID, PrecioUnitario, ImpuestoID_Default, Stock, Activo)
    VALUES (N'RPT-203', N'Bujía estándar genérica', N'Motor', 'Repuestos genéricos', @TipoAuto, 8.00, @IVA15, 100, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Repuestos WHERE Codigo=N'RPT-204')
    INSERT INTO dbo.Repuestos (Codigo, Nombre, Categoria, TipoRepuesto, TipoID, PrecioUnitario, ImpuestoID_Default, Stock, Activo)
    VALUES (N'RPT-204', N'Pastillas de freno genéricas', N'Sistema de frenos', 'Repuestos genéricos', @TipoAuto, 25.00, @IVA15, 30, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Repuestos WHERE Codigo=N'RPT-205')
    INSERT INTO dbo.Repuestos (Codigo, Nombre, Categoria, TipoRepuesto, TipoID, PrecioUnitario, ImpuestoID_Default, Stock, Activo)
    VALUES (N'RPT-205', N'Llanta estándar genérica', N'Neumáticos y llantas', 'Repuestos genéricos', @TipoAuto, 80.00, @IVA15, 20, 1);

-- Usuario administrador por defecto
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE LoginName=N'admin')
    INSERT INTO dbo.Users (LoginName, Password, FirstName, LastName, Position, Email, FotoPerfil, Telefono, TelefonoSecundario)
    VALUES (N'admin', N'123admin', N'admin', N'', N'Administrador', N'correounico@gmail.com', NULL, N'', NULL);
GO

INSERT INTO TipoServicio (Nombre, Descripcion, PrecioServicio, Activo) VALUES
(N'Cambio de aceite', N'Reemplazo de aceite y filtro', 25.00, 1),
(N'Mantenimiento preventivo', N'Revisión general y consumibles', 50.00, 1),
(N'Sistema de frenos', N'Pastillas, discos, líquido', 40.00, 1),
(N'Suspensión y dirección', N'Amortiguadores, bujes', 45.00, 1),
(N'Sistema eléctrico', N'Batería, alternador, luces', 30.00, 1),
(N'Sistema de enfriamiento', N'Radiador, bomba de agua', 35.00, 1),
(N'Afinación de motor', N'Bujías, filtros, correas', 60.00, 1),
(N'Transmisión y embrague', N'Aceite caja, kit clutch', 70.00, 1),
(N'Ruedas y neumáticos', N'Llantas y servicios', 20.00, 1),
(N'Sistema de escape', N'Silenciador, catalizador', 25.00, 1),
(N'Diagnóstico y escáner', N'Lectura de códigos', 15.00, 1),
(N'Limpieza y visibilidad', N'Escobillas, fluidos', 10.00, 1);
