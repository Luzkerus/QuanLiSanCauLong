using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopNghiepVu;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.KhoPOS;
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

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.KhoPOS
{
    /// <summary>
    /// Interaction logic for UcKhoDashboard.xaml
    /// </summary>
    public partial class UcKhoDashboard : UserControl
    {
        private List<ChiTietHoaDon> gioHang = new List<ChiTietHoaDon>();

        public UcKhoDashboard()
        {
            InitializeComponent();

            // Gán dữ liệu mẫu khi load UserControl
    
            LoadData();
        }
      

        private void LoadData()
        { 
            HangHoaBLL hangHoaBLL = new HangHoaBLL();
            HoaDonBLL hoaDonBLL = new HoaDonBLL();
            dgKho.ItemsSource = hangHoaBLL.LayTatCaHangHoa();
            isSanPham.ItemsSource = hangHoaBLL.LayTatCaHangHoa();
            txtTongTien .Text = "0";
            txtSoMatHang.Text = hangHoaBLL.TinhTongSoHangHoa().ToString();
            txtDoanhThuHomNay.Text = hoaDonBLL.TinhTongDoanhThu(DateTime.Now,DateTime.Now).ToString("N0") + " đ";
            txtGiaTriTonKho.Text = hangHoaBLL.TinhGiaTriTonKho().ToString("N0") + " đ";
        }
        private void txtTimKiemKho_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtTimKiemKho.Text.Trim().ToLower();
            HangHoaBLL hangHoaBLL = new HangHoaBLL();
            var allItems = hangHoaBLL.LayTatCaHangHoa();
            var filteredItems = allItems.Where(hh =>
                hh.MaHang.ToLower().Contains(keyword) ||
                hh.TenHang.ToLower().Contains(keyword)

            ).ToList();
            dgKho.ItemsSource = filteredItems;
        }
        private void txtTimKiemBanHang_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtSeach.Text.Trim().ToLower();
            HangHoaBLL hangHoaBLL = new HangHoaBLL();
            var allItems = hangHoaBLL.LayTatCaHangHoa();
            var filteredItems = allItems.Where(hh =>
                hh.TenHang.ToLower().Contains(keyword)

            ).ToList();
            isSanPham.ItemsSource = filteredItems;
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

            bool? kq = frm.ShowDialog();
            if (kq == true)
            {
                // Nếu người dùng lưu phiếu nhập thành công, tải lại dữ liệu kho
                LoadData();
            }
        }

        private void btnLichSuPOS_Click(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);

            var frm = new frmLichSuPOS();


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


        private void btnThemVaoGioHang_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            // DataContext của Button = sản phẩm
            HangHoa sp = btn.DataContext as HangHoa;

            if (sp == null)
            {
                MessageBox.Show("Không xác định được sản phẩm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ThemVaoGio(sp);
        }
        private void ThemVaoGio(HangHoa sp)
        {
            if (sp == null) return;

            // Kiểm tra tồn kho
            if (sp.TonKho <= 0)
            {
                MessageBox.Show("Sản phẩm đã hết hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra sản phẩm đã tồn tại trong giỏ
            var ct = gioHang.FirstOrDefault(x => x.MaHang == sp.MaHang);

            if (ct != null)
            {
                MessageBox.Show("Sản phẩm đã có trong giỏ hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else
            {
                gioHang.Add(new ChiTietHoaDon
                {
                    MaHang = sp.MaHang,
                    TenHang = sp.TenHang,
                    DVT = sp.DVT,
                    GiaBan = sp.GiaBan,
                    SoLuong = 1,
                    ThanhTien = sp.GiaBan
                });
            }

            CapNhatGioHang();
        }
        private void CapNhatGioHang()
        {
            dgGioHang.ItemsSource = null;
            dgGioHang.ItemsSource = gioHang;

            txtTongTien.Text = gioHang.Sum(x => x.ThanhTien).ToString("N0");
        }
        private void btnXoaKhoiGio_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            // DataContext của Button = sản phẩm trong giỏ
            ChiTietHoaDon ct = btn.DataContext as ChiTietHoaDon;
            if (ct == null)
            {
                MessageBox.Show("Không xác định được sản phẩm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            gioHang.Remove(ct);
            CapNhatGioHang();
        }
        private void btnTruSL_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            // Lấy item trong giỏ từ DataContext
            ChiTietHoaDon ct = btn.DataContext as ChiTietHoaDon;
            if (ct == null) return;

            if (ct.SoLuong > 1)
            {
                ct.SoLuong--;
                ct.ThanhTien = ct.GiaBan * ct.SoLuong;
            }
            else
            {
                // Hỏi có xoá không
                if (MessageBox.Show("Xóa sản phẩm khỏi giỏ hàng?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    gioHang.Remove(ct);
                }
            }

            CapNhatGioHang();
        }
        private void btnTangSL_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            ChiTietHoaDon ct = btn.DataContext as ChiTietHoaDon;
            if (ct == null) return;

            // Lấy tồn kho của sản phẩm
            HangHoaBLL hangHoaBLL = new HangHoaBLL();
            var sp = hangHoaBLL.LayTatCaHangHoa().FirstOrDefault(x => x.MaHang == ct.MaHang);

            if (sp == null) return;

            if (ct.SoLuong < sp.TonKho)
            {
                ct.SoLuong++;
                ct.ThanhTien = ct.GiaBan * ct.SoLuong;
            }
            else
            {
                MessageBox.Show("Không đủ tồn kho!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            CapNhatGioHang();
        }
        private void btnHuyDon(object sender, RoutedEventArgs e)
        {
            gioHang.Clear();
            CapNhatGioHang();
        }
        private void btnThanhToan(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);
            if (gioHang.Count == 0)
            {
                MessageBox.Show("Giỏ hàng trống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var frm = new frmPhieuThanhToanPOS(gioHang);


            if (parentWindow != null)
            {
                frm.Owner = parentWindow;
                frm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            bool? kq = frm.ShowDialog();
            if (kq == true)
            {
                LoadData();
                gioHang.Clear();
                CapNhatGioHang();
            }
        }
    }
}

