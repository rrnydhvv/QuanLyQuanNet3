USE QuanLyQuanNet;
GO

/* ======================================================
   7. HÀM KIỂM TRA QUYỀN
   ====================================================== */
CREATE OR ALTER FUNCTION dbo.CheckPermission
(
    @UserID INT,
    @Action NVARCHAR(10), -- SELECT, INSERT, UPDATE, DELETE
    @TableName NVARCHAR(50)
)
RETURNS BIT
AS
BEGIN
    DECLARE @RoleID INT;
    SELECT @RoleID = RoleID FROM NguoiDung WHERE UserID = @UserID AND IsActive = 1;

    IF @RoleID IS NULL RETURN 0;

    -- Map phân quyền
    IF @RoleID = 1 RETURN 1; -- Admin: full

    IF @RoleID = 2 -- Tech
    BEGIN
        IF @TableName = N'MayTinh' AND @Action IN ('SELECT','UPDATE') RETURN 1;
        IF @TableName = N'ThietBi' AND @Action IN ('SELECT','UPDATE') RETURN 1;
        IF @TableName = N'BaoTri' AND @Action IN ('SELECT','INSERT','UPDATE') RETURN 1;
    END

    IF @RoleID = 3 -- Staff
    BEGIN
        IF @TableName IN (N'MayTinh',N'ThietBi',N'BaoTri') AND @Action='SELECT' RETURN 1;
    END

    RETURN 0;
END
GO

/* ======================================================
   8. CRUD STORED PROCEDURES for MayTính
   ====================================================== */
CREATE OR ALTER PROCEDURE sp_AddMayTinh
    @UserID INT,
    @TenMay NVARCHAR(100),
    @TrangThai NVARCHAR(50),
    @ViTri NVARCHAR(100),
    @CauHinh NVARCHAR(255),
    @NgayMua DATE,
    @LoaiID INT
AS
BEGIN
    IF dbo.CheckPermission(@UserID,'INSERT','MayTinh') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền thêm Máy Tính',16,1);
        RETURN;
    END

    INSERT INTO MayTinh(TenMay,TrangThai,ViTri,CauHinh,NgayMua,LoaiID)
    VALUES(@TenMay,@TrangThai,@ViTri,@CauHinh,@NgayMua,@LoaiID);
END
GO

CREATE OR ALTER PROCEDURE sp_GetMayTinh
    @UserID INT
AS
BEGIN
    IF dbo.CheckPermission(@UserID,'SELECT','MayTinh') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền xem Máy Tính',16,1);
        RETURN;
    END

    SELECT * FROM MayTinh;
END
GO

CREATE OR ALTER PROCEDURE sp_UpdateMayTinh
    @UserID INT,
    @MayID INT,
    @TenMay NVARCHAR(100),
    @TrangThai NVARCHAR(50),
    @ViTri NVARCHAR(100),
    @CauHinh NVARCHAR(255),
    @NgayMua DATE,
    @LoaiID INT
AS
BEGIN
    IF dbo.CheckPermission(@UserID,'UPDATE','MayTinh') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền sửa Máy Tính',16,1);
        RETURN;
    END

    UPDATE MayTinh
    SET TenMay=@TenMay, TrangThai=@TrangThai, ViTri=@ViTri, CauHinh=@CauHinh, NgayMua=@NgayMua, LoaiID=@LoaiID
    WHERE MayID=@MayID;
END
GO

CREATE OR ALTER PROCEDURE sp_DeleteMayTinh
    @UserID INT,
    @MayID INT
AS
BEGIN
    IF dbo.CheckPermission(@UserID,'DELETE','MayTinh') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền xóa Máy Tính',16,1);
        RETURN;
    END

    DELETE FROM MayTinh WHERE MayID=@MayID;
END
GO

/* ======================================================
   CRUD STORED PROCEDURES FOR ThietBi
   ====================================================== */

-- Add ThietBi
CREATE OR ALTER PROCEDURE sp_AddThietBi
    @UserID INT,
    @TenThietBi NVARCHAR(100),
    @TinhTrang NVARCHAR(50),
    @SerialNumber VARCHAR(100),
    @NgayMua DATE,
    @MayID INT,
    @LoaiID INT
AS
BEGIN
    IF dbo.CheckPermission(@UserID, 'INSERT', 'ThietBi') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền thêm Thiết Bị', 16, 1);
        RETURN;
    END

    INSERT INTO ThietBi(TenThietBi, TinhTrang, SerialNumber, NgayMua, MayID, LoaiID)
    VALUES(@TenThietBi, @TinhTrang, @SerialNumber, @NgayMua, @MayID, @LoaiID);
