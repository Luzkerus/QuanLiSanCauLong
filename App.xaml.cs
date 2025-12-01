using QuanLiSanCauLong.LopTrinhBay.ManHinh.HeThong;
using QuanLiSanCauLong.LopTruyCapDuLieu;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace QuanLiSanCauLong
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            bool isNew;
            _mutex = new Mutex(true, "QuanLiSanCauLong_SingleInstance", out isNew);
            if (!isNew)
            {
                MessageBox.Show("Ứng dụng đang chạy.", "Thông báo");
                Shutdown();
                return;
            }
            base.OnStartup(e);
            string currentConnectionString = ConnectStringDAL.Instance.GetConnectionString();

            if (ConnectStringDAL.Instance.CheckConnection(currentConnectionString))
            {
                // **KẾT NỐI THÀNH CÔNG**

                // 2. Kiểm tra và Khởi tạo Schema Database
                bool schemaReady = ConnectStringDAL.Instance.InitializeDatabase();

                if (schemaReady)
                {
                    // Nếu Schema đã sẵn sàng, mở màn hình đăng nhập
                    frmDangNhap loginWindow = new frmDangNhap();
                    loginWindow.Show();
                }
                else
                {
                    // Nếu không khởi tạo được schema (người dùng hủy), đóng ứng dụng
                    Shutdown();
                }
            }
            else
            {
                // KẾT NỐI THẤT BẠI: Thông báo và mở màn hình cấu hình
                MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu. Vui lòng cấu hình lại kết nối.",
                                "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);

                // Truyền cờ báo hiệu cần mở màn hình đăng nhập sau khi cấu hình thành công
                frmCauHinhKetNoi configWindow = new frmCauHinhKetNoi(showLoginAfterSave: true);
                configWindow.Show();
            }
        }
    }

}
