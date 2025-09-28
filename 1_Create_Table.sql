USE QuanLyQuanNet;
GO

-- Drop bảng có ràng buộc FK trước
IF OBJECT_ID('dbo.BaoTri', 'U') IS NOT NULL
    DROP TABLE dbo.BaoTri;
GO

IF OBJECT_ID('dbo.ThietBi', 'U') IS NOT NULL
    DROP TABLE dbo.ThietBi;
GO

IF OBJECT_ID('dbo.MayTinh', 'U') IS NOT NULL
    DROP TABLE dbo.MayTinh;
GO

IF OBJECT_ID('dbo.LoaiThietBi', 'U') IS NOT NULL
    DROP TABLE dbo.LoaiThietBi;
GO

IF OBJECT_ID('dbo.NguoiDung', 'U') IS NOT NULL
    DROP TABLE dbo.NguoiDung;
GO

IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL
    DROP TABLE dbo.Roles;
GO

IF OBJECT_ID('dbo.AuditLog', 'U') IS NOT NULL
    DROP TABLE dbo.AuditLog;
GO

/* ======================================================
   1. BẢNG PHÂN LOẠI THIẾT BỊ / MÁY
   ====================================================== */
CREATE TABLE LoaiThietBi (
    LoaiID INT IDENTITY(1,1) PRIMARY KEY,
    TenLoai NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255) NULL
);
GO

/* ======================================================
   2. BẢNG MÁY TÍNH
   ====================================================== */
CREATE TABLE MayTinh (
    MayID INT IDENTITY(1,1) PRIMARY KEY,
    TenMay NVARCHAR(100) NOT NULL,
    TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Rảnh',
    ViTri NVARCHAR(100) NULL,
    CauHinh NVARCHAR(255) NULL,
    NgayMua DATE NULL,
    LoaiID INT NOT NULL,
    FOREIGN KEY (LoaiID) REFERENCES LoaiThietBi(LoaiID)
);
GO

/* ======================================================
   3. BẢNG THIẾT BỊ NGOẠI VI
   ====================================================== */
CREATE TABLE ThietBi (
    ThietBiID INT IDENTITY(1,1) PRIMARY KEY,
    TenThietBi NVARCHAR(100) NOT NULL,
    TinhTrang NVARCHAR(50) NOT NULL DEFAULT N'Tốt',
    SerialNumber VARCHAR(100) UNIQUE NULL,
    NgayMua DATE NULL,
    MayID INT NULL,
    LoaiID INT NOT NULL,
    FOREIGN KEY (MayID) REFERENCES MayTinh(MayID),
    FOREIGN KEY (LoaiID) REFERENCES LoaiThietBi(LoaiID)
);
GO

/* ======================================================
   4. BẢNG LỊCH SỬ BẢO TRÌ
   ====================================================== */
CREATE TABLE BaoTri (
    BaoTriID INT IDENTITY(1,1) PRIMARY KEY,
    NgayBaoTri DATETIME NOT NULL DEFAULT GETDATE(),
    NoiDung NVARCHAR(255) NOT NULL,
    NhanVienPhuTrach NVARCHAR(100) NOT NULL,
    ChiPhi DECIMAL(18,2) NULL,
    MayID INT NULL,
    ThietBiID INT NULL,
    FOREIGN KEY (MayID) REFERENCES MayTinh(MayID),
    FOREIGN KEY (ThietBiID) REFERENCES ThietBi(ThietBiID),
    CONSTRAINT CK_BaoTri_May_ThietBi CHECK (MayID IS NOT NULL OR ThietBiID IS NOT NULL)
);
GO

/* ======================================================
   5. BẢNG QUẢN LÝ NGƯỜI DÙNG & ROLES
   ====================================================== */
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL
);
GO

CREATE TABLE NguoiDung (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100),
    RoleID INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);
GO