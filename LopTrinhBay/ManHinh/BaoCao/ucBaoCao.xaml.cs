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
            for (int i = 0; i < n; i++)
            {
                double x = (width / (n - 1)) * i;
                // đặt nhãn ở dưới cùng (height + margin)
                TextBlock lbl = new TextBlock
                {
                    Text = data[i].Ngay.ToString("dd/MM"),
                    FontSize = 10,
                    Foreground = Brushes.Black
                };
                Canvas.SetLeft(lbl, x - 15); // dịch trái một chút để căn giữa
                Canvas.SetTop(lbl, height - 20);

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

    }


}





