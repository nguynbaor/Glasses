-- =================================================================
-- 1. TẠO CƠ SỞ DỮ LIỆU VÀ CÁC BẢNG
-- =================================================================
CREATE DATABASE QuanLyBanKinh;
GO 

USE QuanLyBanKinh;
GO

CREATE TABLE NguoiDung (
    maNguoiDung VARCHAR(50) PRIMARY KEY,
    matKhau VARCHAR(255) NOT NULL,
    hoVaTen NVARCHAR(100) NOT NULL,
    soDienThoai VARCHAR(15) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    diaChi NVARCHAR(255),
    phanQuyen INT NOT NULL
);

CREATE TABLE KhachHang (
    maKhachHang VARCHAR(50) PRIMARY KEY, 
    FOREIGN KEY (maKhachHang) REFERENCES NguoiDung(maNguoiDung)
);

CREATE TABLE QuanLy (
    maQuanLy VARCHAR(50) PRIMARY KEY,
    FOREIGN KEY (maQuanLy) REFERENCES NguoiDung(maNguoiDung)
);

CREATE TABLE NhanVien (
    maNhanVien VARCHAR(50) PRIMARY KEY,
    FOREIGN KEY (maNhanVien) REFERENCES NguoiDung(maNguoiDung)
);

CREATE TABLE DanhMuc (
    maDanhMuc VARCHAR(50) PRIMARY KEY,
    tenDanhMuc NVARCHAR(100) NOT NULL
);

CREATE TABLE ThuongHieu (
    maThuongHieu VARCHAR(50) PRIMARY KEY,
    tenThuongHieu NVARCHAR(100) NOT NULL
);

CREATE TABLE SanPham (
    maSanPham VARCHAR(50) PRIMARY KEY,
    maDanhMuc VARCHAR(50),
    maThuongHieu VARCHAR(50),
    tenSanPham NVARCHAR(255) NOT NULL,
    thongTin NVARCHAR(MAX),
    gia DECIMAL(18,2) NOT NULL,
    phanTramGiam INT DEFAULT 0,       
    giaDaGiam DECIMAL(18,2) NOT NULL,
    chatLieu NVARCHAR(100),
    HinhAnh NVARCHAR(255),
    FOREIGN KEY (maDanhMuc) REFERENCES DanhMuc(maDanhMuc),
    FOREIGN KEY (maThuongHieu) REFERENCES ThuongHieu(maThuongHieu)
);

CREATE TABLE KinhRam (
    maSanPham VARCHAR(50) PRIMARY KEY,
    doiTuong NVARCHAR(100),
    doChongUV NVARCHAR(50),
    FOREIGN KEY (maSanPham) REFERENCES SanPham(maSanPham)
);

CREATE TABLE KinhCan (
    maSanPham VARCHAR(50) PRIMARY KEY,
    chatLieu NVARCHAR(100),
    kieuDang NVARCHAR(100),
    FOREIGN KEY (maSanPham) REFERENCES SanPham(maSanPham)
);

CREATE TABLE TrongKinh (
    maSanPham VARCHAR(50) PRIMARY KEY,
    loaiTrong NVARCHAR(100),
    thuongHieu NVARCHAR(100),
    chiSoKhucXa DECIMAL(4,2),
    FOREIGN KEY (maSanPham) REFERENCES SanPham(maSanPham)
);

CREATE TABLE GongKinh (
    maSanPham VARCHAR(50) PRIMARY KEY,
    mauGong NVARCHAR(100),
    chatLieu NVARCHAR(100),
    kieuDang NVARCHAR(100),
    FOREIGN KEY (maSanPham) REFERENCES SanPham(maSanPham)
);

CREATE TABLE BienThe (
    maBienThe VARCHAR(50) PRIMARY KEY,
    maSanPham VARCHAR(50) NOT NULL,
    mauSac NVARCHAR(50),
    hinhAnh VARCHAR(255),
    soLuongTon INT NOT NULL DEFAULT 0,
    FOREIGN KEY (maSanPham) REFERENCES SanPham(maSanPham)
);

