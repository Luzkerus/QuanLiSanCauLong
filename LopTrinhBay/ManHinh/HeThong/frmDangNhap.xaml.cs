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
using QuanLiSanCauLong.Properties;

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
            LoadRememberMeSettings();
        }
        private void LoadRememberMeSettings()
        {
            // Kiểm tra trạng thái "Ghi nhớ" đã lưu
            if (Settings.Default.RememberMe)
            {
                // Nếu là true, tải tên đăng nhập đã lưu vào TextBox
                txtUser.Text = Settings.Default.Username;
                chkRememberMe.IsChecked = true;
                pwd.Focus(); // Chuyển con trỏ đến ô mật khẩu
            }
            else
            {
                // Nếu là false, đảm bảo các trường trống
                txtUser.Text = string.Empty;
                chkRememberMe.IsChecked = false;
                txtUser.Focus();
            }
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Lấy thông tin từ giao diện
            string username = txtUser.Text.Trim();
            string password = pwd.Password; // Lấy mật khẩu từ PasswordBox
            bool rememberMe = chkRememberMe.IsChecked == true;
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
                // Đăng nhập thành công, xử lý ghi nhớ
                if (rememberMe)
                {
                    Settings.Default.Username = username;
                    Settings.Default.RememberMe = true;
                }
                else
                {
                    Settings.Default.Username = string.Empty;
                    Settings.Default.RememberMe = false;
                }
                // LƯU CÀI ĐẶT
                Settings.Default.Save(); // <--- THÊM quan trọng

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