END
GO

-- Get ThietBi
CREATE OR ALTER PROCEDURE sp_GetThietBi
    @UserID INT
AS
BEGIN
    IF dbo.CheckPermission(@UserID, 'SELECT', 'ThietBi') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền xem Thiết Bị', 16, 1);
        RETURN;
    END

    SELECT * FROM ThietBi;
END
GO

-- Update ThietBi
CREATE OR ALTER PROCEDURE sp_UpdateThietBi
    @UserID INT,
    @ThietBiID INT,
    @TenThietBi NVARCHAR(100),
    @TinhTrang NVARCHAR(50),
    @SerialNumber VARCHAR(100),
    @NgayMua DATE,
    @MayID INT,
    @LoaiID INT
AS
BEGIN
    IF dbo.CheckPermission(@UserID, 'UPDATE', 'ThietBi') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền sửa Thiết Bị', 16, 1);
        RETURN;
    END

    UPDATE ThietBi
    SET TenThietBi = @TenThietBi, 
        TinhTrang = @TinhTrang, 
        SerialNumber = @SerialNumber, 
        NgayMua = @NgayMua, 
        MayID = @MayID, 
        LoaiID = @LoaiID
    WHERE ThietBiID = @ThietBiID;
END
GO

-- Delete ThietBi
CREATE OR ALTER PROCEDURE sp_DeleteThietBi
    @UserID INT,
    @ThietBiID INT
AS
BEGIN
    IF dbo.CheckPermission(@UserID, 'DELETE', 'ThietBi') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền xóa Thiết Bị', 16, 1);
        RETURN;
    END

    DELETE FROM ThietBi WHERE ThietBiID = @ThietBiID;
END
GO

/* ======================================================
   CRUD STORED PROCEDURES FOR BaoTri
   ====================================================== */

-- Add BaoTri
CREATE OR ALTER PROCEDURE sp_AddBaoTri
    @UserID INT,
    @NgayBaoTri DATETIME,
    @NoiDung NVARCHAR(255),
    @NhanVienPhuTrach NVARCHAR(100),
    @ChiPhi DECIMAL(18,2),
    @MayID INT,
    @ThietBiID INT
AS
BEGIN
    IF dbo.CheckPermission(@UserID, 'INSERT', 'BaoTri') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền thêm Bảo Trì', 16, 1);
        RETURN;
    END

    IF @MayID IS NULL AND @ThietBiID IS NULL
    BEGIN
        RAISERROR(N'Phải cung cấp ít nhất MayID hoặc ThietBiID', 16, 1);
        RETURN;
    END

    INSERT INTO BaoTri(NgayBaoTri, NoiDung, NhanVienPhuTrach, ChiPhi, MayID, ThietBiID)
    VALUES(@NgayBaoTri, @NoiDung, @NhanVienPhuTrach, @ChiPhi, @MayID, @ThietBiID);
END
GO

-- Get BaoTri
CREATE OR ALTER PROCEDURE sp_GetBaoTri
    @UserID INT
AS
BEGIN
    IF dbo.CheckPermission(@UserID, 'SELECT', 'BaoTri') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền xem Bảo Trì', 16, 1);
        RETURN;
    END

    SELECT * FROM BaoTri;
END
GO

-- Update BaoTri
CREATE OR ALTER PROCEDURE sp_UpdateBaoTri
    @UserID INT,
    @BaoTriID INT,
    @NgayBaoTri DATETIME,
    @NoiDung NVARCHAR(255),
    @NhanVienPhuTrach NVARCHAR(100),
    @ChiPhi DECIMAL(18,2),
    @MayID INT,
    @ThietBiID INT
AS
BEGIN
    IF dbo.CheckPermission(@UserID, 'UPDATE', 'BaoTri') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền sửa Bảo Trì', 16, 1);
        RETURN;
    END

    IF @MayID IS NULL AND @ThietBiID IS NULL
    BEGIN
        RAISERROR(N'Phải cung cấp ít nhất MayID hoặc ThietBiID', 16, 1);
        RETURN;
    END

    UPDATE BaoTri
    SET NgayBaoTri = @NgayBaoTri, 
        NoiDung = @NoiDung, 
        NhanVienPhuTrach = @NhanVienPhuTrach, 
        ChiPhi = @ChiPhi, 
        MayID = @MayID, 
        ThietBiID = @ThietBiID
    WHERE BaoTriID = @BaoTriID;
