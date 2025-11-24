using DocumentFormat.OpenXml.Office2010.ExcelAc;
using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopNghiepVu;
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
    /// Interaction logic for frmLichsunhap.xaml
    /// </summary>
    public partial class frmLichsunhap : Window
    {
        PhieuNhapBLL pnBLL = new PhieuNhapBLL();
        ChiTietPhieuNhapBLL ctpnBLL = new ChiTietPhieuNhapBLL();
        public frmLichsunhap()
        {
            InitializeComponent();
            txtTongSoPhieu.Text = pnBLL.TatCaSoPhieuNhap().ToString();
            txtTongTienNhap.Text = pnBLL.TongTienTatCaPhieuNhap().ToString("N0") + " đ";
            txtNhapHomNay.Text = pnBLL.TongTienPhieuNhapHomNay().ToString("N0") + " đ";
        }

        private void btnLoc_Click(object sender, RoutedEventArgs e)
        {
            DateTime fromDate = dptuNgay.SelectedDate ?? DateTime.MinValue;
            DateTime toDate = dpdenNgay.SelectedDate ?? DateTime.MaxValue;

            string timKiem = txtTimKiem.Text?.Trim().ToLower() ?? "";

            // Lấy danh sách phiếu nhập
            var list = pnBLL.LayPhieuNhapTheoNgay(fromDate, toDate).AsEnumerable();

            // Lọc theo từ khóa
            if (!string.IsNullOrWhiteSpace(timKiem))
            {
                list = list.Where(p =>
                         (!string.IsNullOrEmpty(p.NhaCungCap) && p.NhaCungCap.ToLower().Contains(timKiem)) ||
                         (!string.IsNullOrEmpty(p.GhiChu) && p.GhiChu.ToLower().Contains(timKiem))
                );
            }

            var finalList = list.ToList();
            dgPhieuNhap.ItemsSource = finalList;

            // Lọc chi tiết theo danh sách số phiếu sau lọc
            var soPhieuList = finalList.Select(x => x.SoPhieu).ToList();
            var chiTiet = ctpnBLL.LayTatCaChiTietPhieuNhap()
                                 .Where(ct => soPhieuList.Contains(ct.SoPhieu))
                                 .ToList();

            dgChiTiet.ItemsSource = chiTiet;
        }


        private void btnXoaLoc_Click(object sender, RoutedEventArgs e)
        {
            dptuNgay.SelectedDate = null;
            dpdenNgay.SelectedDate = null;
            dgPhieuNhap.ItemsSource = null;
            dgChiTiet.ItemsSource = null;
        }
        private void dgPhieuNhap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lấy dòng đang được chọn
            var selected = dgPhieuNhap.SelectedItem as PhieuNhap;
            if (selected == null)
            {
                // Xóa trắng nếu không chọn gì
                txtSoPhieu.Text = "";
                txtNCC.Text = "";
                txtGhiChu.Text = "";
                txtTongTien.Text = "";

                dgChiTiet.ItemsSource = null;
                return;
            }

            // ====== HIỂN THỊ THÔNG TIN PHIẾU ======
            txtSoPhieu.Text = selected.SoPhieu;
            txtNCC.Text = selected.NhaCungCap;
            txtGhiChu.Text = selected.GhiChu;
            txtTongTien.Text = selected.TongTien.ToString("N0");

            // ====== HIỂN THỊ CHI TIẾT THEO SỐ PHIẾU ======
            dgChiTiet.ItemsSource = ctpnBLL.LayChiTietTheoSoPhieu(selected.SoPhieu);
        }

    }
}