CREATE TABLE GioHang (
    maGioHang VARCHAR(50) PRIMARY KEY,
    maKhachHang VARCHAR(50) NOT NULL,
    tongTien DECIMAL(18,2) DEFAULT 0,
    FOREIGN KEY (maKhachHang) REFERENCES KhachHang(maKhachHang)
);

CREATE TABLE ChiTietGioHang (
    maGioHang VARCHAR(50),
    maBienThe VARCHAR(50),
    soLuong INT NOT NULL,
    PRIMARY KEY (maGioHang, maBienThe),
    FOREIGN KEY (maGioHang) REFERENCES GioHang(maGioHang),
    FOREIGN KEY (maBienThe) REFERENCES BienThe(maBienThe)
);

CREATE TABLE DonHang (
    maDonHang VARCHAR(50) PRIMARY KEY,
    maKhachHang VARCHAR(50) NOT NULL,
    ngayDat DATETIME NOT NULL,
    trangThai NVARCHAR(50) NOT NULL,
    tongTien DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (maKhachHang) REFERENCES KhachHang(maKhachHang)
);

CREATE TABLE ChiTietDonHang (
    maDonHang VARCHAR(50),
    maBienThe VARCHAR(50),
    donGia DECIMAL(18,2) NOT NULL,
    soLuong INT NOT NULL,
    tongTien DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (maDonHang, maBienThe),
    FOREIGN KEY (maDonHang) REFERENCES DonHang(maDonHang),
    FOREIGN KEY (maBienThe) REFERENCES BienThe(maBienThe)
);

CREATE TABLE ThanhToan (
    maThanhToan VARCHAR(50) PRIMARY KEY,
    maDonHang VARCHAR(50) NOT NULL,
    hinhThuc NVARCHAR(50),
    soTien DECIMAL(18,2) NOT NULL,
    ngayThanhToan DATETIME,
    trangThai NVARCHAR(50),
    FOREIGN KEY (maDonHang) REFERENCES DonHang(maDonHang)
);

CREATE TABLE BaoCaoVaThongKe (
    maBaoCao VARCHAR(50) PRIMARY KEY,
    thoiGian DATE NOT NULL,
    tongDoanhThu DECIMAL(18,2) NOT NULL,
    soLuongDonHang INT NOT NULL,
    maQuanLy VARCHAR(50) NOT NULL, 
    FOREIGN KEY (maQuanLy) REFERENCES QuanLy(maQuanLy)
);

CREATE TABLE ChiTietBaoCao_SanPham (
    maBaoCao VARCHAR(50),
    maSanPham VARCHAR(50),
    PRIMARY KEY (maBaoCao, maSanPham),
    FOREIGN KEY (maBaoCao) REFERENCES BaoCaoVaThongKe(maBaoCao),
    FOREIGN KEY (maSanPham) REFERENCES SanPham(maSanPham)
);
GO

-- =================================================================
-- 2. NHẬP DỮ LIỆU MẪU CHUẨN XÁC
-- =================================================================

-- 2.1. Danh mục
INSERT INTO DanhMuc (maDanhMuc, tenDanhMuc) VALUES
('DM01', N'Kính Cận'),
('DM02', N'Kính Râm'),
('DM03', N'Tròng Kính');
GO

-- 2.2. Thương hiệu
INSERT INTO ThuongHieu (maThuongHieu, tenThuongHieu) VALUES
('TH01', N'RayBan'),
('TH02', N'Gentle Monster'),
('TH03', N'OEM'),
('TH04', N'Gucci'),
('TH05', N'Oakley'),
('TH06', N'Bolon'),
('TH07', N'Parim'),
('TH08', N'Exfash'),
('TH09', N'Essilor'),
('TH10', N'Zeiss');
GO

