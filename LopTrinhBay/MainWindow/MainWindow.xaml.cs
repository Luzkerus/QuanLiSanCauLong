using QuanLiSanCauLong.LopNghiepVu;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.BaoCao;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.DatSan;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.HeThong;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.KhachHoiVien;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.KhoPOS;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.NhanVien;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.QuanLySan;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.ThanhToan;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.TongQuan;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

// using ... các màn khác

namespace QuanLiSanCauLong
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _sessionTimer;
        private DateTime _lastActivityTime;
        private int _timeoutPhut;
        private readonly CauHinhHeThongBLL _cauHinhBLL = new CauHinhHeThongBLL();
        public MainWindow()
        {
            InitializeComponent();

            // Mặc định nạp trang Tổng quan/Overview vào vùng nội dung
            // Ví dụ: MainFrame.Content = new TongQuat.ucTongQuan(); 
            // hoặc 1 UserControl tổng quan:
           MainFrame.Content = new ucTongQuan(); // thay bằng màn thật 
           KhoiTaoSessionTimer();
        }

        private void KhoiTaoSessionTimer()
        {
            // 1. Lấy giá trị Timeout từ Cấu hình
            _timeoutPhut = 60; // Mặc định 60 phút nếu lỗi
            try
            {
                var config = _cauHinhBLL.LayCauHinhHeThong();
                if (config.TimeoutPhien > 0)
                {
                    _timeoutPhut = config.TimeoutPhien;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tải cấu hình Timeout: {ex.Message}");
            }

            // 2. Khởi tạo Timer
            _lastActivityTime = DateTime.Now;
            _sessionTimer = new DispatcherTimer();
            _sessionTimer.Interval = TimeSpan.FromSeconds(1); // Kiểm tra mỗi 1 giây
            _sessionTimer.Tick += SessionTimer_Tick;
            _sessionTimer.Start();
        }

        // ----------------------------------------------------
        // 3. XỬ LÝ SỰ KIỆN TIMER TICK
        // ----------------------------------------------------
        private void SessionTimer_Tick(object sender, EventArgs e)
        {
            // Tính toán thời gian không hoạt động
            TimeSpan idleTime = DateTime.Now - _lastActivityTime;
            TimeSpan maxIdleTime = TimeSpan.FromMinutes(_timeoutPhut);

            if (idleTime > maxIdleTime)
            {
                // Đạt đến ngưỡng timeout, thực hiện tự động đăng xuất/đóng màn hình
                _sessionTimer.Stop();

                // TODO: Thực hiện Đăng xuất, hiển thị màn hình đăng nhập, hoặc đóng ứng dụng
                ThucHienDangXuatTuDong();
            }
        }

        // ----------------------------------------------------
        // 4. BẮT SỰ KIỆN HOẠT ĐỘNG (Activity Monitoring)
        // ----------------------------------------------------

        // Ghi đè phương thức xử lý Input (Bắt hoạt động chuột, bàn phím)
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            CapNhatHoatDong();
            base.OnPreviewMouseMove(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            CapNhatHoatDong();
            base.OnPreviewKeyDown(e);
        }

        // Phương thức cập nhật thời gian hoạt động cuối cùng
        private void CapNhatHoatDong()
        {
            _lastActivityTime = DateTime.Now;
        }

        // ----------------------------------------------------
        // 5. LOGIC ĐĂNG XUẤT (Thực hiện hành động)
        // ----------------------------------------------------
        private void ThucHienDangXuatTuDong()
        {
            MessageBox.Show($"Phiên làm việc đã hết hạn sau {_timeoutPhut} phút không hoạt động. Vui lòng đăng nhập lại.",
                            "Hết phiên làm việc", MessageBoxButton.OK, MessageBoxImage.Information);

            // Ví dụ: Đóng cửa sổ chính (kết thúc ứng dụng)
            // Nếu bạn có màn hình đăng nhập, bạn nên đóng MainWindow và mở lại LoginWindow.
            frmDangNhap frmDangNhap = new frmDangNhap();
            frmDangNhap.ShowDialog();
            this.Close();
        }
        // TẤT CẢ trang chức năng chỉ chạy ở đây (ô trắng)
        // Trong MainWindow.xaml.cs

        // ... (Các using và code khác) ...

        private void Sidebar_NavigateRequested(object sender, string key)
        {
            // Kiểm tra trạng thái đăng nhập chung
            if (!SessionManager.IsLoggedIn)
            {
                MessageBox.Show("Vui lòng đăng nhập để sử dụng chức năng.", "Yêu cầu đăng nhập", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Biến để lưu vai trò yêu cầu cho chức năng hiện tại
            string vaiTroYeuCau = string.Empty;
            bool accessGranted = false;

            // Khởi tạo nội dung mới (để tránh lỗi nếu switch không gán)
            object newContent = null;

            switch (key)
            {
                case "overview":
                    newContent = new ucTongQuan();
                    accessGranted = true;
                    break;

                case "courts": // Quản lý Sân (QLSan) - CHỈ ADMIN & QUẢN LÝ
                    vaiTroYeuCau = "Admin hoặc Quản lý";
                    if (SessionManager.CanAccessQuanLySan)
                    {
                        newContent = new ucQuanLySan();
                        accessGranted = true;
                    }
                    break;

                case "booking":
                    newContent = new ucDatSan();
                    accessGranted = true;
                    break;

                case "customers":
                    newContent = new ucKhachHoiVien();
                    accessGranted = true;
                    break;

                case "payment":
                    newContent = new ucThanhToan();
                    accessGranted = true;
                    break;

                case "pos":
                    newContent = new UcKhoDashboard();
                    accessGranted = true;
                    break;

                case "staff": // Quản lý Nhân viên - CHỈ ADMIN
                    vaiTroYeuCau = "Admin";
                    if (SessionManager.IsAdmin)
                    {
                        newContent = new ucNhanVien();
                        accessGranted = true;
                    }
                    break;

                case "reports": // Báo cáo - ADMIN & NHÂN VIÊN
                    vaiTroYeuCau = "Admin hoặc Quản lý";
                    if (SessionManager.CanAccessBaoCao)
                    {
                        newContent = new ucBaoCao();
                        accessGranted = true;
                    }
                    break;

                case "settings": // Cấu hình hệ thống - CHỈ ADMIN

                        newContent = new ucCauHinhHeThong();
                        accessGranted = true;

                    break;
            }

            // --- Xử lý kết quả truy cập ---
            if (accessGranted)
            {
                // Nếu được phép truy cập, gán nội dung mới
                MainFrame.Content = newContent;
            }
            else
            {
                // Nếu bị từ chối truy cập
                string chucNang;

                // SỬ DỤNG CÁC LỆNH IF/ELSE IF (Hoặc LỆNH SWITCH TRUYỀN THỐNG)
                if (key == "courts")
                {
                    chucNang = "Quản lý Sân";
                }
                else if (key == "staff")
                {
                    chucNang = "Quản lý Nhân viên";
                }
                else if (key == "reports")
                {
                    chucNang = "Báo cáo";
                }
                else if (key == "settings")
                {
                    chucNang = "Cấu hình Hệ thống";
                }
                else
                {
                    chucNang = "Chức năng này";
                }
                string message = $"Bạn không có quyền truy cập vào chức năng **{chucNang}**.\n\n" +
                                 $"Yêu cầu vai trò: **{vaiTroYeuCau}**\n" +
                                 $"Vai trò hiện tại: **{SessionManager.CurrentUser.VaiTro}**";

                MessageBox.Show(message, "Từ chối truy cập", MessageBoxButton.OK, MessageBoxImage.Stop);

                // Optional: Không chuyển màn hình, giữ lại màn hình đang xem.
            }
        }

    }
}
