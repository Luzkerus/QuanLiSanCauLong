using Microsoft.Win32;
using ClosedXML.Excel;
using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopNghiepVu;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.ThanhToan
{
    /// <summary>
    /// Interaction logic for frmLichSuThanhToan.xaml
    /// </summary>
    public partial class frmLichSuThanhToan : Window
    {
        private ThanhToanBLL thanhToanBLL = new ThanhToanBLL();
        public frmLichSuThanhToan()
        {
            InitializeComponent();
            //LoadData();
        }
        // Kéo thả cửa sổ khi giữ chuột ở header
        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Cho phép kéo cửa sổ khi giữ chuột trái
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { /* ignore errors khi click nhanh */ }
            }
        }
        private void LoadData()
        {
            var danhSachHoaDon = thanhToanBLL.LayTatCaHoaDon();
            dgvLichSuThanhToan.ItemsSource = danhSachHoaDon;
        }

        // Thu nhỏ cửa sổ
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // Đóng cửa sổ
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void BtnXem_Click(object sender, RoutedEventArgs e)
        {
            DateTime? tuNgay = dpTuNgay.SelectedDate;
            DateTime? denNgay = dpDenNgay.SelectedDate;
            if (tuNgay.HasValue && denNgay.HasValue && tuNgay > denNgay)
            {
                MessageBox.Show("Ngày bắt đầu không được sau ngày kết thúc.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var danhSachHoaDon = thanhToanBLL.LayTatCaHoaDon();

            if (tuNgay.HasValue)
                danhSachHoaDon = danhSachHoaDon.Where(h => h.NgayLap.Date >= tuNgay.Value.Date).ToList();

            if (denNgay.HasValue)
                danhSachHoaDon = danhSachHoaDon.Where(h => h.NgayLap.Date <= denNgay.Value.Date).ToList();

            dgvLichSuThanhToan.ItemsSource = danhSachHoaDon;
        }
        private void btnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            dpTuNgay.SelectedDate = null;
            dpDenNgay.SelectedDate = null;
            LoadData();
        }
        private void btnXuat_Click(object sender, RoutedEventArgs e)
        {
            DateTime? tuNgay = dpTuNgay.SelectedDate;
            DateTime? denNgay = dpDenNgay.SelectedDate;

            var danhSachHoaDon = dgvLichSuThanhToan.ItemsSource as List<QuanLiSanCauLong.LopDuLieu.ThanhToan>;
            if (danhSachHoaDon == null || danhSachHoaDon.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Lọc theo ngày nếu có chọn
            if (tuNgay.HasValue)
                danhSachHoaDon = danhSachHoaDon.Where(h => h.NgayLap.Date >= tuNgay.Value.Date).ToList();

            if (denNgay.HasValue)
                danhSachHoaDon = danhSachHoaDon.Where(h => h.NgayLap.Date <= denNgay.Value.Date).ToList();

            if (danhSachHoaDon.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu trong khoảng thời gian đã chọn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files|*.xlsx";
            saveFileDialog.FileName = $"LichSuThanhToan_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("Lịch sử thanh toán");

                    // ===== HEADER THÔNG TIN SÂN =====
                    ws.Cell(1, 1).Value = "SÂN CẦU LÔNG CỦ CHI";
                    ws.Range(1, 1, 1, 8).Merge(); // gộp 8 cột
                    ws.Cell(1, 1).Style.Font.Bold = true;
                    ws.Cell(1, 1).Style.Font.FontSize = 14;
                    ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(2, 1).Value = "72, Tỉnh lộ 16, Ấp Phú Thuận, Xã Phú Hòa Đông, TP. Hồ Chí Minh";
                    ws.Range(2, 1, 2, 8).Merge();
                    ws.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(2, 1).Style.Font.FontSize = 10;

                    ws.Cell(3, 1).Value = "Tel: 0966752642";
                    ws.Range(3, 1, 3, 8).Merge();
                    ws.Cell(3, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(3, 1).Style.Font.FontSize = 10;


                    ws.Cell(4, 1).Value = $"Từ ngày: {tuNgay?.ToString("dd/MM/yyyy") ?? ""} Đến ngày: {denNgay?.ToString("dd/MM/yyyy") ?? ""}";
                    ws.Range(4, 1, 4, 8).Merge();
                    ws.Cell(4, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(4, 1).Style.Font.FontSize = 11;
                    ws.Cell(4, 1).Style.Font.Bold = true;
                    // ===== HEADER CỘT DỮ LIỆU =====
                    ws.Cell(6, 1).Value = "Số HĐ";
                    ws.Cell(6, 2).Value = "Ngày lập";
                    ws.Cell(6, 3).Value = "Khách hàng";
                    ws.Cell(6, 4).Value = "SĐT";
                    ws.Cell(6, 5).Value = "Tổng tiền sân";
                    ws.Cell(6, 6).Value = "Phương thức";
                    ws.Cell(6, 7).Value = "Tổng tiền thuê vợt";
                    ws.Cell(6, 8).Value = "Tổng thanh toán";

                    ws.Range(6, 1, 6, 8).Style.Font.Bold = true;
                    ws.Range(6, 1, 6, 8).Style.Fill.BackgroundColor = XLColor.LightGray;
                    ws.Range(6, 1, 6, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    int row = 7; // bắt đầu dữ liệu từ hàng 6
                    foreach (var hd in danhSachHoaDon)
                    {
                        ws.Cell(row, 1).Value = hd.SoHD;
                        ws.Cell(row, 2).Value = hd.NgayLap.ToString("dd/MM/yyyy HH:mm");
                        ws.Cell(row, 3).Value = hd.TenKH;
                        ws.Cell(row, 4).Value = hd.SDT;
                        ws.Cell(row, 5).Value = hd.TongTienSan;
                        ws.Cell(row, 6).Value = hd.PhuongThuc;
                        ws.Cell(row, 7).Value = hd.TongTienThueVot;
                        ws.Cell(row, 8).Value = hd.TongTien;
                        row++;
                    }

                    // Format tiền tệ
                    ws.Column(5).Style.NumberFormat.Format = "#,##0 ₫";
                    ws.Column(7).Style.NumberFormat.Format = "#,##0 ₫";
                    ws.Column(8).Style.NumberFormat.Format = "#,##0 ₫";

                    ws.Columns().AdjustToContents();

                    workbook.SaveAs(saveFileDialog.FileName);
                }


                MessageBox.Show("Đã xuất dữ liệu ra Excel thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }



    }
}
