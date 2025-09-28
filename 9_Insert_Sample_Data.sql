USE QuanLyQuanNet;
GO

/* ======================================================
   DỮ LIỆU MẪU ROLES
   ====================================================== */
DELETE FROM Roles;
INSERT INTO Roles (RoleID, RoleName)
VALUES 
    (1, N'Admin'),   -- Toàn quyền
    (2, N'Tech'),    -- Kỹ thuật viên
    (3, N'Staff');   -- Nhân viên
GO

/* ======================================================
   DỮ LIỆU MẪU NGƯỜI DÙNG
   ====================================================== */
DELETE FROM NguoiDung;

-- Insert admin user directly (trigger hashes password)
INSERT INTO NguoiDung (Username, PasswordHash, FullName, RoleID, IsActive)
VALUES (N'admin', N'AdminPass123', N'Nguyễn Văn Quản Trị', 1, 1);

-- Insert additional users using sp_AddNguoiDung
EXEC sp_AddNguoiDung 
    @UserIDCaller = 1, -- Admin
    @Username = N'tech1',
    @Password = N'TechPass456',
    @FullName = N'Trần Văn Kỹ Thuật',
    @RoleID = 2, -- Tech
    @IsActive = 1;

EXEC sp_AddNguoiDung 
    @UserIDCaller = 1,
    @Username = N'tech2',
    @Password = N'TechPass789',
    @FullName = N'Lê Thị Kỹ Thuật',
    @RoleID = 2,
    @IsActive = 1;

EXEC sp_AddNguoiDung 
    @UserIDCaller = 1,
    @Username = N'staff1',
    @Password = N'StaffPass123',
    @FullName = N'Phạm Văn Nhân Viên',
    @RoleID = 3, -- Staff
    @IsActive = 1;

EXEC sp_AddNguoiDung 
    @UserIDCaller = 1,
    @Username = N'staff2',
    @Password = N'StaffPass456',
    @FullName = N'Ngô Thị Nhân Viên',
    @RoleID = 3,
    @IsActive = 1;

EXEC sp_AddNguoiDung 
    @UserIDCaller = 1,
    @Username = N'staff3',
    @Password = N'StaffPass789',
    @FullName = N'Đỗ Văn Nhân Viên',
    @RoleID = 3,
    @IsActive = 0; -- Inactive staff for demo

-- Verify NguoiDung data
SELECT UserID, Username, FullName, RoleID, IsActive FROM NguoiDung;
GO

/* ======================================================
   DỮ LIỆU MẪU LOẠI THIẾT BỊ
   ====================================================== */
DELETE FROM LoaiThietBi;
INSERT INTO LoaiThietBi (TenLoai, MoTa)
VALUES 
    (N'Máy Tính', N'Máy tính để bàn hoặc laptop dùng trong quán net'),
    (N'Màn Hình', N'Màn hình LCD hoặc LED cho máy tính'),
    (N'Chuột', N'Chuột máy tính quang học hoặc cơ'),
    (N'Bàn Phím', N'Bàn phím cơ hoặc màng'),
    (N'Tai Nghe', N'Tai nghe có micro cho game thủ'),
    (N'Ghế Gaming', N'Ghế chuyên dụng cho quán net'),
    (N'Webcam', N'Thiết bị quay video hoặc stream');
GO

/* ======================================================
   DỮ LIỆU MẪU MÁY TÍNH
   ====================================================== */
DELETE FROM MayTinh;
EXEC sp_AddMayTinh 
    @UserID = 1, -- Admin
    @TenMay = N'Máy 01',
    @TrangThai = N'Rảnh',
    @ViTri = N'Khu A - Dãy 1 - Máy 1',
    @CauHinh = N'CPU i5-12400, RAM 16GB, GPU RTX 3060, SSD 512GB',
    @NgayMua = '2024-01-10',
    @LoaiID = 1;

EXEC sp_AddMayTinh 
    @UserID = 1,
    @TenMay = N'Máy 02',
    @TrangThai = N'Đang Sử Dụng',
    @ViTri = N'Khu A - Dãy 1 - Máy 2',
    @CauHinh = N'CPU Ryzen 5 5600X, RAM 32GB, GPU RX 6700 XT, SSD 1TB',
    @NgayMua = '2024-02-15',
    @LoaiID = 1;

