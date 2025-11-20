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

using QuanLiSanCauLong.LopTrinhBay.ManHinh.KhoPOS;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.KhoPOS
{
    /// <summary>
    /// Interaction logic for UcKhoDashboard.xaml
    /// </summary>
    public partial class UcKhoDashboard : UserControl
    {
        public UcKhoDashboard()
        {
            InitializeComponent();

            // Gán dữ liệu mẫu khi load UserControl
            this.Loaded += (s, e) =>
            {
                //if (DataContext == null)
                //   DataContext = this;
                LoadSampleData();

            };
        }
        /// <summary>
        /// ===== DỮ LIỆU MẪU CHO UI =====
        /// </summary>
        private void LoadSampleData()
        {
            DataContext = new
            {
                // KPI mẫu
                Dashboard = new
                {
                    TongGiaTriTon = 12500000,
                    DoanhThuPOSHomNay = 850000,
                    SoMatHang = 12
                },

                // Danh sách sản phẩm POS mẫu
                PosProducts = new List<dynamic>
                {
                    new { MaHH="NU01", TenHH="Nước suối Lavie 500ml", GiaBan=8000, SoLuongHienCo=120 },
                    new { MaHH="NG02", TenHH="Coca Cola 330ml", GiaBan=15000, SoLuongHienCo=65 },
                    new { MaHH="TS03", TenHH="Trà sữa đóng chai", GiaBan=22000, SoLuongHienCo=30 },
                    new { MaHH="SP04", TenHH="Nước tăng lực Sting", GiaBan=14000, SoLuongHienCo=44 },
                    new { MaHH="RA05", TenHH="Revive 500ml", GiaBan=12000, SoLuongHienCo=20 },
                },

                // Quản lý kho mẫu
                KhoProducts = new List<dynamic>
                {
                    new { MaHH="NU01", TenHH="Nước suối Lavie 500ml", DonViTinh="Chai", GiaNhap=5000, GiaBan=8000, SoLuongHienCo=120, NgayNhapCuoi=DateTime.Today.AddDays(-2), TrangThai="Đang bán" },
                    new { MaHH="NG02", TenHH="Coca Cola 330ml", DonViTinh="Lon", GiaNhap=9000, GiaBan=15000, SoLuongHienCo=65, NgayNhapCuoi=DateTime.Today.AddDays(-5), TrangThai="Đang bán" },
                    new { MaHH="TS03", TenHH="Trà sữa đóng chai", DonViTinh="Chai", GiaNhap=15000, GiaBan=22000, SoLuongHienCo=30, NgayNhapCuoi=DateTime.Today.AddDays(-1), TrangThai="Sắp hết" },
                    new { MaHH="SP04", TenHH="Nước tăng lực Sting", DonViTinh="Chai", GiaNhap=8000, GiaBan=14000, SoLuongHienCo=44, NgayNhapCuoi=DateTime.Today.AddDays(-7), TrangThai="Đang bán" },
                    new { MaHH="RA05", TenHH="Revive 500ml", DonViTinh="Chai", GiaNhap=9000, GiaBan=12000, SoLuongHienCo=20, NgayNhapCuoi=DateTime.Today.AddDays(-3), TrangThai="Đang bán" },
                },

                // Giỏ hàng mẫu
                CartItems = new List<dynamic>
                {
                    new { TenHH="Coca Cola 330ml", SoLuong=2, DonGia=15000, ThanhTien=30000 },
                    new { TenHH="Nước suối Lavie 500ml", SoLuong=1, DonGia=8000, ThanhTien=8000 }
                },

                // Tổng tiền mẫu
                CartTotal = 38000
            };
        }



        private void btnNhapHang(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);

            var frm = new frmNhapHang();


            if (parentWindow != null)
            {
                frm.Owner = parentWindow;
                frm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            frm.ShowDialog();
        }

        private void btnLichSuNhap(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);

            var frm = new frmLichsunhap();


            if (parentWindow != null)
            {
                frm.Owner = parentWindow;
                frm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            frm.ShowDialog();
        }

        private void btnThanhToan(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);

            var frm = new frmPhieuThanhToanPOS();


            if (parentWindow != null)
            {
                frm.Owner = parentWindow;
                frm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            frm.ShowDialog();
        }
    }
}