-- 2.3. Người dùng
INSERT INTO NguoiDung (maNguoiDung, matKhau, hoVaTen, soDienThoai, email, diaChi, phanQuyen) VALUES
('KH01', 'hashed_pwd_123', N'Nguyễn Văn An',  '0901234567', 'an.nguyen@email.com',  N'Quận 1, TP.HCM',    3),
('KH02', 'hashed_pwd_123', N'Trần Thị Bích',  '0912345678', 'bich.tran@email.com',  N'Biên Hòa, Đồng Nai', 3),
('KH03', 'hashed_pwd_123', N'Lê Minh Khang',  '0923456789', 'khang.le@email.com',   N'Quận 3, TP.HCM',    3),
('KH04', 'hashed_pwd_456', N'Phạm Thùy Linh', '0934567890', 'linh.pham@email.com',  N'Gò Vấp, TP.HCM',    3),
('KH05', 'hashed_pwd_789', N'Đỗ Hoàng Nam',   '0945678901', 'nam.do@email.com',     N'Thủ Đức, TP.HCM',   3),
('KH06', 'hashed_pwd_abc', N'Nguyễn Ngọc Mai', '0956789012', 'mai.nguyen@email.com', N'Hải Châu, Đà Nẵng',  3);
GO

-- 2.4. Khách hàng
INSERT INTO KhachHang (maKhachHang) VALUES
('KH01'), ('KH02'), ('KH03'), ('KH04'), ('KH05'), ('KH06');
GO

-- 2.5. Sản phẩm (Bao gồm SP01-SP03, SP11-SP13 và SP14-SP33)
INSERT INTO SanPham (maSanPham, maDanhMuc, maThuongHieu, tenSanPham, thongTin, gia, phanTramGiam, giaDaGiam, chatLieu, HinhAnh) VALUES
-- Nhóm mẫu ban đầu
('SP01', 'DM02', 'TH01', N'RayBan Unisex – Gọng kính Kim loại – Chống tia UV', N'Chống tia UV', 5190000, 0, 5190000, N'Kim loại', N'https://kinhhaitrieu.com/wp-content/uploads/2026/07/kinh-rayban-0rb3774d-003-87-55-2.jpg'),
('SP02', 'DM02', 'TH01', N'RayBan Unisex – Gọng kính kim loại – Chống chói Polarized, chống UV', N'Chống tia UV, chống chói Polarized', 6490000, 0, 6490000, N'Kim loại', N'https://kinhhaitrieu.com/wp-content/uploads/2026/07/kinh-rayban-0rb3774d-001-9a-55-2.jpg'),
('SP03', 'DM01', 'TH03', N'Kính cận mắt mèo thời trang', N'Kiểu dáng thanh lịch, trẻ trung', 650000, 0, 650000, N'Nhựa TR90', N'kinh-can-mat-meo.jpg'),
('SP11', 'DM02', 'TH01', N'Kính râm RayBan Aviator', N'Kính phi công', 3500000, 10, 3150000, N'Hợp kim', N'rayban-aviator.jpg'),
('SP12', 'DM02', 'TH01', N'Kính râm RayBan Wayfarer', N'Kính râm thời trang chống tia UV', 2800000, 20, 2240000, N'Nhựa cao cấp', N'rayban-wayfarer.jpg'),
('SP13', 'DM02', 'TH02', N'Kính râm Gentle Monster Square', N'Thiết kế cá tính, phong cách hiện đại', 4200000, 5, 3990000, N'Acetate', N'gentle-monster-square.jpg'),