EXEC sp_AddMayTinh 
    @UserID = 1,
    @TenMay = N'Máy 03',
    @TrangThai = N'Bảo Trì',
    @ViTri = N'Khu A - Dãy 2 - Máy 1',
    @CauHinh = N'CPU i7-11700, RAM 16GB, GPU GTX 1660 Ti, HDD 1TB',
    @NgayMua = '2023-11-20',
    @LoaiID = 1;

EXEC sp_AddMayTinh 
    @UserID = 1,
    @TenMay = N'Máy 04',
    @TrangThai = N'Rảnh',
    @ViTri = N'Khu B - Dãy 1 - Máy 1',
    @CauHinh = N'CPU i5-13400, RAM 16GB, GPU RTX 3070, SSD 512GB',
    @NgayMua = '2024-03-05',
    @LoaiID = 1;

EXEC sp_AddMayTinh 
    @UserID = 1,
    @TenMay = N'Máy 05',
    @TrangThai = N'Đang Sử Dụng',
    @ViTri = N'Khu B - Dãy 1 - Máy 2',
    @CauHinh = N'CPU Ryzen 7 5800X, RAM 32GB, GPU RTX 3080, SSD 1TB',
    @NgayMua = '2024-04-10',
    @LoaiID = 1;

EXEC sp_AddMayTinh 
    @UserID = 1,
    @TenMay = N'Máy 06',
    @TrangThai = N'Rảnh',
    @ViTri = N'Khu B - Dãy 2 - Máy 1',
    @CauHinh = N'CPU i3-12100, RAM 8GB, GPU GTX 1650, SSD 256GB',
    @NgayMua = '2023-10-15',
    @LoaiID = 1;

EXEC sp_AddMayTinh 
    @UserID = 1,
    @TenMay = N'Máy 07',
    @TrangThai = N'Bảo Trì',
    @ViTri = N'Khu C - Dãy 1 - Máy 1',
    @CauHinh = N'CPU i5-11400, RAM 16GB, GPU RTX 2060, SSD 512GB',
    @NgayMua = '2023-12-01',
    @LoaiID = 1;

EXEC sp_AddMayTinh 
    @UserID = 1,
    @TenMay = N'Máy 08',
    @TrangThai = N'Rảnh',
    @ViTri = N'Khu C - Dãy 1 - Máy 2',
    @CauHinh = N'CPU Ryzen 5 4600G, RAM 16GB, GPU Integrated, SSD 512GB',
    @NgayMua = '2024-01-25',
    @LoaiID = 1;

EXEC sp_AddMayTinh 
    @UserID = 1,
    @TenMay = N'Máy 09',
    @TrangThai = N'Đang Sử Dụng',
    @ViTri = N'Khu C - Dãy 2 - Máy 1',
    @CauHinh = N'CPU i7-12700, RAM 32GB, GPU RTX 3090, SSD 1TB',
    @NgayMua = '2024-05-10',
    @LoaiID = 1;

EXEC sp_AddMayTinh 
    @UserID = 1,
    @TenMay = N'Máy 10',
    @TrangThai = N'Rảnh',
    @ViTri = N'Khu C - Dãy 2 - Máy 2',
    @CauHinh = N'CPU Ryzen 5 5500, RAM 16GB, GPU RX 6600, SSD 512GB',
    @NgayMua = '2024-06-15',
    @LoaiID = 1;

-- Verify MayTinh data
SELECT * FROM MayTinh;
GO

/* ======================================================
   DỮ LIỆU MẪU THIẾT BỊ
   ====================================================== */
