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

            try
            {
                // Gọi hàm nghiệp vụ để kiểm tra đăng nhập
                NhanVienModel nhanVienDangNhap = _nhanVienBLL.DangNhap(username, password);

                if (nhanVienDangNhap != null)
                {
                    SessionManager.Login(nhanVienDangNhap);
                    // Đăng nhập thành công
                    MessageBox.Show($"Đăng nhập thành công! Chào mừng {nhanVienDangNhap.TenNV} ({nhanVienDangNhap.VaiTro}).",
                                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    // TODO: Chuyển sang màn hình chính (Main Window)
                    // Ví dụ:
                    // new MainWindow().Show();
                    this.Close();
                }
                else
                {
                    // Đăng nhập thất bại (Sai Username hoặc Password)
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.",
                                    "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (ArgumentException ex)
            {
                // Xử lý lỗi do nhập thiếu dữ liệu (ví dụ: Username/Password trống)
                MessageBox.Show(ex.Message, "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khác (ví dụ: lỗi kết nối CSDL)
                MessageBox.Show($"Đã xảy ra lỗi hệ thống: {ex.Message}",
                                "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
