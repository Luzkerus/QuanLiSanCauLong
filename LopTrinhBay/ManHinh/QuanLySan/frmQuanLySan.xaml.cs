using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            DanhSachSan = new List<San>
            {
                new San { TenSan="Sân 1", TrangThai="Đang chơi", GiaNgayThuong="80.000đ/giờ", GiaCuoiTuan="100.000đ/giờ", GiaLeTet="120.000đ/giờ", NgayBaoTri="15/11/2025" },
                new San { TenSan="Sân 2", TrangThai="Trống",  GiaNgayThuong="80.000đ/giờ", GiaCuoiTuan="100.000đ/giờ", GiaLeTet="120.000đ/giờ", NgayBaoTri="20/11/2025" },
                new San { TenSan="Sân 3", TrangThai="Đặt",  GiaNgayThuong="80.000đ/giờ", GiaCuoiTuan="100.000đ/giờ", GiaLeTet="120.000đ/giờ", NgayBaoTri="18/11/2025" },
                new San { TenSan="Sân 4", TrangThai="Đang chơi",  GiaNgayThuong="80.000đ/giờ", GiaCuoiTuan="100.000đ/giờ", GiaLeTet="120.000đ/giờ", NgayBaoTri="25/11/2025" },
                new San { TenSan="Sân 5", TrangThai="Đang chơi",  GiaNgayThuong="80.000đ/giờ", GiaCuoiTuan="100.000đ/giờ", GiaLeTet="120.000đ/giờ", NgayBaoTri="22/11/2025" },
                new San { TenSan="Sân 6", TrangThai="Trống", GiaNgayThuong="80.000đ/giờ", GiaCuoiTuan="100.000đ/giờ", GiaLeTet="120.000đ/giờ", NgayBaoTri="01/12/2025" },
                new San { TenSan="Sân 7", TrangThai="Trống", GiaNgayThuong="80.000đ/giờ", GiaCuoiTuan="100.000đ/giờ", GiaLeTet="120.000đ/giờ", NgayBaoTri="01/12/2025" },
            };
            DataContext = this;
        }

        public class San
        {
            public string TenSan { get; set; }
            public string TrangThai { get; set; }
            public string GiaNgayThuong { get; set; }
            public string GiaCuoiTuan { get; set; }
            public string GiaLeTet { get; set; }
            public string NgayBaoTri { get; set; }

        }
        private void btnThemSanMoi(object sender, RoutedEventArgs e)
        {
            // Mở cửa sổ thêm sân
            frmThemSanMoi themSanMoiWindow = new frmThemSanMoi();
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
            ChinhSuaSanWindow.ShowDialog();
        }

    }
}