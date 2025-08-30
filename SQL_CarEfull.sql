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
-- Tabla de Vehículos
-- =========================================
CREATE TABLE Vehiculos (
    VehicleID INT IDENTITY(1,1) PRIMARY KEY,
    Placa NVARCHAR(10) NOT NULL UNIQUE,        
    Marca NVARCHAR(50) NULL,
    Modelo NVARCHAR(50) NULL,
    Anio INT NULL,
    NumeroMotor NVARCHAR(100) NULL,
    NumeroChasis NVARCHAR(100) NULL,
    Tipo NVARCHAR(50) NULL,
    Color NVARCHAR(30) NULL,
    Combustible NVARCHAR(30) NULL,
    Kilometraje INT NULL,
    Estado NVARCHAR(10) NULL,
    ID_Propietario INT NULL,
    CONSTRAINT FK_Vehiculos_Propietarios FOREIGN KEY (ID_Propietario)
    REFERENCES Propietarios(ID_Propietario)
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
-- Tabla de Repuestos
-- =========================================
CREATE TABLE Repuestos (
    RepuestoID INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(30) NOT NULL UNIQUE,
    Nombre NVARCHAR(150) NOT NULL,
    Categoria NVARCHAR(100) NULL,
    Marca NVARCHAR(100) NULL,
    Modelo NVARCHAR(100) NULL,
    PrecioUnitario DECIMAL(12,4) NOT NULL,
    ImpuestoID_Default INT NULL,
    Stock INT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Repuestos_ImpuestoDefault FOREIGN KEY (ImpuestoID_Default)
        REFERENCES Impuestos(ImpuestoID)
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
-- Tabla Detalle de Mantenimiento (Repuestos usados)
-- =========================================
CREATE TABLE Mantenimiento_DetalleRepuesto (
    DetalleID INT IDENTITY(1,1) PRIMARY KEY,
    MantenimientoID INT NOT NULL,
    RepuestoID INT NOT NULL,
    Cantidad DECIMAL(12,4) NOT NULL,
    PrecioUnitario DECIMAL(12,4) NOT NULL,
    ImpuestoCodigo NVARCHAR(20) NOT NULL,
    ImpuestoNombre NVARCHAR(100) NOT NULL,
    ImpuestoTasa DECIMAL(6,4) NOT NULL,
    Subtotal AS (Cantidad * PrecioUnitario) PERSISTED,
    MontoImpuesto AS ((Cantidad * PrecioUnitario) * ImpuestoTasa) PERSISTED,
    TotalLinea AS ((Cantidad * PrecioUnitario) + ((Cantidad * PrecioUnitario) * ImpuestoTasa)) PERSISTED,
    CONSTRAINT FK_Detalle_Mantenimientos FOREIGN KEY (MantenimientoID)
        REFERENCES Mantenimientos(MantenimientoID),
    CONSTRAINT FK_Detalle_Repuestos FOREIGN KEY (RepuestoID)
        REFERENCES Repuestos(RepuestoID)
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

-- =========================================
-- Tabla TrabajadorVehiculo (Asignación de usuarios a vehículos)
-- =========================================
CREATE TABLE TrabajadorVehiculo (
    UserID INT NOT NULL,
    VehicleID INT NOT NULL,
    FechaAsignacion DATE NOT NULL,
    PRIMARY KEY (UserID, VehicleID),
    CONSTRAINT FK_TrabajadorVehiculo_Users FOREIGN KEY (UserID)
        REFERENCES Users(UserID),
    CONSTRAINT FK_TrabajadorVehiculo_Vehiculos FOREIGN KEY (VehicleID)
        REFERENCES Vehiculos(VehicleID)
);

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

    -- Claves foráneas
    CONSTRAINT FK_Facturas_Propietarios FOREIGN KEY (ID_Propietario)
        REFERENCES Propietarios(ID_Propietario),
    CONSTRAINT FK_Facturas_Vehiculos FOREIGN KEY (VehicleID)
        REFERENCES Vehiculos(VehicleID),
    CONSTRAINT FK_Facturas_Users FOREIGN KEY (UserID)
        REFERENCES Users(UserID)
);


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

CREATE TABLE Empresa (
    EmpresaID INT IDENTITY(1,1) PRIMARY KEY,
    RazonSocial        NVARCHAR(150) NULL,
    NombreComercial    NVARCHAR(150) NULL,
    RUC                NVARCHAR(20)  NULL,
    Direccion          NVARCHAR(200) NULL,
    Telefono           NVARCHAR(30)  NULL,
    ColorPrimarioHex   NVARCHAR(9)   NULL, 
    ColorSecundarioHex NVARCHAR(9)   NULL,
    Logo               VARBINARY(MAX) NULL,
    LogoMimeType       NVARCHAR(50)   NULL,
    LogoUpdatedAt      DATETIME2 NOT NULL DEFAULT SYSDATETIME()
  );

SELECT * FROM Empresa
SELECT * FROM Facturas
SELECT * FROM Propietarios
SELECT * FROM Vehiculos