-- Nhóm Gọng Kính / Kính Cận mới (DM01)
('SP14', 'DM01', 'TH04', N'Gọng kính Gucci Square Acetate', N'Thiết kế sang trọng, logo Gucci nổi bật', 8500000, 10, 7650000, N'Acetate', N'gucci-square.jpg'),
('SP15', 'DM01', 'TH04', N'Gọng kính Gucci Kim Loại Vuông', N'Khung kim loại mạ vàng cao cấp', 9200000, 5, 8740000, N'Kim loại', N'gucci-metal.jpg'),
('SP16', 'DM01', 'TH05', N'Gọng kính thể thao Oakley Pitchman R', N'Chất liệu O Matter siêu nhẹ, bám tốt', 4100000, 0, 4100000, N'O Matter', N'oakley-pitchman.jpg'),
('SP17', 'DM01', 'TH05', N'Gọng kính Oakley Holbrook RX', N'Kiểu dáng thể thao năng động', 3800000, 10, 3420000, N'Nhựa cao cấp', N'oakley-holbrook.jpg'),
('SP18', 'DM01', 'TH06', N'Gọng kính Bolon Titanium Ultralight', N'Titanium siêu nhẹ, không gỉ', 3200000, 15, 2720000, N'Titanium', N'bolon-titanium.jpg'),
('SP19', 'DM01', 'TH06', N'Gọng kính Bolon Dáng Lục Giác', N'Thiết kế thời trang Ý hiện đại', 2900000, 10, 2610000, N'Kim loại', N'bolon-hexagon.jpg'),
('SP20', 'DM01', 'TH07', N'Gọng kính Parim Nhựa TR90 Dẻo', N'Nhựa TR90 siêu dẻo, chống gãy', 1250000, 20, 1000000, N'Nhựa TR90', N'parim-tr90.jpg'),
('SP21', 'DM01', 'TH07', N'Gọng kính Parim Mắt Mèo Nữ', N'Dáng mắt mèo quyến rũ cho nữ', 1400000, 10, 1260000, N'Nhựa Acetate', N'parim-cat-eye.jpg'),
('SP22', 'DM01', 'TH08', N'Gọng kính Exfash Hàn Quốc Vành Tròn', N'Phong cách K-Pop trẻ trung', 980000, 10, 882000, N'Kim loại', N'exfash-round.jpg'),
('SP23', 'DM01', 'TH08', N'Gọng kính Exfash Titan Bán Kim Loại', N'Gọng xẻ cước thanh lịch công sở', 1350000, 0, 1350000, N'Titanium', N'exfash-titan.jpg'),

-- Nhóm Kính Râm mới (DM02)
('SP24', 'DM02', 'TH04', N'Kính râm Gucci Aviator Gold', N'Tròng kính chống UV400, gọng mạ vàng', 10500000, 10, 9450000, N'Kim loại', N'gucci-aviator.jpg'),
('SP25', 'DM02', 'TH05', N'Kính râm thể thao Oakley Frogskins', N'Tròng Prizm tăng cường phản xạ màu', 4600000, 5, 4370000, N'O Matter', N'oakley-frogskins.jpg'),
('SP26', 'DM02', 'TH06', N'Kính râm Bolon Cat-Eye Polarized', N'Tròng phân cực chống chói hiệu quả', 3500000, 10, 3150000, N'Acetate', N'bolon-polarized.jpg'),
('SP27', 'DM02', 'TH07', N'Kính râm Parim Mắt Vuông Nam', N'Chống tia UV400, kiểu dáng mạnh mẽ', 1650000, 15, 1402500, N'Nhựa TR90', N'parim-sunglasses.jpg'),

-- Nhóm Tròng Kính mới (DM03)
('SP28', 'DM03', 'TH09', N'Tròng kính Essilor Crizal Rock 1.56', N'Chống trầy xước gấp 3 lần, chống bám bẩn', 1280000, 0, 1280000, N'Polycarbonate', N'essilor-crizal-rock.jpg'),
('SP29', 'DM03', 'TH09', N'Tròng kính Essilor Elements Blue UV 1.60', N'Lọc ánh sáng xanh thông minh từ máy tính', 1680000, 10, 1512000, N'High Index Resin', N'essilor-elements.jpg'),
('SP30', 'DM03', 'TH09', N'Tròng kính đổi màu Essilor Transitions Classic 1.56', N'Đổi màu nhanh khi ra nắng, trong suốt trong nhà', 3380000, 5, 3211000, N'Photochromic Resin', N'essilor-transitions.jpg'),
('SP31', 'DM03', 'TH10', N'Tròng kính Zeiss ClearView FSV 1.60', N'Công nghệ đúc điểm ảnh cho tầm nhìn sắc nét', 2480000, 0, 2480000, N'Organic Resin', N'zeiss-clearview.jpg'),
('SP32', 'DM03', 'TH10', N'Tròng kính Zeiss BlueGuard 1.67 Mỏng', N'Chống ánh sáng xanh thế hệ mới, tròng siêu mỏng', 4280000, 10, 3852000, N'High Index 1.67', N'zeiss-blueguard.jpg'),
('SP33', 'DM03', 'TH10', N'Tròng kính Zeiss DriveSafe 1.50', N'Chuyên dụng lái xe ban đêm và trời mưa', 4980000, 5, 4731000, N'Organic Resin', N'zeiss-drivesafe.jpg');
GO

