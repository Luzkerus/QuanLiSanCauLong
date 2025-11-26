using Microsoft.Win32;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
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
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using Microsoft.Win32;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.KhoPOS
{
    /// <summary>
    /// Interaction logic for frmLichSuPOS.xaml
    /// </summary>
    public partial class frmLichSuPOS : Window
    {
        HoaDonBLL hoaDonBLL = new HoaDonBLL();
        private object _lastSelectedHoaDon = null;
        public frmLichSuPOS()
        {
            InitializeComponent();
            // DataContext = new LichSuPOSViewModel(); // nếu bạn có VM
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnLoc_Click(object sender, RoutedEventArgs e)
        {
            DateTime fromDate = dpTuNgay.SelectedDate ?? DateTime.MinValue;
            DateTime toDate = dpDenNgay.SelectedDate ?? DateTime.MaxValue;
            if (fromDate > toDate)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string timKiem = (cbSearch.Text?.Trim().ToLower() == "tất cả") ? "" : cbSearch.Text?.Trim().ToLower() ?? "";
            var list = hoaDonBLL.LayHoaDonTheoNgay(fromDate, toDate).AsEnumerable();
            if (!string.IsNullOrWhiteSpace(timKiem))
            {
                list = list.Where(hd =>
                         (!string.IsNullOrEmpty(hd.PhuongThuc) && hd.PhuongThuc.ToLower().Contains(timKiem))
                );
            }
            var finalList = list.ToList();
            dgHoaDon.ItemsSource = finalList;
            var soHDNlist = finalList.Select(hd => hd.SoHDN).ToList();
            var chiTietList = hoaDonBLL.LayChiTietHoaDonTheoSoHDN(soHDNlist);
            dgChiTietHoaDon.ItemsSource = chiTietList;
            HoaDonTextBlock.Text = "Chi tiết hóa đơn";
            _lastSelectedHoaDon = null;
            txtTongTien.Text = "";
        }

        private void btnXoaLoc_Click(object sender, RoutedEventArgs e)
        {
            dpDenNgay.SelectedDate = null;
            dpTuNgay.SelectedDate = null;
            cbSearch.SelectedIndex = 0;
            dgChiTietHoaDon.ItemsSource = null;
            dgHoaDon.ItemsSource = null;
            HoaDonTextBlock.Text = "Chi tiết hóa đơn";
            _lastSelectedHoaDon = null;
            txtTongTien.Text = "";
        }
        private void dgHoaDon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lấy item đang được chọn trong DataGrid
            var currentSelectedHoaDon = dgHoaDon.SelectedItem;

            // Kiểm tra xem người dùng có đang chọn lại chính item đó không
            if (currentSelectedHoaDon != null && currentSelectedHoaDon.Equals(_lastSelectedHoaDon))
            {
                // Hành động 1: Bỏ chọn (Unselect) item đó
                dgHoaDon.SelectedItem = null;

                // Cập nhật TextBlock và DataGrid chi tiết
                HoaDonTextBlock.Text = "Chi tiết hóa đơn";
                dgChiTietHoaDon.ItemsSource = null;

                // Reset biến trạng thái
                _lastSelectedHoaDon = null;
            }
            else if (currentSelectedHoaDon != null)
            {
                // Hành động 2: Chọn một item mới (hoặc lần đầu tiên)

                // Lấy thông tin hóa đơn. Giả sử hóa đơn là một đối tượng có thuộc tính SoHDN
                // Bạn cần thay đổi kiểu dữ liệu (var) thành kiểu đối tượng thực tế của hóa đơn (vd: HoaDon)
                dynamic hoaDon = currentSelectedHoaDon;
                string soHdn = hoaDon.SoHDN;

                // Cập nhật TextBlock
                HoaDonTextBlock.Text = $"Chi tiết hóa đơn {soHdn}";
                txtTongTien.Text = hoaDon.TongTien.ToString("N0") + " đ";
                // Lấy và hiển thị chi tiết hóa đơn
                var chiTietList = hoaDonBLL.LayChiTietHoaDonTheoSoHDN(new List<string> { soHdn });
                dgChiTietHoaDon.ItemsSource = chiTietList;

                // Cập nhật biến trạng thái
                _lastSelectedHoaDon = currentSelectedHoaDon;
            }
            else
            {
                // Khi không có gì được chọn (ví dụ: sau khi gọi dgHoaDon.SelectedItem = null)
                HoaDonTextBlock.Text = "Chi tiết hóa đơn";
                dgChiTietHoaDon.ItemsSource = null;
                _lastSelectedHoaDon = null;
            }
        }
        private void BtnInLaiHoaDon_Click(object sender, RoutedEventArgs e)
        {
            var selectedHoaDon = dgHoaDon.SelectedItem as HoaDon; // Thay HoaDon bằng kiểu dữ liệu thực tế

            if (selectedHoaDon == null)
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn cần in lại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 1. Lấy số hóa đơn
            string soHdn = selectedHoaDon.SoHDN;

            // 2. Lấy chi tiết hóa đơn từ BLL (Sử dụng phương thức đã có)
            List<string> soHDNlist = new List<string> { soHdn };
            var chiTietList = hoaDonBLL.LayChiTietHoaDonTheoSoHDN(soHDNlist); // Giả sử phương thức này trả về List<ChiTietHoaDon>

            if (chiTietList == null || chiTietList.Count == 0)
            {
                MessageBox.Show("Không tìm thấy chi tiết hóa đơn này. Không thể in.", "Lỗi dữ liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. Gọi phương thức in PDF mới
            try
            {
                InHoaDon(selectedHoaDon, chiTietList.OfType<ChiTietHoaDon>().ToList()); // Ép kiểu nếu cần
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo PDF: {ex.Message}", "Lỗi In ấn", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
            // Trong frmLichSuPOS.xaml.cs (hoặc một lớp Helper khác)
        private void InHoaDon(HoaDon hoaDon, List<ChiTietHoaDon> chiTietList)
                {
                    // Cần đảm bảo thư viện PdfSharpCore được sử dụng:
        // Dùng cho SaveFileDialog

                    // Tạo PDF
                    PdfDocument document = new PdfDocument();
                    document.Info.Title = "Phiếu Thanh Toán";

                    // Sử dụng khổ A6
                    PdfPage page = document.AddPage();
                    page.Size = PdfSharpCore.PageSize.A6;

                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XFont fontTitle = new XFont("Arial", 16, XFontStyle.Bold);
                    XFont fontNormal = new XFont("Arial", 10);
                    XFont fontSmall = new XFont("Arial", 8);

                    double pageWidth = page.Width;
                    double y = 20;

                    // Tương tự code cũ, nhưng sử dụng biến 'hoaDon' và 'chiTietList'
                    // ... (Phần Header, Thông tin cửa hàng, Số hóa đơn, Ngày) ...

                    // ===== Header =====
                    gfx.DrawString("PHIẾU THANH TOÁN", fontTitle, XBrushes.Black,
                        new XRect(0, y, pageWidth, 20), XStringFormats.TopCenter);
                    y += 30;

                    gfx.DrawString("SÂN CẦU LÔNG CỦ CHI", fontNormal, XBrushes.Black,
                        new XRect(0, y, pageWidth, 20), XStringFormats.TopCenter);
                    y += 20;

                    gfx.DrawString("72, Tỉnh lộ 15, Ấp Phú Thuận, Xã Phú Hòa Đông, TP. Hồ Chí Minh", fontSmall, XBrushes.Black,
                        new XRect(0, y, pageWidth, 20), XStringFormats.TopCenter);
                    y += 15;

                    gfx.DrawString("Tel: 0966752642", fontSmall, XBrushes.Black,
                        new XRect(0, y, pageWidth, 20), XStringFormats.TopCenter);
                    y += 25;

                    // ===== Thông tin hóa đơn (Sử dụng dữ liệu từ đối tượng hoaDon) =====
                    gfx.DrawString($"Số hóa đơn: {hoaDon.SoHDN}", fontNormal, XBrushes.Black,
                                new XRect(10, y, pageWidth - 20, 20), XStringFormats.TopLeft);
                    y += 20;

                    gfx.DrawString($"Ngày: {hoaDon.Ngay:dd/MM/yyyy HH:mm}", fontNormal, XBrushes.Black,
                                    new XRect(10, y, pageWidth - 20, 20), XStringFormats.TopLeft);
                    y += 25;


                    // ===== Danh sách sản phẩm =====
                    double startX = 10;
                    double startY = y;
                    double col1 = 150 / 2;
                    double col2 = 40;
                    double col3 = 80;
                    double col4 = 80;
                    double rowHeight = 20;

                    // Vẽ header bảng (giống code cũ)
                    gfx.DrawRectangle(XPens.Black, startX, startY, col1 + col2 + col3 + col4, rowHeight);
                    gfx.DrawLine(XPens.Black, startX + col1, startY, startX + col1, startY + rowHeight);
                    gfx.DrawLine(XPens.Black, startX + col1 + col2, startY, startX + col1 + col2, startY + rowHeight);
                    gfx.DrawLine(XPens.Black, startX + col1 + col2 + col3, startY, startX + col1 + col2 + col3, startY + rowHeight);

                    gfx.DrawString("Sản phẩm", fontNormal, XBrushes.Black, new XRect(startX + 2, startY + 2, col1, rowHeight), XStringFormats.TopLeft);
                    gfx.DrawString("SL", fontNormal, XBrushes.Black, new XRect(startX + col1 + 2, startY + 2, col2, rowHeight), XStringFormats.TopLeft);
                    gfx.DrawString("Đơn giá", fontNormal, XBrushes.Black, new XRect(startX + col1 + col2 + 2, startY + 2, col3, rowHeight), XStringFormats.TopLeft);
                    gfx.DrawString("Thành tiền", fontNormal, XBrushes.Black, new XRect(startX + col1 + col2 + col3 + 2, startY + 2, col4, rowHeight), XStringFormats.TopLeft);

                    y += rowHeight;

                    // ===== Vẽ từng dòng (Sử dụng chiTietList) =====
                    foreach (ChiTietHoaDon item in chiTietList) // Lấy từ biến chiTietList
                    {
                        // Vẽ khung hàng
                        gfx.DrawRectangle(XPens.Black, startX, y, col1 + col2 + col3 + col4, rowHeight);

                        // Vẽ các đường kẻ cột
                        gfx.DrawLine(XPens.Black, startX + col1, y, startX + col1, y + rowHeight);
                        gfx.DrawLine(XPens.Black, startX + col1 + col2, y, startX + col1 + col2, y + rowHeight);
                        gfx.DrawLine(XPens.Black, startX + col1 + col2 + col3, y, startX + col1 + col2 + col3, y + rowHeight);

                        // Vẽ nội dung từng cột
                        gfx.DrawString(item.TenHang, fontNormal, XBrushes.Black, new XRect(startX + 2, y + 2, col1, rowHeight), XStringFormats.TopLeft);
                        gfx.DrawString(item.SoLuong.ToString(), fontNormal, XBrushes.Black, new XRect(startX + col1 + 2, y + 2, col2, rowHeight), XStringFormats.TopLeft);
                        gfx.DrawString($"{item.GiaBan:N0}đ", fontNormal, XBrushes.Black, new XRect(startX + col1 + col2 + 2, y + 2, col3, rowHeight), XStringFormats.TopLeft);
                        gfx.DrawString($"{item.ThanhTien:N0}đ", fontNormal, XBrushes.Black, new XRect(startX + col1 + col2 + col3 + 2, y + 2, col4, rowHeight), XStringFormats.TopLeft);

                        y += rowHeight;
                    }

                    y += 5;
                    gfx.DrawLine(XPens.Black, 10, y, pageWidth - 10, y);
                    y += 10;

                    // ===== Tổng tiền =====
                    gfx.DrawString($"Tổng cộng: {hoaDon.TongTien:N0} đ", fontNormal, XBrushes.Black,
                        new XRect(10, y, pageWidth - 20, 20), XStringFormats.TopLeft);
                    y += 20;

                    // ===== Phương thức thanh toán =====
                    gfx.DrawString($"Phương thức: {hoaDon.PhuongThuc}", fontNormal, XBrushes.Black, new XRect(10, y, pageWidth - 20, 20), XStringFormats.TopLeft);

                    double footerHeight = 40;
                    double footerY = page.Height - footerHeight;

                    // Vẽ dòng cảm ơn
                    gfx.DrawString(
                        "CẢM ƠN VÀ HẸN GẶP LẠI Ở LẦN CHƠI TIẾP THEO!",
                        fontNormal,
                        XBrushes.Black,
                        new XRect(0, footerY, pageWidth, 20),
                        XStringFormats.TopCenter
                    );

                    // Vẽ mã hóa đơn + ngày dưới dòng cảm ơn
                    string hdCode = $"#HD-{hoaDon.Ngay:ddMMyyyyHHmm}_{hoaDon.SoHDN}";
                    gfx.DrawString(
                        $"{hoaDon.Ngay:dd/MM/yyyy HH:mm}    {hdCode}",
                        fontSmall,
                        XBrushes.Black,
                        new XRect(0, footerY + 20, pageWidth, 20),
                        XStringFormats.TopCenter
                    );

                    // ===== Lưu PDF =====
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.Title = "Lưu hóa đơn PDF";
                    dlg.Filter = "PDF File|*.pdf";
                    dlg.FileName = $"HoaDon_{hoaDon.SoHDN}.pdf";

                    if (dlg.ShowDialog() == true)
                    {
                        document.Save(dlg.FileName);
                        MessageBox.Show("Đã lưu hóa đơn thành công!");
                    }
                }
    }
    

}
