using ClosedXML.Excel;
using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopNghiepVu;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.BaoCao
{
    public partial class ucBaoCao : UserControl
    {
        public ObservableCollection<TimeSlotVM> TopTimeSlots { get; set; }
        public ucBaoCao()
        {
          InitializeComponent(); // ← KHÔNG được comment
                                 // Đầu tháng
            DateTime dauThang = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // Cuối tháng
            DateTime cuoiThang = dauThang.AddMonths(1).AddDays(-1);

            // Gán cho DatePicker
            dpTuNgay.SelectedDate = dauThang;
            dpDenNgay.SelectedDate = cuoiThang;
            this.Loaded += (s, e) => LoadDuLieu();
            this.ChartCanvas.SizeChanged += ChartCanvas_SizeChanged;

        }
        private void ChartCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Kiểm tra xem đã có dữ liệu để vẽ chưa
            if (dpTuNgay.SelectedDate.HasValue && dpDenNgay.SelectedDate.HasValue)
            {
                // Vẽ lại biểu đồ với kích thước mới
                VeBieuDo(dpTuNgay.SelectedDate.Value, dpDenNgay.SelectedDate.Value);
            }
        }
        private void Loc_Click(object sender, RoutedEventArgs e)
        {
            LoadDuLieu();
        }
        private void LoadDuLieu() {
            DateTime? fromDate = dpTuNgay.SelectedDate;
            DateTime? toDate = dpDenNgay.SelectedDate;
            if (fromDate == null || toDate == null)
            {
                MessageBox.Show("Vui lòng chọn cả ngày bắt đầu và ngày kết thúc.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (fromDate > toDate)
            {
                MessageBox.Show("Ngày bắt đầu không được sau ngày kết thúc.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ThanhToanBLL thanhToanBLL = new ThanhToanBLL();
            HoaDonBLL hoaDonBLL = new HoaDonBLL();
            DatSanBLL datSanBLL = new DatSanBLL();
            KhachHangBLL khachHangBLL = new KhachHangBLL();
            txtDoanhThuSan.Text = thanhToanBLL.TinhDoanhThuTuNgayDenNgay(fromDate.Value, toDate.Value).ToString("N0");
            txtDoanhThuPOS.Text = hoaDonBLL.TinhDoanhThuTuNgayDenNgay(fromDate.Value, toDate.Value).ToString("N0");
            txtLuotKhach.Text = datSanBLL.TinhSoLuotDatSanTuNgayDenNgay(fromDate.Value, toDate.Value).ToString();
            txtTongDonhThu.Text = (decimal.Parse(txtDoanhThuSan.Text.Replace(",", "")) + decimal.Parse(txtDoanhThuPOS.Text.Replace(",", ""))).ToString("N0");
            VeBieuDo(fromDate.Value, toDate.Value);
            TopTimeSlots = new ObservableCollection<TimeSlotVM>(
                datSanBLL.LayBaoCaoTheoKhungGio(fromDate.Value, toDate.Value)
            );

            this.DataContext = this;
            txtTongSoDonDat.Text = txtLuotKhach.Text;
            txtTongGioChoi.Text = datSanBLL.TinhSoGioTuNgayDenNgay(fromDate.Value, toDate.Value).ToString();
            txtGiaTrungBinhGio.Text = datSanBLL.TinhGiaTrungBinhMotGio(fromDate.Value, toDate.Value).ToString("N0");
            

            txtMatHangBanChay.Text = hoaDonBLL.LayMatHangBanChayNhat(fromDate.Value, toDate.Value);
            txtGiaTriTrungBinhPOS.Text = hoaDonBLL.GiaTriTrungBinhBanRa(fromDate.Value, toDate.Value).ToString("N0");
            txtSoLuongPOS.Text = hoaDonBLL.TongSoLuongBanRa(fromDate.Value, toDate.Value).ToString();
            txtHoiVienMoi.Text = khachHangBLL.TinhHoiVienMoiTuNgayDenNgay(fromDate.Value, toDate.Value).ToString();
            txtDiemTichLuyPhatHanh.Text = khachHangBLL.TinhDiemTichLuyTuNgayDenNgay(fromDate.Value, toDate.Value).ToString("N0");
        }

        private void VeBieuDo(DateTime fromDate, DateTime toDate)
        {
            ChartCanvas.Children.Clear();

            ThanhToanBLL thanhToanBLL = new ThanhToanBLL();
            HoaDonBLL hoaDonBLL = new HoaDonBLL();

            var data = new List<(DateTime Ngay, decimal Pos, decimal San, decimal Tong)>();

            for (DateTime d = fromDate; d <= toDate; d = d.AddDays(1))
            {
                decimal pos = hoaDonBLL.TinhDoanhThuTuNgayDenNgay(d, d);
                decimal san = thanhToanBLL.TinhDoanhThuTuNgayDenNgay(d, d);
                decimal tong = pos + san;
                data.Add((d, pos, san, tong));
            }

            double width = ChartCanvas.ActualWidth;
            double height = ChartCanvas.ActualHeight;
            if (width == 0 || height == 0) { width = 400; height = 200; }

            double maxValue = (double)data.Max(x => x.Tong);
            if (maxValue == 0) maxValue = 1;

            // Vẽ 3 đường: POS, Sân, Tổng
            DrawLine(data.Select(x => (x.Ngay, x.Pos)).ToList(), Brushes.Orange, width, height, maxValue);
            DrawLine(data.Select(x => (x.Ngay, x.San)).ToList(), Brushes.Green, width, height, maxValue);
            DrawLine(data.Select(x => (x.Ngay, x.Tong)).ToList(), Brushes.Blue, width, height, maxValue);

            // Vẽ nhãn ngày bên dưới
            int n = data.Count;
            // Số ngày tối đa có thể hiển thị (ví dụ: 8 nhãn)
            int maxLabels = 8;
            // Bước nhảy: Tính xem cần bỏ qua bao nhiêu ngày
            int step = 1;
            if (n > maxLabels)
            {
                // Chia tổng số ngày cho số nhãn tối đa mong muốn (làm tròn lên)
                step = (int)Math.Ceiling((double)n / maxLabels);
            }

            for (int i = 0; i < n; i++)
            {
                // Chỉ vẽ nhãn nếu 'i' là bội số của 'step'
                if (i % step != 0 && i != n - 1) // Luôn hiển thị ngày đầu tiên (i=0) và ngày cuối cùng (i=n-1) nếu cần
                {
                    continue;
                }

                double x = (width / (n - 1)) * i;
                // Đặt nhãn ở dưới cùng (height - offsetBottom + margin)
                TextBlock lbl = new TextBlock
                {
                    Text = data[i].Ngay.ToString("dd/MM"),
                    FontSize = 10,
                    Foreground = Brushes.Black
                };
                Canvas.SetLeft(lbl, x - (lbl.Text.Length * 3)); // Dịch trái để căn giữa gần đúng
                Canvas.SetTop(lbl, height - 20); // Đảm bảo vị trí Top nằm ngoài khu vực biểu đồ

                // Tránh lỗi khi n=1 (biểu đồ 1 ngày)
                if (n == 1) Canvas.SetLeft(lbl, (width / 2) - (lbl.Text.Length * 3));

                ChartCanvas.Children.Add(lbl);

            }
        }


        private void DrawLine(List<(DateTime Ngay, decimal GiaTri)> data, Brush color, double width, double height, double maxValue)
        {
            Polyline line = new Polyline
            {
                Stroke = color,
                StrokeThickness = 2
            };

            int n = data.Count;
            for (int i = 0; i < n; i++)
            {
                double x = (width / (n - 1)) * i;
                double offsetBottom = 30; // khoảng trống dành cho nhãn ngày
                double y = (height - offsetBottom) - (double)data[i].GiaTri / maxValue * (height - offsetBottom);
                line.Points.Add(new Point(x, y));
            }

            ChartCanvas.Children.Add(line);
        }
        private void XuatExcel_Click(object sender, RoutedEventArgs e)
        {
            DateTime? tuNgay = dpTuNgay.SelectedDate;
            DateTime? denNgay = dpDenNgay.SelectedDate;
            if (tuNgay == null || denNgay == null)
            {
                MessageBox.Show("Vui lòng chọn ngày trước khi xuất Excel.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Báo cáo");

                    // ===== HEADER CHUNG =====
                    ws.Cell(1, 1).Value = "SÂN CẦU LÔNG CỦ CHI";
                    ws.Range(1, 1, 1, 12).Merge();
                    ws.Cell(1, 1).Style.Font.Bold = true;
                    ws.Cell(1, 1).Style.Font.FontSize = 14;
                    ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(2, 1).Value = "72, Tỉnh lộ 16, Ấp Phú Thuận, Xã Phú Hòa Đông, TP. Hồ Chí Minh";
                    ws.Range(2, 1, 2, 12).Merge();
                    ws.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(3, 1).Value = "Tel: 0966752642";
                    ws.Range(3, 1, 3, 12).Merge();
                    ws.Cell(3, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(4, 1).Value = $"Từ ngày: {tuNgay:dd/MM/yyyy} Đến ngày: {denNgay:dd/MM/yyyy}";
                    ws.Range(4, 1, 4, 12).Merge();
                    ws.Cell(4, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(4, 1).Style.Font.Bold = true;

                    // ===== PHẦN 1: Doanh thu theo ngày (cột A–D) =====
                    int row = 6;
                    ws.Cell(row, 1).Value = "Ngày";
                    ws.Cell(row, 2).Value = "Doanh thu POS";
                    ws.Cell(row, 3).Value = "Doanh thu Sân";
                    ws.Cell(row, 4).Value = "Tổng";
                    ws.Range(row, 1, row, 4).Style.Font.Bold = true;
                    ws.Range(row, 1, row, 4).Style.Fill.BackgroundColor = XLColor.LightGray;
                    ws.Range(row, 1, row, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    row++;

                    ThanhToanBLL thanhToanBLL = new ThanhToanBLL();
                    HoaDonBLL hoaDonBLL = new HoaDonBLL();
                    for (DateTime d = tuNgay.Value; d <= denNgay.Value; d = d.AddDays(1))
                    {
                        decimal pos = hoaDonBLL.TinhDoanhThuTuNgayDenNgay(d, d);
                        decimal san = thanhToanBLL.TinhDoanhThuTuNgayDenNgay(d, d);
                        decimal tong = pos + san;

                        ws.Cell(row, 1).Value = d.ToString("dd/MM/yyyy");
                        ws.Cell(row, 2).Value = pos;
                        ws.Cell(row, 3).Value = san;
                        ws.Cell(row, 4).Value = tong;
                        row++;
                    }
                    // Kẻ border cho toàn bộ bảng doanh thu
                    ws.Range(6, 1, row - 1, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Range(6, 1, row - 1, 4).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    // ===== PHẦN 2: TopTimeSlots (cột F–I) =====
                    int row2 = 6;
                    ws.Cell(row2, 6).Value = "Khung giờ";
                    ws.Cell(row2, 7).Value = "Lượt đặt";
                    ws.Cell(row2, 8).Value = "Công suất (%)";
                    ws.Cell(row2, 9).Value = "Trạng thái";
                    ws.Range(row2, 6, row2, 9).Style.Font.Bold = true;
                    ws.Range(row2, 6, row2, 9).Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Range(row2, 6, row2, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    row2++;

                    foreach (var slot in TopTimeSlots)
                    {
                        ws.Cell(row2, 6).Value = slot.TimeRange;
                        ws.Cell(row2, 7).Value = slot.BookingsDisplay;
                        ws.Cell(row2, 8).Value = slot.UtilPercent;
                        ws.Cell(row2, 9).Value = slot.StatusText;
                        row2++;
                    }
                    // Kẻ border cho toàn bộ bảng TopTimeSlots
                    ws.Range(6, 6, row2 - 1, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Range(6, 6, row2 - 1, 9).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    // ===== PHẦN 3: KPI (cột K–L) =====
                    int row3 = 6;
                    ws.Cell(row3, 11).Value = "Chỉ số";
                    ws.Cell(row3, 12).Value = "Giá trị";
                    ws.Range(row3, 11, row3, 12).Style.Font.Bold = true;
                    ws.Range(row3, 11, row3, 12).Style.Fill.BackgroundColor = XLColor.LightGreen;
                    ws.Range(row3, 11, row3, 12).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    row3++;

                    ws.Cell(row3++, 11).Value = "Doanh thu sân"; ws.Cell(row3 - 1, 12).Value = txtDoanhThuSan.Text;
                    ws.Cell(row3++, 11).Value = "Doanh thu POS"; ws.Cell(row3 - 1, 12).Value = txtDoanhThuPOS.Text;
                    ws.Cell(row3++, 11).Value = "Tổng doanh thu"; ws.Cell(row3 - 1, 12).Value = txtTongDonhThu.Text;
                    ws.Cell(row3++, 11).Value = "Lượt khách"; ws.Cell(row3 - 1, 12).Value = txtLuotKhach.Text;
                    ws.Cell(row3++, 11).Value = "Tổng số đơn đặt"; ws.Cell(row3 - 1, 12).Value = txtTongSoDonDat.Text;
                    ws.Cell(row3++, 11).Value = "Tổng giờ chơi"; ws.Cell(row3 - 1, 12).Value = txtTongGioChoi.Text;
                    ws.Cell(row3++, 11).Value = "Giá trung bình/giờ"; ws.Cell(row3 - 1, 12).Value = txtGiaTrungBinhGio.Text;
                    ws.Cell(row3++, 11).Value = "Mặt hàng bán chạy"; ws.Cell(row3 - 1, 12).Value = txtMatHangBanChay.Text;
                    ws.Cell(row3++, 11).Value = "Giá trị TB POS"; ws.Cell(row3 - 1, 12).Value = txtGiaTriTrungBinhPOS.Text;
                    ws.Cell(row3++, 11).Value = "Số lượng POS"; ws.Cell(row3 - 1, 12).Value = txtSoLuongPOS.Text;
                    ws.Cell(row3++, 11).Value = "Hội viên mới"; ws.Cell(row3 - 1, 12).Value = txtHoiVienMoi.Text;
                    ws.Cell(row3++, 11).Value = "Điểm tích lũy phát hành"; ws.Cell(row3 - 1, 12).Value = txtDiemTichLuyPhatHanh.Text;
                    // Kẻ border cho toàn bộ bảng KPI
                    ws.Range(6, 11, row3 - 1, 12).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Range(6, 11, row3 - 1, 12).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    ws.Columns().AdjustToContents();

                    // ===== LƯU FILE =====
                    var sfd = new Microsoft.Win32.SaveFileDialog
                    {
                        Filter = "Excel Workbook|*.xlsx",
                        FileName = $"BaoCao_{tuNgay:yyyyMMdd}_{denNgay:yyyyMMdd}.xlsx"
                    };

                    if (sfd.ShowDialog() == true)
                    {
                        wb.SaveAs(sfd.FileName);
                        MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



    }


}