-- 2.6. Kính Cận
INSERT INTO KinhCan (maSanPham, chatLieu, kieuDang) VALUES
('SP01', N'Nhựa TR90', N'Tròn'),
('SP02', N'Nhựa Acetate', N'Vuông'),
('SP03', N'Nhựa TR90', N'Mắt mèo'),
('SP14', N'Acetate', N'Vuông'),
('SP15', N'Kim loại', N'Vuông'),
('SP16', N'O Matter', N'Tròn'),
('SP17', N'Nhựa cao cấp', N'Vuông'),
('SP18', N'Titanium', N'Lục giác'),
('SP19', N'Kim loại', N'Lục giác'),
('SP20', N'Nhựa TR90', N'Chữ nhật'),
('SP21', N'Nhựa Acetate', N'Mắt mèo'),
('SP22', N'Kim loại', N'Tròn'),
('SP23', N'Titanium', N'Nửa khung');

-- 2.7. Kính Râm
INSERT INTO KinhRam (maSanPham, doiTuong, doChongUV) VALUES
('SP11', N'Unisex', N'UV400'),
('SP12', N'Unisex', N'UV400'),
('SP13', N'Nam và nữ', N'UV400'),
('SP24', N'Unisex', N'UV400'),
('SP25', N'Nam', N'UV400 & Polarized'),
('SP26', N'Nữ', N'UV400 & Polarized'),
('SP27', N'Nam', N'UV400');

-- 2.8. Tròng Kính
INSERT INTO TrongKinh (maSanPham, loaiTrong, thuongHieu, chiSoKhucXa) VALUES
('SP28', N'Chống trầy & Chống chói', N'Essilor', 1.56),
('SP29', N'Chống ánh sáng xanh', N'Essilor', 1.60),
('SP30', N'Đổi màu khi ra nắng', N'Essilor', 1.56),
('SP31', N'Tròng đơn siêu nét', N'Zeiss', 1.60),
('SP32', N'Chống ánh sáng xanh mỏng', N'Zeiss', 1.67),
('SP33', N'Chuyên dụng lái xe', N'Zeiss', 1.50);
GO

-- 2.9. Biến thể sản phẩm
INSERT INTO BienThe (maBienThe, maSanPham, mauSac, hinhAnh, soLuongTon) VALUES
-- Biến thể cũ
('SKU_SP01_DEN',   'SP01', N'Đen',        'kinh-can-den.jpg',               50),
('SKU_SP01_TRANG', 'SP01', N'Trắng trong', 'kinh-can-trang.jpg',             30),
('SKU_SP02_DEN',   'SP02', N'Đen',        'kinh-can-vuong-den.jpg',         25),
('SKU_SP02_NAU',   'SP02', N'Nâu',        'kinh-can-vuong-nau.jpg',         20),
('SKU_SP03_DEN',   'SP03', N'Đen',        'kinh-can-mat-meo-den.jpg',       18),
('SKU_SP03_DO',    'SP03', N'Đỏ',         'kinh-can-mat-meo-do.jpg',        12),
('SKU_SP11_VANG',  'SP11', N'Gọng vàng',  'rayban-vang.jpg',                15),
('SKU_SP12_DEN',   'SP12', N'Đen',        'rayban-wayfarer-den.jpg',        10),
('SKU_SP12_NAU',   'SP12', N'Nâu',        'rayban-wayfarer-nau.jpg',        8),
('SKU_SP13_DEN',   'SP13', N'Đen',        'gentle-monster-square-den.jpg',   7),
('SKU_SP13_TRANG', 'SP13', N'Trắng',      'gentle-monster-square-trang.jpg', 5),

