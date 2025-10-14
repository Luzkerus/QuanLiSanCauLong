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
        public frmQuanLySan()
        {
            InitializeComponent();

            DanhSachSan = new List<San>
            {
                new San { TenSan="Sân 1", TrangThai="Đang chơi", MauNen="#DFF7EA", MauChu="#1E7F4C", GiaNgayThuong="80.000đ/giờ", GiaCuoiTuan="100.000đ/giờ", GiaLeTet="120.000đ/giờ", NgayBaoTri="15/11/2025" },
                new San { TenSan="Sân 2", TrangThai="Trống", MauNen="#EDF3FF", MauChu="#3C5AA8", GiaNgayThuong="80.000đ/giờ", GiaCuoiTuan="100.000đ/giờ", GiaLeTet="120.000đ/giờ", NgayBaoTri="20/11/2025" },
                new San { TenSan="Sân 3", TrangThai="Đặt", MauNen="#FFF4E3", MauChu="#B76B00", GiaNgayThuong="80.000đ/giờ", GiaCuoiTuan="100.000đ/giờ", GiaLeTet="120.000đ/giờ", NgayBaoTri="18/11/2025" },
                new San { TenSan="Sân 4", TrangThai="Đang chơi", MauNen="#DFF7EA", MauChu="#1E7F4C", GiaNgayThuong="80.000đ/giờ", GiaCuoiTuan="100.000đ/giờ", GiaLeTet="120.000đ/giờ", NgayBaoTri="25/11/2025" },
                new San { TenSan="Sân 5", TrangThai="Đang chơi", MauNen="#DFF7EA", MauChu="#1E7F4C", GiaNgayThuong="80.000đ/giờ", GiaCuoiTuan="100.000đ/giờ", GiaLeTet="120.000đ/giờ", NgayBaoTri="22/11/2025" },
                new San { TenSan="Sân 6", TrangThai="Trống", MauNen="#EDF3FF", MauChu="#3C5AA8", GiaNgayThuong="80.000đ/giờ", GiaCuoiTuan="100.000đ/giờ", GiaLeTet="120.000đ/giờ", NgayBaoTri="01/12/2025" },
                new San { TenSan="Sân 7", TrangThai="Trống", MauNen="#EDF3FF", MauChu="#3C5AA8", GiaNgayThuong="80.000đ/giờ", GiaCuoiTuan="100.000đ/giờ", GiaLeTet="120.000đ/giờ", NgayBaoTri="01/12/2025" },
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
            public string MauNen { get; set; } // màu nền của tag trạng thái
            public string MauChu { get; set; } // màu chữ của tag trạng thái
        }

    }
}