DELETE FROM ThietBi;
EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Màn Hình 01',
    @TinhTrang = N'Tốt',
    @SerialNumber = N'MH001',
    @NgayMua = '2024-01-10',
    @MayID = 1,
    @LoaiID = 2;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Màn Hình 02',
    @TinhTrang = N'Tốt',
    @SerialNumber = N'MH002',
    @NgayMua = '2024-02-15',
    @MayID = 2,
    @LoaiID = 2;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Màn Hình 03',
    @TinhTrang = N'Hỏng',
    @SerialNumber = N'MH003',
    @NgayMua = '2023-11-20',
    @MayID = 3,
    @LoaiID = 2;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Chuột 01',
    @TinhTrang = N'Tốt',
    @SerialNumber = N'CH001',
    @NgayMua = '2024-01-10',
    @MayID = 1,
    @LoaiID = 3;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Chuột 02',
    @TinhTrang = N'Tốt',
    @SerialNumber = N'CH002',
    @NgayMua = '2024-02-15',
    @MayID = 2,
    @LoaiID = 3;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Chuột 03',
    @TinhTrang = N'Tốt',
    @SerialNumber = N'CH003',
    @NgayMua = '2023-11-20',
    @MayID = 3,
    @LoaiID = 3;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Bàn Phím 01',
    @TinhTrang = N'Tốt',
    @SerialNumber = N'BP001',
    @NgayMua = '2024-01-10',
    @MayID = 1,
    @LoaiID = 4;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Bàn Phím 02',
    @TinhTrang = N'Hỏng',
    @SerialNumber = N'BP002',
    @NgayMua = '2024-02-15',
    @MayID = 2,
    @LoaiID = 4;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Bàn Phím 03',
    @TinhTrang = N'Tốt',
    @SerialNumber = N'BP003',
    @NgayMua = '2023-11-20',
    @MayID = 3,
    @LoaiID = 4;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Tai Nghe 01',
    @TinhTrang = N'Tốt',
    @SerialNumber = N'TN001',
    @NgayMua = '2024-01-10',
    @MayID = 1,
    @LoaiID = 5;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Tai Nghe 02',
    @TinhTrang = N'Tốt',
    @SerialNumber = N'TN002',
    @NgayMua = '2024-02-15',
    @MayID = 2,
    @LoaiID = 5;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Tai Nghe 03',
    @TinhTrang = N'Hỏng',
    @SerialNumber = N'TN003',
    @NgayMua = '2023-11-20',
    @MayID = 3,
    @LoaiID = 5;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Ghế Gaming 01',
    @TinhTrang = N'Tốt',
    @SerialNumber = N'GH001',
    @NgayMua = '2024-03-05',
    @MayID = 4,
    @LoaiID = 6;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Ghế Gaming 02',
    @TinhTrang = N'Tốt',
    @SerialNumber = N'GH002',
    @NgayMua = '2024-04-10',
    @MayID = 5,
    @LoaiID = 6;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Webcam 01',
    @TinhTrang = N'Tốt',
    @SerialNumber = N'WC001',
    @NgayMua = '2024-05-10',
    @MayID = 9,
    @LoaiID = 7;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Màn Hình 04',
    @TinhTrang = N'Tốt',
    @SerialNumber = N'MH004',
    @NgayMua = '2024-03-05',
    @MayID = 4,
    @LoaiID = 2;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Chuột 04',
    @TinhTrang = N'Tốt',
    @SerialNumber = N'CH004',
    @NgayMua = '2024-03-05',
    @MayID = 4,
    @LoaiID = 3;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Bàn Phím 04',
    @TinhTrang = N'Tốt',
    @SerialNumber = N'BP004',
    @NgayMua = '2024-03-05',
    @MayID = 4,
    @LoaiID = 4;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Tai Nghe 04',
    @TinhTrang = N'Tốt',
    @SerialNumber = N'TN004',
    @NgayMua = '2024-03-05',
    @MayID = 4,
    @LoaiID = 5;

EXEC sp_AddThietBi 
    @UserID = 1,
    @TenThietBi = N'Màn Hình 05',
    @TinhTrang = N'Tốt',
    @SerialNumber = N'MH005',
    @NgayMua = '2024-04-10',
    @MayID = 5,
    @LoaiID = 2;

-- Verify ThietBi data
SELECT * FROM ThietBi;
GO

/* ======================================================
   DỮ LIỆU MẪU BẢO TRÌ
   ====================================================== */