-- Biến thể Gucci
('SKU_SP14_DEN', 'SP14', N'Đen bóng', 'gucci-square-black.jpg', 15),
('SKU_SP14_DOIHOI', 'SP14', N'Đồi mồi', 'gucci-square-tortoise.jpg', 10),
('SKU_SP15_VANG', 'SP15', N'Gọng vàng', 'gucci-metal-gold.jpg', 8),
('SKU_SP24_KHONG', 'SP24', N'Tròng đen gọng vàng', 'gucci-aviator-gold.jpg', 6),

-- Biến thể Oakley
('SKU_SP16_DEN', 'SP16', N'Đen nhám', 'oakley-pitchman-black.jpg', 20),
('SKU_SP17_XAM', 'SP17', N'Xám trong', 'oakley-holbrook-grey.jpg', 12),
('SKU_SP25_XANH', 'SP25', N'Tròng gương xanh', 'oakley-frogskins-blue.jpg', 14),

-- Biến thể Bolon
('SKU_SP18_BACC', 'SP18', N'Bạc Titanium', 'bolon-titanium-silver.jpg', 25),
('SKU_SP19_VANGHONG', 'SP19', N'Vàng hồng', 'bolon-hexagon-rosegold.jpg', 18),
('SKU_SP26_DEN', 'SP26', N'Đen huyền', 'bolon-cat-black.jpg', 10),

-- Biến thể Parim & Exfash
('SKU_SP20_XANHDEN', 'SP20', N'Xanh đen', 'parim-tr90-blue.jpg', 30),
('SKU_SP21_HONG', 'SP21', N'Hồng trong', 'parim-cat-pink.jpg', 15),
('SKU_SP22_VANG', 'SP22', N'Gọng vàng mảnh', 'exfash-round-gold.jpg', 22),
('SKU_SP23_DEN', 'SP23', N'Đen công sở', 'exfash-titan-black.jpg', 16),
('SKU_SP27_NAU', 'SP27', N'Nâu trà', 'parim-sunglasses-brown.jpg', 18),

-- Biến thể Tròng Kính Essilor & Zeiss
('SKU_SP28_STD', 'SP28', N'Trong suốt', 'essilor-crizal-rock.jpg', 100),
('SKU_SP29_STD', 'SP29', N'Trong suốt (Ánh tím)', 'essilor-elements.jpg', 80),
('SKU_SP30_KHOMAU', 'SP30', N'Đổi màu Khói (Grey)', 'essilor-transitions-grey.jpg', 50),
('SKU_SP31_STD', 'SP31', N'Trong suốt', 'zeiss-clearview.jpg', 60),
('SKU_SP32_STD', 'SP32', N'Trong suốt', 'zeiss-blueguard.jpg', 40),
('SKU_SP33_STD', 'SP33', N'Trong suốt (Phản quang)', 'zeiss-drivesafe.jpg', 35);
GO

-- 2.10. Đơn hàng
INSERT INTO DonHang (maDonHang, maKhachHang, ngayDat, trangThai, tongTien) VALUES
('DH01', 'KH01', '2026-10-01', N'Đã giao',        315000),
('DH02', 'KH02', '2026-10-02', N'Đang giao',      3150000),
('DH03', 'KH03', '2026-10-03', N'Đã giao',        816000),
('DH04', 'KH04', '2026-10-04', N'Đang xử lý',     2240000),
('DH05', 'KH05', '2026-10-05', N'Đang giao',      4416000),
('DH06', 'KH06', '2026-10-06', N'Chờ thanh toán', 650000);
GO

-- 2.11. Chi tiết đơn hàng
INSERT INTO ChiTietDonHang (maDonHang, maBienThe, donGia, soLuong, tongTien) VALUES
('DH01', 'SKU_SP01_DEN',  315000,  1, 315000),
('DH02', 'SKU_SP11_VANG', 3150000, 1, 3150000),
('DH03', 'SKU_SP02_DEN',  408000,  1, 408000),
('DH03', 'SKU_SP02_NAU',  408000,  1, 408000),
('DH04', 'SKU_SP12_DEN',  2240000, 1, 2240000),
('DH05', 'SKU_SP12_NAU',  2240000, 1, 2240000),
('DH05', 'SKU_SP13_DEN',  3990000, 1, 3990000),
('DH06', 'SKU_SP03_DEN',  650000,  1, 650000);
GO