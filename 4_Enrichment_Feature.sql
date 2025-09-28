USE QuanLyQuanNet;
GO

/* ======================================================
   TẠO BẢNG AUDITLOG ĐỂ GHI NHẬT KÝ HÀNH ĐỘNG
   ====================================================== */
IF OBJECT_ID('AuditLog', 'U') IS NOT NULL
    DROP TABLE AuditLog;
GO

CREATE TABLE AuditLog
(
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT,
    Action NVARCHAR(10), -- INSERT, UPDATE, DELETE
    TableName NVARCHAR(50),
    RecordID INT,
    ActionTime DATETIME DEFAULT GETDATE(),
    Details NVARCHAR(1000)
);
GO

/* ======================================================
   TRIGGER GHI NHẬT KÝ HÀNH ĐỘNG CHO NGƯỜI DÙNG
   ====================================================== */
IF OBJECT_ID('trg_AuditLog_NguoiDung', 'TR') IS NOT NULL
    DROP TRIGGER trg_AuditLog_NguoiDung;
GO

CREATE TRIGGER trg_AuditLog_NguoiDung
ON NguoiDung
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserID INT = NULL; -- Lấy từ biến hệ thống hoặc ứng dụng
    DECLARE @Action NVARCHAR(10);
    DECLARE @RecordID INT;
    DECLARE @Details NVARCHAR(1000);

    -- Xác định hành động
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted)
        SET @Action = 'UPDATE';
    ELSE IF EXISTS (SELECT * FROM inserted)
        SET @Action = 'INSERT';
    ELSE
        SET @Action = 'DELETE';

    -- Lấy RecordID và Details
    IF @Action IN ('INSERT', 'UPDATE')
    BEGIN
        SELECT @RecordID = UserID, 
               @Details = N'Username: ' + Username + N', FullName: ' + FullName + N', RoleID: ' + CAST(RoleID AS NVARCHAR(10))
        FROM inserted;
    END
    ELSE -- DELETE
    BEGIN
        SELECT @RecordID = UserID, 
               @Details = N'Username: ' + Username + N', FullName: ' + FullName + N', RoleID: ' + CAST(RoleID AS NVARCHAR(10))
        FROM deleted;
    END

    -- Ghi vào AuditLog
    INSERT INTO AuditLog (UserID, Action, TableName, RecordID, ActionTime, Details)
    VALUES (@UserID, @Action, N'NguoiDung', @RecordID, GETDATE(), @Details);
END
GO

IF OBJECT_ID('trg_AuditLog_MayTinh', 'TR') IS NOT NULL
    DROP TRIGGER trg_AuditLog_MayTinh;
GO

CREATE TRIGGER trg_AuditLog_MayTinh
ON MayTinh
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Action NVARCHAR(10), @RecordID INT, @Details NVARCHAR(1000);
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted)
        SET @Action = 'UPDATE';
    ELSE IF EXISTS (SELECT * FROM inserted)
        SET @Action = 'INSERT';
    ELSE
        SET @Action = 'DELETE';
    IF @Action IN ('INSERT', 'UPDATE')
        SELECT @RecordID = MayID, 
               @Details = N'TenMay: ' + TenMay + N', TrangThai: ' + TrangThai
        FROM inserted;
    ELSE
        SELECT @RecordID = MayID, 
               @Details = N'TenMay: ' + TenMay + N', TrangThai: ' + TrangThai
        FROM deleted;
    INSERT INTO AuditLog (UserID, Action, TableName, RecordID, ActionTime, Details)
    VALUES (NULL, @Action, N'MayTinh', @RecordID, GETDATE(), @Details);
END
GO

/* ======================================================
   TRIGGER KIỂM TRA TRẠNG THÁI MÁY TÍNH
   ====================================================== */
IF OBJECT_ID('trg_MayTinh_ValidateTrangThai', 'TR') IS NOT NULL
    DROP TRIGGER trg_MayTinh_ValidateTrangThai;
GO

