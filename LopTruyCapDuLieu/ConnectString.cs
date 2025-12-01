using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows; // Cần thiết cho MessageBox khi ghi file

namespace QuanLiSanCauLong.LopTruyCapDuLieu
{
    public class ConnectStringDAL
    {
        // 1. Singleton Instance: Thể hiện tĩnh, duy nhất để truy cập.
        private static ConnectStringDAL _instance;
        public static ConnectStringDAL Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConnectStringDAL();
                }
                return _instance;
            }
        }

        // 2. Cấu hình Persistence
        private const string CONFIG_FILE_NAME = "connection.config";
        private const string DEFAULT_CONNECTION_STRING =
            "Data Source=localhost;Initial Catalog=QuanLiSanCauLong;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        private string _connectionString;

        // 3. Constructor Private: Ngăn tạo instance từ bên ngoài
        private ConnectStringDAL()
        {
            // Tải chuỗi kết nối từ file khi instance được tạo lần đầu
            _connectionString = LoadConnectionStringFromFile() ?? DEFAULT_CONNECTION_STRING;
        }

        // Lấy đường dẫn file cấu hình (trong thư mục chạy ứng dụng)
        private string ConfigFilePath
        {
            get
            {
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                return Path.Combine(appDirectory, CONFIG_FILE_NAME);
            }
        }

        // --- Logic Đọc/Ghi File ---

        private string LoadConnectionStringFromFile()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    return File.ReadAllText(ConfigFilePath).Trim();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi đọc file cấu hình: {ex.Message}");
            }
            return null;
        }

        private void SaveConnectionStringToFile(string connectionString)
        {
            try
            {
                File.WriteAllText(ConfigFilePath, connectionString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi ghi file cấu hình: {ex.Message}");
                MessageBox.Show($"Lỗi lưu cấu hình: Không thể ghi vào file {CONFIG_FILE_NAME}.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- Phương thức Public ---

        public string GetConnectionString()
        {
            return _connectionString;
        }

        public void SetConnectionString(string newConnectionString)
        {
            _connectionString = newConnectionString;
            SaveConnectionStringToFile(newConnectionString); // Ghi vào file
        }

        public bool CheckConnection(string connectionStringToCheck)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStringToCheck))
                {
                    // Đặt timeout ngắn để kiểm tra nhanh
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool InitializeDatabase()
        {
            if (!CheckConnection(_connectionString))
            {
                // Không thể khởi tạo nếu không kết nối được
                MessageBox.Show("Không thể kết nối đến CSDL để khởi tạo schema.", "Lỗi khởi tạo", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // 1. KIỂM TRA SỰ TỒN TẠI CỦA BẢNG QUAN TRỌNG (ví dụ: San)
                    string checkTableSql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'San'";
                    using (SqlCommand checkCmd = new SqlCommand(checkTableSql, connection))
                    {
                        int tableCount = (int)checkCmd.ExecuteScalar();

                        // Nếu bảng đã tồn tại, coi như schema đã được khởi tạo
                        if (tableCount > 0)
                        {
                            Console.WriteLine("Schema đã tồn tại.");
                            return true;
                        }
                    }

                    // 2. KHỞI TẠO SCHEMA NẾU CHƯA TỒN TẠI
                    MessageBoxResult confirmResult = MessageBox.Show(
                        "Database trống. Bạn có muốn chạy script tạo bảng và dữ liệu mặc định không?",
                        "Khởi tạo Schema",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (confirmResult == MessageBoxResult.No)
                    {
                        return false; // Người dùng không đồng ý khởi tạo
                    }

                    // Thực thi toàn bộ script tạo bảng và dữ liệu
                    using (SqlCommand createCmd = new SqlCommand(SQL_SCHEMA_SCRIPT, connection))
                    {
                        // Thay đổi CommandTimeout vì script SQL có thể chạy lâu hơn mặc định
                        createCmd.CommandTimeout = 60;

                        // Sử dụng ExecuteNonQuery vì không trả về dữ liệu
                        createCmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Khởi tạo schema database thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi trong quá trình khởi tạo database:\n{ex.Message}", "Lỗi SQL", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // --- CHUỖI SCRIPT SQL CỦA BẠN ---
        private const string SQL_SCHEMA_SCRIPT = @"
        
        -- Bỏ qua phần tạo/sử dụng database vì đã có ở phần CheckConnection/Initialize
        
        ------------------------------------------------------------
        -- 1) Core: Sân & Giá sân & Bảo trì
        ------------------------------------------------------------
        
        CREATE TABLE dbo.San (
          MaSan        INT IDENTITY(1,1) PRIMARY KEY,
          TenSan       NVARCHAR(100) NOT NULL UNIQUE,
          TrangThai NVARCHAR(20) NOT NULL,
          NgayBaoTri DATE NULL
        );
        
        
        CREATE TABLE BangGiaChung (
            MaBangGia INT IDENTITY(1,1) PRIMARY KEY,
            GioBatDau TIME NOT NULL,
            GioKetThuc TIME NOT NULL,
            DonGia DECIMAL(18,2) NOT NULL,
            LoaiNgay NVARCHAR(20) NOT NULL CHECK (LoaiNgay IN (N'Thứ 2-Thứ 6', N'Cuối Tuần')),
            PhuThuLePercent DECIMAL(5,2) DEFAULT 0
        );
        
        
        
        ------------------------------------------------------------
        -- 2) Khách hàng & CRM
        ------------------------------------------------------------
        
        CREATE TABLE KhachHang (
            SDT NVARCHAR(15) PRIMARY KEY,
            Ten NVARCHAR(100) NOT NULL,
            Email NVARCHAR(100),
            LuotChoi INT DEFAULT 0,
            TongChiTieu DECIMAL(18, 2) DEFAULT 0,
            TuNgay DATE DEFAULT GETDATE(),
            DiemTichLuy AS (CAST(TongChiTieu / 1000 AS INT)) PERSISTED
        );
        ALTER TABLE KhachHang
        ADD SDTPhu NVARCHAR(15) NULL;
        
        
        
        
        ------------------------------------------------------------
        -- 3) Đặt sân & Đồng bộ lịch
        ------------------------------------------------------------
        
        
        CREATE TABLE DatSan (
            MaPhieu NVARCHAR(20) PRIMARY KEY,
            SDT NVARCHAR(15) NOT NULL,
            NgayTao DATE DEFAULT GETDATE(),
            TongTien DECIMAL(18,2) DEFAULT 0,
            FOREIGN KEY (SDT) REFERENCES KhachHang(SDT) ON UPDATE CASCADE 
        );
        
        CREATE TABLE ChiTietDatSan (
            MaChiTiet NVARCHAR(25) PRIMARY KEY,
            MaPhieu NVARCHAR(20) NOT NULL,
            MaSan INT NOT NULL,
            TenSanCached NVARCHAR(100) NOT NULL,
            NgayDat DATE NOT NULL,
            GioBatDau TIME NOT NULL,
            GioKetThuc TIME NOT NULL,
            DonGia DECIMAL(18,2) NOT NULL,
            PhuThuLe DECIMAL(5,2) DEFAULT 0,
            ThanhTien DECIMAL(18,2),
            FOREIGN KEY (MaPhieu) REFERENCES DatSan(MaPhieu) ON UPDATE CASCADE ON DELETE CASCADE,
            FOREIGN KEY (MaSan) REFERENCES San(MaSan) ON UPDATE CASCADE
        );
        ALTER TABLE ChiTietDatSan
        ADD TrangThai NVARCHAR(50);
        ALTER TABLE ChiTietDatSan
        ADD CONSTRAINT DF_ChiTietDatSan_TrangThai DEFAULT N'Chưa bắt đầu' FOR TrangThai;
        ALTER TABLE ChiTietDatSan
        ADD TrangThaiThanhToan NVARCHAR(50);
        ALTER TABLE ChiTietDatSan
        ADD CONSTRAINT DF_ChiTietDatSan_TrangThaiThanhToan DEFAULT N'Chưa thanh toán' FOR TrangThaiThanhToan;
        
        
        
        ------------------------------------------------------------
        -- 4) Thanh Toán
        ------------------------------------------------------------
        Create table ThanhToan(
            SoHD Nvarchar(15) Primary key,
            SDT NVARCHAR(15),
            TenKH Nvarchar(100),
            NgayLap Date default getdate(),
            TongTienSan decimal(18,2),
            TongTienThueVot decimal(18,2),
            TongTien decimal (18,2),
            PhuongThuc Nvarchar(20),
            FOREIGN KEY (SDT) REFERENCES KhachHang(SDT) ON UPDATE CASCADE 
        );
        
        
        
        
        
        
        ------------------------------------------------------------
        -- 4) Kho & Nhập hàng
        ------------------------------------------------------------
        create table PhieuNhap(
            SoPhieu nvarchar(15) primary key,
            NgayNhap date default getdate(),
            GhiChu nvarchar(100),
            TongTien decimal(18,2)
        
        );
        alter table PhieuNhap Add NhaCungCap nvarchar(50)
        create table HangHoa(
            MaHang nvarchar(15) primary key,
            TenHang nvarchar(50),
            DVT nvarchar(20),
            TonKho int,
            GiaNhap decimal(18,2),
            GiaBan decimal(18,2),
            LanCuoiNhap date,
            TrangThai nvarchar(20),
        )
        
        
        create table ChiTietPhieuNhap(
            MaChiTiet nvarchar(20) primary key,
            MaHang nvarchar(15),
            TenHang nvarchar(50),
            DVT nvarchar(20),
            SoLuong int,
            GiaNhap decimal(18,2),
            ChietKhau decimal(5,2),
            ChietKhauTien decimal(18,2),
            VAT decimal(5,2) default 5,
            SoLo nvarchar(20),
            HSD date,
            ThanhTien decimal(18,2),
            SoPhieu nvarchar(15),
            foreign key(SoPhieu) references PhieuNhap(SoPhieu) on delete cascade
        );
        alter table ChiTietPhieuNhap add constraint fk_chitiet_hanghoa foreign key(MaHang) references HangHoa(MaHang);
        
        ------------------------------------------------------------
        -- 5) Hóa đơn bán hàng
        ------------------------------------------------------------
        create table HoaDon(
            SoHDN nvarchar(15) primary key,
            Ngay Datetime,
            PhuongThuc nvarchar(20),
            TongTien decimal(18,2),
        );
        
        
        create table ChiTietHoaDon(
            MaChiTiet nvarChar(20),
            MaHang nvarchar(15),
            TenHang nvarchar(50),
            SoLuong int,
            DVT nvarchar(20),
            GiaBan decimal(18,2),
            ThanhTien decimal(18,2),
            SoHDN nvarchar(15),
        );
        Alter table ChiTietHoaDon add CONSTRAINT FK_CHITIETHOADON_HANGHOA foreign key(MaHang) references HangHoa(MaHang);
        
        ------------------------------------------------------------
        -- 6) Nhân Viên & Cấu hình
        ------------------------------------------------------------
        
        Create table NhanVien(
            MaNV nvarchar(20) PRIMARY KEY, -- Thêm PRIMARY KEY
            TenNV nvarchar(50),
            SDT nvarchar(20),
            Email nvarchar(50),
            Username nvarchar(50),
            PasswordHash nvarchar(100),
            VaiTro  nvarchar(20),
            NgayVaoLam Date default GetDate(),
            TrangThai  nvarchar(50),
            GhiChu NvarChar(200)
        );
        
        -- Lưu ý: Tôi đã thêm PRIMARY KEY cho NhanVien để nó khớp với các quy tắc thiết kế database.
        
        INSERT INTO NhanVien (MaNV, TenNV, SDT, Email, Username, PasswordHash, VaiTro, NgayVaoLam, TrangThai, GhiChu) VALUES
        (N'ADMIN', 'Admin', N'0123456789', 'admin@email.com', N'admin', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Admin', '2024-05-01', N'Đang làm', N'Admin');
        -- (PasswordHash là SHA256 của '123' cho dễ kiểm tra)
        
        
        CREATE TABLE CauHinhHeThong (
            TenThamSo NVARCHAR(50) PRIMARY KEY,
            GiaTriThamSo NVARCHAR(255) NOT NULL,
            MoTa NVARCHAR(100) NULL
        );
        
        INSERT INTO CauHinhHeThong (TenThamSo, GiaTriThamSo, MoTa)
        VALUES
        ('CanhBaoNoShow', '15', N'Cảnh báo No-show trước (phút)'),
        ('SoSanToiDa', '5', N'Số sân tối đa cho một lần đặt'),
        ('SoSlotToiDa', '10', N'Số slot tối đa cho một lần đặt'),
        ('TimeoutPhien', '30', N'Timeout phiên đăng nhập (phút)'),
        ('NguongTonKhoThap', '20', N'Ngưỡng cảnh báo tồn kho thấp (đơn vị)');
        ";
    
}
}