DELETE FROM BaoTri;
EXEC sp_AddBaoTri 
    @UserID = 1,
    @NgayBaoTri = '2025-08-01 09:00:00',
    @NoiDung = N'Vệ sinh máy tính và kiểm tra CPU',
    @NhanVienPhuTrach = N'Trần Văn Kỹ Thuật',
    @ChiPhi = 500000,
    @MayID = 1,
    @ThietBiID = NULL;

EXEC sp_AddBaoTri 
    @UserID = 1,
    @NgayBaoTri = '2025-08-05 14:00:00',
    @NoiDung = N'Thay bàn phím mới do phím bấm hỏng',
    @NhanVienPhuTrach = N'Trần Văn Kỹ Thuật',
    @ChiPhi = 350000,
    @MayID = NULL,
    @ThietBiID = 8; -- Bàn Phím 02

EXEC sp_AddBaoTri 
    @UserID = 1,
    @NgayBaoTri = '2025-08-10 10:00:00',
    @NoiDung = N'Cập nhật driver GPU và kiểm tra nhiệt độ',
    @NhanVienPhuTrach = N'Lê Thị Kỹ Thuật',
    @ChiPhi = 200000,
    @MayID = 2,
    @ThietBiID = NULL;

EXEC sp_AddBaoTri 
    @UserID = 1,
    @NgayBaoTri = '2025-08-15 15:00:00',
    @NoiDung = N'Sửa màn hình bị lỗi hiển thị',
    @NhanVienPhuTrach = N'Trần Văn Kỹ Thuật',
    @ChiPhi = 800000,
    @MayID = NULL,
    @ThietBiID = 3; -- Màn Hình 03

EXEC sp_AddBaoTri 
    @UserID = 1,
    @NgayBaoTri = '2025-08-20 11:00:00',
    @NoiDung = N'Nâng cấp RAM từ 8GB lên 16GB',
    @NhanVienPhuTrach = N'Lê Thị Kỹ Thuật',
    @ChiPhi = 1200000,
    @MayID = 6,
    @ThietBiID = NULL;

EXEC sp_AddBaoTri 
    @UserID = 1,
    @NgayBaoTri = '2025-08-25 13:00:00',
    @NoiDung = N'Thay tai nghe do micro hỏng',
    @NhanVienPhuTrach = N'Trần Văn Kỹ Thuật',
    @ChiPhi = 450000,
    @MayID = NULL,
    @ThietBiID = 12; -- Tai Nghe 03

EXEC sp_AddBaoTri 
    @UserID = 1,
    @NgayBaoTri = '2025-09-01 09:00:00',
    @NoiDung = N'Kiểm tra và vệ sinh quạt làm mát',
    @NhanVienPhuTrach = N'Lê Thị Kỹ Thuật',
    @ChiPhi = 300000,
    @MayID = 7,
    @ThietBiID = NULL;

EXEC sp_AddBaoTri 
    @UserID = 1,
    @NgayBaoTri = '2025-09-05 14:00:00',
    @NoiDung = N'Cài đặt lại hệ điều hành Windows',
    @NhanVienPhuTrach = N'Trần Văn Kỹ Thuật',
    @ChiPhi = 400000,
    @MayID = 8,
    @ThietBiID = NULL;

EXEC sp_AddBaoTri 
    @UserID = 1,
    @NgayBaoTri = '2025-09-10 10:00:00',
    @NoiDung = N'Kiểm tra webcam bị lỗi kết nối',
    @NhanVienPhuTrach = N'Lê Thị Kỹ Thuật',
    @ChiPhi = 250000,
    @MayID = NULL,
    @ThietBiID = 15; -- Webcam 01

EXEC sp_AddBaoTri 
    @UserID = 1,
    @NgayBaoTri = '2025-09-15 15:00:00',
    @NoiDung = N'Thay ghế gaming do hỏng bánh xe',
    @NhanVienPhuTrach = N'Trần Văn Kỹ Thuật',
    @ChiPhi = 600000,
    @MayID = NULL,
    @ThietBiID = 13; -- Ghế Gaming 01

-- Verify BaoTri data
SELECT * FROM BaoTri;
GO