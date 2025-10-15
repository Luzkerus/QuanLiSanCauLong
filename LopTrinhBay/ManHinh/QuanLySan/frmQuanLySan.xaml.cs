using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.QuanLySan
{
    /// <summary>
    /// Interaction logic for frmChinhSuaSan.xaml
    /// </summary>
    public partial class frmQuanLySan : Window
    {
        string connectionString = "Data Source=.;Initial Catalog=QuanLiSanCauLong;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";
        public List<San> DanhSachSan { get; set; }
        public int TongSoSan => DanhSachSan?.Count ?? 0;
        public int TongSoSanKhongBaoTri =>
        DanhSachSan?.Count(s => s.TrangThai != "Bảo trì") ?? 0;
        public int TongSoSanBaoTri =>
        DanhSachSan?.Count(s => s.TrangThai == "Bảo trì") ?? 0;
        public int TongSoSanDat =>
        DanhSachSan?.Count(s => s.TrangThai == "Đặt") ?? 0;

        public frmQuanLySan()
        {
            InitializeComponent();

            DanhSachSan = LayDanhSachSanTuDatabase();

            DataContext = this;
        }
        private List<San> LayDanhSachSanTuDatabase()
        {
            List<San> dsSan = new List<San>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT MaSan, TenSan, TrangThai, GiaNgayThuong, GiaCuoiTuan, GiaLeTet, NgayBaoTri FROM dbo.San";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    San san = new San
                    {
                        MaSan = reader.GetInt32(0),
                        TenSan = reader.GetString(1),
                        TrangThai = reader.GetString(2),
                        GiaNgayThuong = reader.GetDecimal(3),
                        GiaCuoiTuan = reader.GetDecimal(4),
                        GiaLeTet = reader.GetDecimal(5),
                        NgayBaoTri = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6)
                    };
                    dsSan.Add(san);
                }
            }

            return dsSan;
        }


        public class San
        {
            public int MaSan { get; set; }
            public string TenSan { get; set; }
            public string TrangThai { get; set; }
            public decimal GiaNgayThuong { get; set; }
            public decimal GiaCuoiTuan { get; set; }
            public decimal GiaLeTet { get; set; }
            public DateTime? NgayBaoTri { get; set; }
        }

        private void btnThemSanMoi(object sender, RoutedEventArgs e)
        {
            // Mở cửa sổ thêm sân
            frmThemSanMoi themSanMoiWindow = new frmThemSanMoi();
            themSanMoiWindow.Owner = this; // Đặt cửa sổ cha
            themSanMoiWindow.ShowDialog();
        }

        private void btn_Xoa(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Nút đã được nhấn thành công! Đây là thông báo từ Code-behind (C#).", // Nội dung thông báo
                "Xác nhận Hoạt Động Của Nút", // Tiêu đề của hộp thoại
                MessageBoxButton.OK, // Chỉ hiển thị nút OK
                MessageBoxImage.Information // Icon thông tin
            );
        }

        private void btn_Sua(object sender, RoutedEventArgs e) { 
            frmChinhSuaSan ChinhSuaSanWindow = new frmChinhSuaSan();
            ChinhSuaSanWindow.Owner = this; // Đặt cửa sổ cha
            ChinhSuaSanWindow.ShowDialog();
        }

    }
}