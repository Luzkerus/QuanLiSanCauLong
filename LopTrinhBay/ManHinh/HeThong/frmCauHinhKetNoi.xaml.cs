using QuanLiSanCauLong.LopTruyCapDuLieu;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient; // Cần dùng để tạo connection string

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.HeThong
{
    public partial class frmCauHinhKetNoi : Window
    {
        // Biến cờ để quyết định có mở màn hình đăng nhập sau khi lưu thành công không
        private readonly bool _showLoginAfterSave;

        // Constructor cập nhật: nhận tham số để mở frmDangNhap sau khi cấu hình
        public frmCauHinhKetNoi(bool showLoginAfterSave = false)
        {
            InitializeComponent();
            _showLoginAfterSave = showLoginAfterSave;

            // Hiển thị chuỗi kết nối hiện tại lên form
            LoadCurrentConfig();

            chkWindowsAuth_Checked(null, null); // Cập nhật trạng thái User/Pass ban đầu
        }

        // Hàm tải chuỗi kết nối hiện tại lên các ô nhập liệu
        private void LoadCurrentConfig()
        {
            // Cố gắng phân tích chuỗi kết nối hiện tại để hiển thị lên UI
            string currentCs = ConnectStringDAL.Instance.GetConnectionString();

            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(currentCs);

                txtServer.Text = builder.DataSource;
                txtDatabase.Text = builder.InitialCatalog;

                // Kiểm tra loại Authentication
                if (builder.IntegratedSecurity)
                {
                    chkWindowsAuth.IsChecked = true;
                }
                else
                {
                    chkWindowsAuth.IsChecked = false;
                    txtUser.Text = builder.UserID;
                    // Không bao giờ hiển thị mật khẩu đã lưu
                }
            }
            catch
            {
                // Nếu chuỗi kết nối mặc định không hợp lệ, giữ nguyên ô trống
            }
        }


        // Tạo chuỗi kết nối từ các control
        private string CreateConnectionString(bool isWindowsAuth)
        {
            string server = txtServer.Text.Trim();
            string database = txtDatabase.Text.Trim();

            if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(database))
            {
                return null;
            }

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = server;
            builder.InitialCatalog = database;
            builder.Encrypt = true;
            builder.TrustServerCertificate = true;

            if (isWindowsAuth)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                string user = txtUser.Text.Trim();
                string password = txtPassword.Password;
                if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
                {
                    return null;
                }
                builder.UserID = user;
                builder.Password = password;
            }
            return builder.ConnectionString;
        }

        // Xử lý khi CheckBox Windows Auth thay đổi
        private void chkWindowsAuth_Checked(object sender, RoutedEventArgs e)
        {
            bool isChecked = chkWindowsAuth.IsChecked ?? false;
            txtUser.IsEnabled = !isChecked;
            txtPassword.IsEnabled = !isChecked;

            if (isChecked)
            {
                // Không xóa mật khẩu, chỉ ẩn và vô hiệu hóa
            }
        }

        // Nút Test
        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            string newConnectionString = CreateConnectionString(chkWindowsAuth.IsChecked ?? false);

            if (string.IsNullOrWhiteSpace(newConnectionString))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin kết nối.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Gọi hàm CheckConnection qua Instance Singleton
            if (ConnectStringDAL.Instance.CheckConnection(newConnectionString))
            {
                MessageBox.Show("Kết nối thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Kết nối thất bại. Vui lòng kiểm tra lại thông tin Server/Database/User/Password.", "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Nút Save
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string newConnectionString = CreateConnectionString(chkWindowsAuth.IsChecked ?? false);

            if (string.IsNullOrWhiteSpace(newConnectionString))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin kết nối.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Chỉ lưu khi kết nối thành công
            // Gọi hàm CheckConnection qua Instance Singleton
            if (ConnectStringDAL.Instance.CheckConnection(newConnectionString))
            {
                // Gọi hàm SetConnectionString qua Instance Singleton (hàm này tự động lưu vào file)
                ConnectStringDAL.Instance.SetConnectionString(newConnectionString);

                MessageBox.Show("Lưu cấu hình kết nối thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Nếu được gọi từ OnStartup, mở màn hình đăng nhập
                if (_showLoginAfterSave)
                {
                    frmDangNhap loginWindow = new frmDangNhap();
                    loginWindow.Show();
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("Lưu thất bại. Không thể kết nối với cấu hình này.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}