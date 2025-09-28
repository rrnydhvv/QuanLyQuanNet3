USE QuanLyQuanNet;
GO

/* ======================================================
   STORED PROCEDURE ĐĂNG NHẬP
   ====================================================== */
CREATE OR ALTER PROCEDURE sp_LoginNguoiDung
    @Username NVARCHAR(50),
    @Password NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StoredHash NVARCHAR(255);
    DECLARE @UserID INT, @FullName NVARCHAR(100), @RoleID INT, @IsActive BIT;

    -- Lấy thông tin người dùng
    SELECT @StoredHash = PasswordHash, 
           @UserID = UserID, 
           @FullName = FullName, 
           @RoleID = RoleID, 
           @IsActive = IsActive
    FROM NguoiDung 
    WHERE Username = @Username;

    -- Kiểm tra xem người dùng tồn tại và đang hoạt động
    IF @StoredHash IS NULL OR @IsActive = 0
    BEGIN
        RAISERROR(N'Tên đăng nhập không tồn tại hoặc tài khoản không hoạt động', 16, 1);
        RETURN;
    END

    -- So sánh mật khẩu
    IF @StoredHash = CONVERT(NVARCHAR(255), HASHBYTES('SHA2_256', @Password), 2)
    BEGIN
        SELECT 
            UserID,
            Username,
            FullName,
            RoleID,
            IsActive
        FROM NguoiDung
        WHERE UserID = @UserID;
    END
    ELSE
    BEGIN
        RAISERROR(N'Mật khẩu không đúng', 16, 1);
        RETURN;
    END
END
GO

/* ======================================================
   STORED PROCEDURE LẤY DANH SÁCH LOẠI THIẾT BỊ
   ====================================================== */
CREATE OR ALTER PROCEDURE sp_GetLoaiThietBi
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF dbo.CheckPermission(@UserID, 'SELECT', 'LoaiThietBi') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền xem Loại Thiết Bị', 16, 1);
        RETURN;
    END

    SELECT LoaiID, TenLoai, MoTa
    FROM LoaiThietBi
    ORDER BY TenLoai;
END
GO

/* ======================================================
   STORED PROCEDURE THÊM LOẠI THIẾT BỊ
   ====================================================== */
CREATE OR ALTER PROCEDURE sp_AddLoaiThietBi
    @UserID INT,
    @TenLoai NVARCHAR(100),
    @MoTa NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF dbo.CheckPermission(@UserID, 'INSERT', 'LoaiThietBi') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền thêm Loại Thiết Bị', 16, 1);
        RETURN;
    END

    INSERT INTO LoaiThietBi (TenLoai, MoTa)
    VALUES (@TenLoai, @MoTa);
END
GO

/* ======================================================
   STORED PROCEDURE SỬA LOẠI THIẾT BỊ
   ====================================================== */
CREATE OR ALTER PROCEDURE sp_UpdateLoaiThietBi
    @UserID INT,
    @LoaiID INT,
    @TenLoai NVARCHAR(100),
    @MoTa NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF dbo.CheckPermission(@UserID, 'UPDATE', 'LoaiThietBi') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền sửa Loại Thiết Bị', 16, 1);
        RETURN;
    END

    UPDATE LoaiThietBi
    SET TenLoai = @TenLoai,
        MoTa = @MoTa
    WHERE LoaiID = @LoaiID;
END
GO

/* ======================================================
   STORED PROCEDURE XÓA LOẠI THIẾT BỊ
   ====================================================== */
CREATE OR ALTER PROCEDURE sp_DeleteLoaiThietBi
    @UserID INT,
    @LoaiID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF dbo.CheckPermission(@UserID, 'DELETE', 'LoaiThietBi') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền xóa Loại Thiết Bị', 16, 1);
        RETURN;
    END

    -- Kiểm tra xem LoaiID có đang được sử dụng trong MayTinh hoặc ThietBi
    IF EXISTS (SELECT 1 FROM MayTinh WHERE LoaiID = @LoaiID)
       OR EXISTS (SELECT 1 FROM ThietBi WHERE LoaiID = @LoaiID)
    BEGIN
        RAISERROR(N'Loại thiết bị đang được sử dụng, không thể xóa', 16, 1);
        RETURN;
    END

    DELETE FROM LoaiThietBi
    WHERE LoaiID = @LoaiID;