CREATE TRIGGER trg_MayTinh_ValidateTrangThai
ON MayTinh
INSTEAD OF INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @InvalidTrangThai NVARCHAR(50);

    -- Kiểm tra trạng thái hợp lệ
    SELECT @InvalidTrangThai = TrangThai
    FROM inserted
    WHERE TrangThai NOT IN (N'Rảnh', N'Đang Sử Dụng', N'Bảo Trì');

    IF @InvalidTrangThai IS NOT NULL
    BEGIN
        RAISERROR(N'Trạng thái máy tính không hợp lệ: %s. Chỉ cho phép: Rảnh, Đang Sử Dụng, Bảo Trì', 16, 1, @InvalidTrangThai);
        RETURN;
    END

    -- Thực hiện INSERT hoặc UPDATE nếu trạng thái hợp lệ
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted)
    BEGIN
        -- UPDATE
        UPDATE MayTinh
        SET TenMay = i.TenMay,
            TrangThai = i.TrangThai,
            ViTri = i.ViTri,
            CauHinh = i.CauHinh,
            NgayMua = i.NgayMua,
            LoaiID = i.LoaiID
        FROM inserted i
        WHERE MayTinh.MayID = i.MayID;
    END
    ELSE
    BEGIN
        -- INSERT
        INSERT INTO MayTinh (TenMay, TrangThai, ViTri, CauHinh, NgayMua, LoaiID)
        SELECT TenMay, TrangThai, ViTri, CauHinh, NgayMua, LoaiID
        FROM inserted;
    END
END
GO

/* ======================================================
   STORED PROCEDURE BÁO CÁO THỐNG KÊ MÁY TÍNH
   ====================================================== */
CREATE OR ALTER PROCEDURE sp_GetMayTinhStatistics
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF dbo.CheckPermission(@UserID, 'SELECT', 'MayTinh') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền xem thống kê Máy Tính', 16, 1);
        RETURN;
    END

    SELECT 
        TrangThai,
        COUNT(*) AS SoLuong,
        CAST(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER () AS DECIMAL(5,2)) AS TyLePhanTram
    FROM MayTinh
    GROUP BY TrangThai;
END
GO

/* ======================================================
   STORED PROCEDURE BÁO CÁO CHI PHÍ BẢO TRÌ
   ====================================================== */
CREATE OR ALTER PROCEDURE sp_GetBaoTriCostReport
    @UserID INT,
    @StartDate DATE = NULL,
    @EndDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF dbo.CheckPermission(@UserID, 'SELECT', 'BaoTri') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền xem báo cáo chi phí Bảo Trì', 16, 1);
        RETURN;
    END

    SELECT 
        ISNULL(CAST(MayID AS NVARCHAR(10)), N'N/A') AS MayID,
        ISNULL(CAST(ThietBiID AS NVARCHAR(10)), N'N/A') AS ThietBiID,
        SUM(ChiPhi) AS TongChiPhi,
        COUNT(*) AS SoLuotBaoTri
    FROM BaoTri
    WHERE (@StartDate IS NULL OR NgayBaoTri >= @StartDate)
      AND (@EndDate IS NULL OR NgayBaoTri <= @EndDate)
    GROUP BY MayID, ThietBiID;
END
GO

/* ======================================================
   STORED PROCEDURE CHUYỂN ĐỔI TRẠNG THÁI MÁY TÍNH
   ====================================================== */
CREATE OR ALTER PROCEDURE sp_ChangeMayTinhTrangThai
    @UserID INT,
    @MayID INT,
    @TrangThai NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF dbo.CheckPermission(@UserID, 'UPDATE', 'MayTinh') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền thay đổi trạng thái Máy Tính', 16, 1);
        RETURN;
    END

    -- Kiểm tra trạng thái hợp lệ (dư phòng, vì trigger đã kiểm tra)
    IF @TrangThai NOT IN (N'Rảnh', N'Đang Sử Dụng', N'Bảo Trì')
    BEGIN
        RAISERROR(N'Trạng thái không hợp lệ. Chỉ cho phép: Rảnh, Đang Sử Dụng, Bảo Trì', 16, 1);
        RETURN;
    END

    UPDATE MayTinh
    SET TrangThai = @TrangThai
    WHERE MayID = @MayID;