END
GO

-- Delete BaoTri
CREATE OR ALTER PROCEDURE sp_DeleteBaoTri
    @UserID INT,
    @BaoTriID INT
AS
BEGIN
    IF dbo.CheckPermission(@UserID, 'DELETE', 'BaoTri') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền xóa Bảo Trì', 16, 1);
        RETURN;
    END

    DELETE FROM BaoTri WHERE BaoTriID = @BaoTriID;
END
GO

/* ======================================================
   TRIGGER FOR AUTOMATIC PASSWORD HASHING ON INSERT
   ====================================================== */
IF OBJECT_ID('trg_NguoiDung_HashPassword', 'TR') IS NOT NULL
    DROP TRIGGER trg_NguoiDung_HashPassword;
GO

CREATE TRIGGER trg_NguoiDung_HashPassword
ON NguoiDung
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO NguoiDung (Username, PasswordHash, FullName, RoleID, IsActive)
    SELECT 
        Username,
        CONVERT(NVARCHAR(255), HASHBYTES('SHA2_256', PasswordHash), 2), -- Hash the provided plain-text password
        FullName,
        RoleID,
        IsActive
    FROM inserted;
END
GO

/* ======================================================
   CRUD STORED PROCEDURES FOR NguoiDung
   ====================================================== */

-- Add NguoiDung
CREATE OR ALTER PROCEDURE sp_AddNguoiDung
    @UserIDCaller INT,
    @Username NVARCHAR(50),
    @Password NVARCHAR(255), -- Plain-text password
    @FullName NVARCHAR(100),
    @RoleID INT,
    @IsActive BIT
AS
BEGIN
    IF dbo.CheckPermission(@UserIDCaller, 'INSERT', 'NguoiDung') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền thêm Người Dùng', 16, 1);
        RETURN;
    END

    -- Insert with plain-text password; trigger will handle hashing
    INSERT INTO NguoiDung (Username, PasswordHash, FullName, RoleID, IsActive)
    VALUES (@Username, @Password, @FullName, @RoleID, @IsActive);
END
GO

-- Get NguoiDung
CREATE OR ALTER PROCEDURE sp_GetNguoiDung
    @UserIDCaller INT
AS
BEGIN
    IF dbo.CheckPermission(@UserIDCaller, 'SELECT', 'NguoiDung') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền xem Người Dùng', 16, 1);
        RETURN;
    END

    -- Exclude PasswordHash from output for security
    SELECT UserID, Username, FullName, RoleID, IsActive 
    FROM NguoiDung;
END
GO

-- Update NguoiDung
CREATE OR ALTER PROCEDURE sp_UpdateNguoiDung
    @UserIDCaller INT,
    @UserID INT,
    @Username NVARCHAR(50),
    @Password NVARCHAR(255), -- Plain-text password
    @FullName NVARCHAR(100),
    @RoleID INT,
    @IsActive BIT
AS
BEGIN
    IF dbo.CheckPermission(@UserIDCaller, 'UPDATE', 'NguoiDung') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền sửa Người Dùng', 16, 1);
        RETURN;
    END

    -- Hash the password if provided; otherwise retain existing hash
    DECLARE @PasswordHash NVARCHAR(255);
    IF @Password IS NOT NULL AND @Password <> ''
        SET @PasswordHash = CONVERT(NVARCHAR(255), HASHBYTES('SHA2_256', @Password), 2);
    ELSE
        SELECT @PasswordHash = PasswordHash FROM NguoiDung WHERE UserID = @UserID;

    UPDATE NguoiDung
    SET Username = @Username,
        PasswordHash = @PasswordHash,
        FullName = @FullName,
        RoleID = @RoleID,
        IsActive = @IsActive
    WHERE UserID = @UserID;
END
GO

-- Delete NguoiDung (Soft Delete by setting IsActive = 0)
CREATE OR ALTER PROCEDURE sp_DeleteNguoiDung
    @UserIDCaller INT,
    @UserID INT
AS
BEGIN
    IF dbo.CheckPermission(@UserIDCaller, 'DELETE', 'NguoiDung') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền xóa Người Dùng', 16, 1);
        RETURN;
    END

    -- Soft delete by setting IsActive to 0
    UPDATE NguoiDung
    SET IsActive = 0
    WHERE UserID = @UserID;
END
GO