END
GO

/* ======================================================
   STORED PROCEDURE LỌC MÁY TÍNH THEO TRẠNG THÁI
   ====================================================== */
CREATE OR ALTER PROCEDURE sp_FilterMayTinhByTrangThai
    @UserID INT,
    @TrangThai NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF dbo.CheckPermission(@UserID, 'SELECT', 'MayTinh') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền xem Máy Tính', 16, 1);
        RETURN;
    END

    SELECT MayID, TenMay, TrangThai, ViTri, CauHinh, NgayMua, LoaiID
    FROM MayTinh
    WHERE TrangThai = @TrangThai
    ORDER BY MayID;
END
GO

/* ======================================================
   STORED PROCEDURE LỌC THIẾT BỊ THEO TÌNH TRẠNG
   ====================================================== */
CREATE OR ALTER PROCEDURE sp_FilterThietBiByTinhTrang
    @UserID INT,
    @TinhTrang NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF dbo.CheckPermission(@UserID, 'SELECT', 'ThietBi') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền xem Thiết Bị', 16, 1);
        RETURN;
    END

    SELECT ThietBiID, TenThietBi, TinhTrang, SerialNumber, NgayMua, MayID, LoaiID
    FROM ThietBi
    WHERE TinhTrang = @TinhTrang
    ORDER BY ThietBiID;
END
GO

/* ======================================================
   STORED PROCEDURE LẤY LỊCH SỬ BẢO TRÌ THEO MÁY HOẶC THIẾT BỊ
   ====================================================== */
CREATE OR ALTER PROCEDURE sp_GetBaoTriByMayOrThietBi
    @UserID INT,
    @MayID INT = NULL,
    @ThietBiID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF dbo.CheckPermission(@UserID, 'SELECT', 'BaoTri') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền xem Bảo Trì', 16, 1);
        RETURN;
    END

    IF @MayID IS NULL AND @ThietBiID IS NULL
    BEGIN
        RAISERROR(N'Phải cung cấp ít nhất MayID hoặc ThietBiID', 16, 1);
        RETURN;
    END

    SELECT BaoTriID, NgayBaoTri, NoiDung, NhanVienPhuTrach, ChiPhi, MayID, ThietBiID
    FROM BaoTri
    WHERE (@MayID IS NOT NULL AND MayID = @MayID)
       OR (@ThietBiID IS NOT NULL AND ThietBiID = @ThietBiID)
    ORDER BY NgayBaoTri DESC;
END
GO

/* ======================================================
   STORED PROCEDURE THAY ĐỔI MẬT KHẨU
   ====================================================== */
CREATE OR ALTER PROCEDURE sp_ChangePassword
    @UserID INT,
    @OldPassword NVARCHAR(255),
    @NewPassword NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StoredHash NVARCHAR(255);

    -- Lấy mật khẩu hiện tại
    SELECT @StoredHash = PasswordHash
    FROM NguoiDung
    WHERE UserID = @UserID AND IsActive = 1;

    IF @StoredHash IS NULL
    BEGIN
        RAISERROR(N'Người dùng không tồn tại hoặc không hoạt động', 16, 1);
        RETURN;
    END

    -- Kiểm tra mật khẩu cũ
    IF @StoredHash != CONVERT(NVARCHAR(255), HASHBYTES('SHA2_256', @OldPassword), 2)
    BEGIN
        RAISERROR(N'Mật khẩu cũ không đúng', 16, 1);
        RETURN;
    END

    -- Cập nhật mật khẩu mới
    UPDATE NguoiDung
    SET PasswordHash = CONVERT(NVARCHAR(255), HASHBYTES('SHA2_256', @NewPassword), 2)
    WHERE UserID = @UserID;
END
GO