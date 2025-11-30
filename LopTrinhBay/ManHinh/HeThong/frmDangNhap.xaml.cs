using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopNghiepVu;
using NhanVienModel = QuanLiSanCauLong.LopDuLieu.NhanVien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.HeThong
{
    /// <summary>
    /// Interaction logic for frmDangNhap.xaml
    /// </summary>
    public partial class frmDangNhap : Window
    {
        private readonly NhanVienBLL _nhanVienBLL = new NhanVienBLL();
        public frmDangNhap()
        {
            InitializeComponent();
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Lấy thông tin từ giao diện
            string username = txtUser.Text.Trim();
            string password = pwd.Password; // Lấy mật khẩu từ PasswordBox
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Tên đăng nhập và mật khẩu không được để trống.",
                                "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);

                // Trả về ngay lập tức, không gọi hàm DangNhap
                return;
            }
            NhanVienModel nhanVienDangNhap = _nhanVienBLL.DangNhap(username, password);

            if (nhanVienDangNhap == null)
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.",
                                "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (nhanVienDangNhap.TrangThai != "Đang làm")
            {
                MessageBox.Show($"Tài khoản đang ở trạng thái {nhanVienDangNhap.TrangThai}, không thể đăng nhập.",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                SessionManager.Login(nhanVienDangNhap);
                MessageBox.Show($"Đăng nhập thành công! Chào mừng {nhanVienDangNhap.TenNV} ({nhanVienDangNhap.VaiTro}).",
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }
    }
}