END
GO

/* ======================================================
   STORED PROCEDURE TÌM KIẾM MÁY TÍNH ĐA TIÊU CHÍ
   ====================================================== */
CREATE OR ALTER PROCEDURE sp_SearchMayTinh
    @UserID INT,
    @TenMay NVARCHAR(100) = NULL,
    @TrangThai NVARCHAR(50) = NULL,
    @ViTri NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF dbo.CheckPermission(@UserID, 'SELECT', 'MayTinh') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền tìm kiếm Máy Tính', 16, 1);
        RETURN;
    END

    SELECT MayID, TenMay, TrangThai, ViTri, CauHinh, NgayMua, LoaiID
    FROM MayTinh
    WHERE (@TenMay IS NULL OR TenMay LIKE '%' + @TenMay + '%')
      AND (@TrangThai IS NULL OR TrangThai = @TrangThai)
      AND (@ViTri IS NULL OR ViTri LIKE '%' + @ViTri + '%')
    ORDER BY MayID;
END
GO

/* ======================================================
   STORED PROCEDURE TÌM KIẾM THIẾT BỊ ĐA TIÊU CHÍ
   ====================================================== */
CREATE OR ALTER PROCEDURE sp_SearchThietBi
    @UserID INT,
    @TenThietBi NVARCHAR(100) = NULL,
    @TinhTrang NVARCHAR(50) = NULL,
    @SerialNumber VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF dbo.CheckPermission(@UserID, 'SELECT', 'ThietBi') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền tìm kiếm Thiết Bị', 16, 1);
        RETURN;
    END

    SELECT ThietBiID, TenThietBi, TinhTrang, SerialNumber, NgayMua, MayID, LoaiID
    FROM ThietBi
    WHERE (@TenThietBi IS NULL OR TenThietBi LIKE '%' + @TenThietBi + '%')
      AND (@TinhTrang IS NULL OR TinhTrang = @TinhTrang)
      AND (@SerialNumber IS NULL OR SerialNumber LIKE '%' + @SerialNumber + '%')
    ORDER BY ThietBiID;
END
GO

/* ======================================================
   STORED PROCEDURE TÌM KIẾM BẢO TRÌ ĐA TIÊU CHÍ
   ====================================================== */
CREATE OR ALTER PROCEDURE sp_SearchBaoTri
    @UserID INT,
    @MayID INT = NULL,
    @ThietBiID INT = NULL,
    @StartDate DATE = NULL,
    @EndDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF dbo.CheckPermission(@UserID, 'SELECT', 'BaoTri') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền tìm kiếm Bảo Trì', 16, 1);
        RETURN;
    END

    SELECT BaoTriID, NgayBaoTri, NoiDung, NhanVienPhuTrach, ChiPhi, MayID, ThietBiID
    FROM BaoTri
    WHERE (@MayID IS NULL OR MayID = @MayID)
      AND (@ThietBiID IS NULL OR ThietBiID = @ThietBiID)
      AND (@StartDate IS NULL OR NgayBaoTri >= @StartDate)
      AND (@EndDate IS NULL OR NgayBaoTri <= @EndDate)
    ORDER BY NgayBaoTri DESC;
END
GO

/* ======================================================
   STORED PROCEDURE LẤY DANH SÁCH ROLES
   ====================================================== */
CREATE OR ALTER PROCEDURE sp_GetRoles
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF dbo.CheckPermission(@UserID, 'SELECT', 'NguoiDung') = 0
    BEGIN
        RAISERROR(N'Bạn không có quyền xem danh sách Roles', 16, 1);
        RETURN;
    END

    SELECT RoleID, RoleName
    FROM Roles
    ORDER BY RoleID;
END
GO