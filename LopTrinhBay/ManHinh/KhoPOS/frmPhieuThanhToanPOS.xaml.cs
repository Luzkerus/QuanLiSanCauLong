using Microsoft.Win32;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopNghiepVu;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.ThanhToan;
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
    /// Interaction logic for frmPhieuThanhToanPOS.xaml
    /// </summary>
    public partial class frmPhieuThanhToanPOS : Window
    {
        HoaDonBLL hoaDonBLL = new HoaDonBLL();
        private bool daThanhToan = false;
        public frmPhieuThanhToanPOS(List<ChiTietHoaDon> gioHang)
        {
            InitializeComponent();
            dgGioHang.ItemsSource = gioHang;
            txtNgay.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            txtSoHDN.Text = hoaDonBLL.TaoSoHDN();
            txtTongTien.Text = gioHang.Sum(item => item.ThanhTien).ToString("N0");
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void LuuHoaDon()
        {
            if (daThanhToan) return;

            string phuongThuc = rbTienMat.IsChecked == true ? "Tiền mặt" :
                                 rbBank.IsChecked == true ? "Ngân hàng" :
                                 "MoMo";

            HoaDon hd = new HoaDon
            {
                SoHDN = txtSoHDN.Text,
                Ngay = DateTime.Now,
                TongTien = decimal.Parse(txtTongTien.Text),
                PhuongThuc = phuongThuc
            };

            List<ChiTietHoaDon> chiTiet = dgGioHang.ItemsSource.Cast<ChiTietHoaDon>().ToList();

            hoaDonBLL.ThemHoaDon(hd, chiTiet);
            hoaDonBLL.CapNhatTonKhoSauKhiBan(chiTiet);

            daThanhToan = true;
        }
        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var result =  MessageBox.Show("Bạn có chắc muốn xác nhận thanh toán không in hóa ?",
                                         "Xác nhận",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                LuuHoaDon();
                MessageBox.Show("Lưu hóa đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
        }
        private void btnIn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có muốn thanh toán và in hóa đơn?",
                                         "In hóa đơn",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                if (!daThanhToan)
                {
                    LuuHoaDon(); // lưu hóa đơn trước khi in, nhưng không đóng form
                }
                TaoPDF(); // gọi hàm tạo PDF
                this.DialogResult = true;
                this.Close();
            }
        }

        private void TaoPDF()
        {


            // Tạo PDF
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Phiếu Thanh Toán";

            PdfPage page = document.AddPage();
            page.Size = PdfSharpCore.PageSize.A6;

            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont fontTitle = new XFont("Arial", 16, XFontStyle.Bold);
            XFont fontNormal = new XFont("Arial", 10);
            XFont fontSmall = new XFont("Arial", 8);

            double pageWidth = page.Width;
            double y = 20;

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

            // ===== Thông tin hóa đơn =====
            gfx.DrawString($"Số hóa đơn: {txtSoHDN.Text}", fontNormal, XBrushes.Black,
                      new XRect(10, y, pageWidth - 20, 20), XStringFormats.TopLeft);
            y += 20;

            gfx.DrawString($"Ngày: {txtNgay.Text}", fontNormal, XBrushes.Black,
                           new XRect(10, y, pageWidth - 20, 20), XStringFormats.TopLeft);
            y += 25;

            // ===== Danh sách sản phẩm =====
            double startX = 10;
            double startY = y;
            double col1 = 150/2; // độ rộng cột Sản phẩm
            double col2 = 40;  // SL
            double col3 = 80;  // Đơn giá
            double col4 = 80;  // Thành tiền
            double rowHeight = 20;

            // ===== Vẽ header =====
            // Vẽ header với các đường kẻ cột
            gfx.DrawRectangle(XPens.Black, startX, startY, col1 + col2 + col3 + col4, rowHeight);
            gfx.DrawLine(XPens.Black, startX + col1, startY, startX + col1, startY + rowHeight); // cột SL
            gfx.DrawLine(XPens.Black, startX + col1 + col2, startY, startX + col1 + col2, startY + rowHeight); // cột Đơn giá
            gfx.DrawLine(XPens.Black, startX + col1 + col2 + col3, startY, startX + col1 + col2 + col3, startY + rowHeight); // cột Thành tiền

            gfx.DrawString("Sản phẩm", fontNormal, XBrushes.Black, new XRect(startX + 2, startY + 2, col1, rowHeight), XStringFormats.TopLeft);
            gfx.DrawString("SL", fontNormal, XBrushes.Black, new XRect(startX + col1 + 2, startY + 2, col2, rowHeight), XStringFormats.TopLeft);
            gfx.DrawString("Đơn giá", fontNormal, XBrushes.Black, new XRect(startX + col1 + col2 + 2, startY + 2, col3, rowHeight), XStringFormats.TopLeft);
            gfx.DrawString("Thành tiền", fontNormal, XBrushes.Black, new XRect(startX + col1 + col2 + col3 + 2, startY + 2, col4, rowHeight), XStringFormats.TopLeft);

            y += rowHeight;

            // ===== Vẽ từng dòng =====
            foreach (ChiTietHoaDon item in dgGioHang.ItemsSource)
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
            gfx.DrawString($"Tổng cộng: {txtTongTien.Text} đ", fontNormal, XBrushes.Black,
                new XRect(10, y, pageWidth - 20, 20), XStringFormats.TopLeft);
            y += 20;

            // ===== Phương thức thanh toán =====
            string phuongThuc = "Tiền mặt";
            if (rbBank.IsChecked == true) phuongThuc = "Ngân hàng";
            else if (rbMoMo.IsChecked == true) phuongThuc = "MoMo";

            gfx.DrawString($"Phương thức: {phuongThuc}", fontNormal, XBrushes.Black, new XRect(10, y, pageWidth - 20, 20), XStringFormats.TopLeft);

            double footerHeight = 40; // Dự kiến chừa chỗ cho 2 dòng footer
            double footerY = page.Height - footerHeight; // Vị trí y bắt đầu footer

            // Vẽ dòng cảm ơn
            gfx.DrawString(
                "CẢM ƠN VÀ HẸN GẶP LẠI Ở LẦN CHƠI TIẾP THEO!",
                fontNormal,
                XBrushes.Black,
                new XRect(0, footerY, pageWidth, 20),
                XStringFormats.TopCenter
            );

            // Vẽ mã hóa đơn + ngày dưới dòng cảm ơn
            string hdCode = $"#HD-{txtNgay.Text}_{txtSoHDN.Text}";
            gfx.DrawString(
                $"{txtNgay.Text:dd/MM/yyyy HH:mm}   {hdCode}",
                fontSmall,
                XBrushes.Black,
                new XRect(0, footerY + 20, pageWidth, 20),
                XStringFormats.TopCenter
            );

            // ===== Lưu PDF =====
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Lưu hóa đơn PDF";
            dlg.Filter = "PDF File|*.pdf";
            dlg.FileName = $"HoaDon_{txtSoHDN.Text}.pdf";

            if (dlg.ShowDialog() == true)
            {
                document.Save(dlg.FileName);
                MessageBox.Show("Đã lưu hóa đơn thành công!");
            }

        }

    }
}
