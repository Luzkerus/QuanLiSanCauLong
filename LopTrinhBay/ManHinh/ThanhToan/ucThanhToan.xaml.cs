using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Win32;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopNghiepVu;
using QuanLiSanCauLong.LopTruyCapDuLieu;
using System;
using System.Collections.Generic;
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
//using System.Windows.Shapes;


namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.ThanhToan
{
    /// <summary>
    /// Interaction logic for ucThanhToan.xaml
    /// </summary>
    public partial class ucThanhToan : UserControl
    {
        private readonly KhachHangBLL khBLL = new KhachHangBLL();
        private QuanLiSanCauLong.LopDuLieu.ThanhToan currentHD;
        private List<dynamic> selectedDetails;
        private ThanhToanBLL bll = new ThanhToanBLL();
        public ucThanhToan()
        {
            InitializeComponent();
            
            LoadData();
        }
        private void LoadData()
        {
            txtDoanhThuHomNay.Text = bll.LayDoanhThuHomNay().ToString("N0") + " VNĐ";
            txtDoanhThuThang.Text = bll.LayDoanhThuThang().ToString("N0") + " VNĐ"; 
            txtSoHoaDonHomNay.Text = bll.LayTongHoaDonHomNay().ToString();
            txtTongDonThang.Text = bll.LayTongHoaDonThang().ToString();
            donGiaThueVot = 35000; // ví dụ
            txtDonGiaThueVot.Text = donGiaThueVot.ToString();
        }
        private void btnLichSuThanhToan_Click(object sender, RoutedEventArgs e)
        {
            var frm = new frmLichSuThanhToan();
            frm.ShowDialog();
        }
        private void txtSDT_TextChanged(object sender, RoutedEventArgs e)
        {
            LoadKhachHang(txtSDT.Text);
            LoadChiTietChuaThanhToan(txtSDT.Text);
        }
        private void LoadChiTietChuaThanhToan(string sdt)
        {
            var chiTietBLL = new ChiTietDatSanBLL();
            var danhSach = chiTietBLL.LayDanhSachChuaThanhToan(sdt);
            dataGridChuaThanhToan.ItemsSource = danhSach;
            TinhVaHienThiTongTienSan();
        }
        private void LoadKhachHang(string sdt)
        {
            if (string.IsNullOrEmpty(sdt))
            {
                txtTenKH.Text = "";
                return;
            }

            var kh = khBLL.LayKhachHangTheoSDT(sdt);
            if (kh != null)
            {
                txtTenKH.Text = kh.Ten;
            }
            else
            {
                txtTenKH.Text = "";
            }
        }

        private void TinhVaHienThiTongTienSan()
        {
            // Lấy nguồn dữ liệu từ DataGrid
            if (dataGridChuaThanhToan.ItemsSource is IEnumerable<dynamic> danhSachChiTiet)
            {
                decimal tongTienSan = 0;

                // Lặp qua các mục và tính tổng nếu mục đó được check
                foreach (var chiTiet in danhSachChiTiet)
                {
                    // Kiểm tra xem mục có thuộc tính IsChecked và giá trị là true không
                    bool isChecked = false;
                    try
                    {
                        isChecked = chiTiet.IsChecked; // Giả định có thuộc tính IsChecked
                    }
                    catch (RuntimeBinderException)
                    {
                        // Xử lý nếu thuộc tính không tồn tại, có thể bỏ qua hoặc báo lỗi
                        // Trong trường hợp này, nếu không có IsChecked, ta bỏ qua mục đó.
                    }

                    if (isChecked)
                    {
                        decimal thanhTien = 0;
                        try
                        {
                            thanhTien = chiTiet.ThanhTien; // Giả định có thuộc tính ThanhTien
                        }
                        catch (RuntimeBinderException)
                        {
                            // Xử lý nếu thuộc tính không tồn tại
                        }

                        tongTienSan += thanhTien;
                    }
                }

                // Hiển thị tổng tiền
                txtTongTienSan.Text = string.Format("{0:N0} VNĐ", tongTienSan);
                CapNhatTongTien();
            }
            else
            {
                txtTongTienSan.Text = "0 VNĐ";
            }
        }
        private void Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            TinhVaHienThiTongTienSan(); // Tính lại ngay
        }

        private void Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            TinhVaHienThiTongTienSan(); // Tính lại ngay
        }

        // Event cho "Chọn tất cả" (nếu dùng checkbox ở header)
        private void ChkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            if (dataGridChuaThanhToan.ItemsSource is IEnumerable<dynamic> items)
            {
                foreach (var item in items)
                {
                    try { item.IsChecked = true; }
                    catch { }
                }
            }

            dataGridChuaThanhToan.Items.Refresh();  // ⭐ THÊM DÒNG NÀY
            TinhVaHienThiTongTienSan();
        }

        private void ChkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            if (dataGridChuaThanhToan.ItemsSource is IEnumerable<dynamic> items)
            {
                foreach (var item in items)
                {
                    try { item.IsChecked = false; }
                    catch { }
                }
            }

            dataGridChuaThanhToan.Items.Refresh();  // ⭐ THÊM DÒNG NÀY
            TinhVaHienThiTongTienSan();
        }


        // Event thay thế nếu dùng RowEditEnding (cho trường hợp checkbox không trigger trực tiếp)
        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            TinhVaHienThiTongTienSan(); // Gọi khi edit row kết thúc (bao gồm check)
        }
        // Biến lưu trữ số lượng vợt thuê và đơn giá thuê vợt
        private int soLuongVot = 0;
        private decimal donGiaThueVot = 0;
        private decimal tongTienThueVot = 0;
        private void TinhTienThueVot()
        {
            tongTienThueVot = soLuongVot * donGiaThueVot;
            txtTongTienThueVot.Text = string.Format("{0:N0} VNĐ", tongTienThueVot);
            CapNhatTongTien();
        }
        private void BtnTangVot_Click(object sender, RoutedEventArgs e)
        {
            soLuongVot++;
            txtSoLuongVot.Text = soLuongVot.ToString();
            TinhTienThueVot();
        }
        private void BtnDatLaiVot()
        {
            soLuongVot = 0;
            txtSoLuongVot.Text = soLuongVot.ToString();
            TinhTienThueVot();
        }

        private void BtnGiamVot_Click(object sender, RoutedEventArgs e)
        {
            if (soLuongVot > 0)
                soLuongVot--;

            txtSoLuongVot.Text = soLuongVot.ToString();
            TinhTienThueVot();
        }
        private void txtDonGiaThueVot_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(txtDonGiaThueVot.Text, out decimal value))
            {
                donGiaThueVot = value;
                TinhTienThueVot();
            }
        }
        private void CapNhatTongTien()
        {
            // 1. Tính tổng tiền sân
            decimal tongTienSan = 0;
            if (dataGridChuaThanhToan.ItemsSource is IEnumerable<dynamic> danhSachChiTiet)
            {
                foreach (var chiTiet in danhSachChiTiet)
                {
                    bool isChecked = false;
                    try { isChecked = chiTiet.IsChecked; } catch { }
                    if (isChecked)
                    {
                        decimal thanhTien = 0;
                        try { thanhTien = chiTiet.ThanhTien; } catch { }
                        tongTienSan += thanhTien;
                    }
                }
            }

            // 2. Tính tổng tiền thuê vợt
            decimal tongTienVot = soLuongVot * donGiaThueVot;

            // 3. Tổng tiền = sân + thuê vợt
            decimal tongTien = tongTienSan + tongTienVot;

            // 4. Hiển thị ra TextBlock
            txtTongTien.Text = string.Format("{0:N0} VNĐ", tongTien);
        }

        private void cbPhuongThuc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;

            var item = cbPhuongThuc.SelectedItem as ComboBoxItem;
            var text = (item?.Content as string) ?? string.Empty;

            // đổi đường dẫn cho đúng thư mục ảnh của bạn
            string uri = null;

            if (text == "Momo")
            {
                uri = "/LopTrinhBay/TaiNguyen/ThanhToan/momo.jpg";
            }
            else if (text == "Ngân hàng")
            {
                uri = "/LopTrinhBay/TaiNguyen/ThanhToan/nganhang.jpg";
            }

            if (uri == null)
            {
                imgQR.Source = null;   // Tiền mặt thì không hiện QR
            }
            else
            {
                imgQR.Source = new BitmapImage(new Uri(uri, UriKind.Relative));
            }
        }

        private void btnThanhToan_Click(object sender, RoutedEventArgs e)
        {
            
            string sdt = txtSDT.Text;
            var kh = khBLL.LayKhachHangTheoSDT(sdt);
            var tenKH = txtTenKH.Text;
           
            if (string.IsNullOrEmpty(sdt))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại và tên khách hàng.");
                return;
            }
            if (decimal.Parse(txtTongTien.Text.Replace(" VNĐ", "").Replace(",", "")) <= 0)
            {
                MessageBox.Show("Không có mục nào được chọn để thanh toán.");
                return;
            }
           
            if (!KiemTraSDT(sdt))
            {
                MessageBox.Show("Số điện thoại không hợp lệ! Vui lòng nhập lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string thongBao = $"Xác nhận thanh toán tổng cộng {txtTongTien.Text} cho khách hàng {tenKH} ({sdt})?";
            MessageBoxResult result = MessageBox.Show(thongBao, "Xác nhận Thanh Toán", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                // Người dùng chọn 'No', hủy thao tác
                return;
            }

            // Nếu khách hàng chưa có, tạo mới
            if (kh == null)
            {
                kh = new KhachHang
                {
                    SDT = sdt,
                    Ten = string.IsNullOrEmpty(tenKH) ? "Khách mới" : tenKH,
                };

                bool taoKH = khBLL.ThemKhachHangMoi(kh); // cần phương thức trong BLL để tạo khách
                if (!taoKH)
                {
                    MessageBox.Show("Không thể tạo khách hàng mới!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            var hd = new QuanLiSanCauLong.LopDuLieu.ThanhToan
            {
                SoHD = bll.TaoSoHoaDonMoi(),
                SDT = txtSDT.Text,
                TenKH = kh.Ten,
                NgayLap = DateTime.Now,
                TongTienSan = decimal.Parse(txtTongTienSan.Text.Replace(" VNĐ", "").Replace(",", "")),
                TongTienThueVot = tongTienThueVot,
                TongTien = decimal.Parse(txtTongTien.Text.Replace(" VNĐ", "").Replace(",", "")),
                PhuongThuc = (cbPhuongThuc.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Tiền mặt"
            };
            if (bll.LuuHoaDon(hd))
            {
                currentHD = hd;
                btnTaoHD.IsEnabled = true;

                // ⭐ luôn khởi tạo selectedDetails (tránh null)
                selectedDetails = new List<dynamic>();

                // Lấy chi tiết được chọn
                var chiTietDAL = new ChiTietDatSanDAL();
                List<string> danhSachMaChiTiet = new List<string>();

                if (dataGridChuaThanhToan.ItemsSource is IEnumerable<dynamic> danhSachChiTiet)
                {
                    foreach (var ct in danhSachChiTiet)
                    {
                        bool isChecked = false;
                        try { isChecked = ct.IsChecked; } catch { }

                        if (isChecked)
                        {
                            // ⭐ Lưu lại chi tiết được chọn để tạo PDF
                            selectedDetails.Add(ct);

                            // ⭐ Lưu MaPhieu để update DB
                            try { danhSachMaChiTiet.Add(ct.MaPhieu); } catch { }
                        }
                    }
                }

                // ⭐ Chỉ update trạng thái nếu có mục được chọn
                if (danhSachMaChiTiet.Count > 0)
                {
                    chiTietDAL.CapNhatTrangThaiDanhSach(danhSachMaChiTiet, "Đã thanh toán");
                }

                MessageBox.Show("Lưu hóa đơn thành công!");

                // ⭐ Refresh lại danh sách sân
                LoadChiTietChuaThanhToan(txtSDT.Text);
                LoadData();
            }

        }
        private bool KiemTraSDT(string sdt)
        {
            if (string.IsNullOrWhiteSpace(sdt)) return false;

            // Chỉ chứa số
            if (!sdt.All(char.IsDigit))
                return false;

            // Độ dài hợp lệ
            return sdt.Length >= 9 && sdt.Length <= 11;
        }
        private void btnTaoHD_Click(object sender, RoutedEventArgs e)
        {
            if (currentHD == null || selectedDetails == null)
            {
                MessageBox.Show("Chưa có hóa đơn hợp lệ để tạo PDF. Vui lòng thực hiện thanh toán trước.");
                return;
            }
            try
            {
                TaoHoaDonPDF(currentHD, selectedDetails, soLuongVot, donGiaThueVot);
                // Reset sau khi tạo
                currentHD = null;
                selectedDetails = null;
                btnTaoHD.IsEnabled = false;
                soLuongVot = 0;
                txtSoLuongVot.Text = "0";
                TinhTienThueVot();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo PDF: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TaoHoaDonPDF(
            QuanLiSanCauLong.LopDuLieu.ThanhToan hd,
            List<dynamic> chiTiets,
            int soLuongVot,
            decimal donGiaVot)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Phiếu Thanh Toán";
            PdfPage page = document.AddPage();
            page.Size = PdfSharpCore.PageSize.A5;

            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont fontTitle = new XFont("Arial", 18, XFontStyle.Bold);
            XPen penLine = new XPen(XColors.Gray, 0.6);
            XFont fontHeader = new XFont("Arial", 11, XFontStyle.Bold);
            XFont fontNormal = new XFont("Arial", 10);
            XFont fontSmall = new XFont("Arial", 8);
            XFont fontBold10 = new XFont("Arial", 10, XFontStyle.Bold);
            XFont fontItalic9 = new XFont("Arial", 9, XFontStyle.Italic);

            double pageWidth = page.Width;
            double marginLeft = 40;
            double marginRight = 40;
            double y = 20;

            // ======================= LOGO + HEADER ==========================
            double logoWidth = 70;
            double logoHeight = 0;

            try
            {
                string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                string logoPath = Path.Combine(exeDir, "TaiNguyen", "Logo", "sancaulongcuchi-logo.png");
                if (!File.Exists(logoPath))
                    logoPath = Path.Combine(exeDir, "LopTrinhBay", "TaiNguyen", "Logo", "sancaulongcuchi-logo.png");

                if (File.Exists(logoPath))
                {
                    XImage logo = XImage.FromFile(logoPath);
                    logoHeight = logo.PixelHeight * logoWidth / logo.PixelWidth;
                    double logoX = (pageWidth - logoWidth) / 2;
                    gfx.DrawImage(logo, logoX, y, logoWidth, logoHeight);
                    y += logoHeight + 8;
                }
                else
                {
                    logoHeight = 40;
                    y += logoHeight;
                }
            }
            catch
            {
                y += 40;
            }

            gfx.DrawString("SÂN CẦU LÔNG CỦ CHI", fontHeader, XBrushes.Black,
                new XRect(0, y, pageWidth, 14), XStringFormats.TopCenter);
            y += 14;

            gfx.DrawString("72, Tỉnh lộ 15, Ấp Phú Thuận, Xã Phú Hòa Đông, TP. Hồ Chí Minh",
                fontSmall, XBrushes.Black,
                new XRect(0, y, pageWidth, 12), XStringFormats.TopCenter);
            y += 12;

            gfx.DrawString("Tel: 0966752642", fontSmall, XBrushes.Black,
                new XRect(0, y, pageWidth, 12), XStringFormats.TopCenter);
            y += 16;

            // gạch ngang
            gfx.DrawLine(XPens.Gray, marginLeft, y, pageWidth - marginRight, y);
            y += 22;

            // ======================= TIÊU ĐỀ PHIẾU ==========================
            gfx.DrawString("PHIẾU THANH TOÁN", fontTitle, XBrushes.Black,
                new XRect(0, y, pageWidth, 20), XStringFormats.TopCenter);
            y += 30;

            // ======================= THÔNG TIN CHUNG (2 CỘT) =================
            double colMidX = pageWidth / 2 + 5;
            double leftX = marginLeft;
            double rightX = colMidX;

            string ngayVao = hd.NgayLap.ToString("dd.MM.yyyy HH:mm"); // bạn có thể thay bằng giờ vào thực tế
            string ngayRa = hd.NgayLap.ToString("dd.MM.yyyy HH:mm"); // hoặc dùng thuộc tính khác

            gfx.DrawString("NGÀY/ GIỜ VÀO:", fontSmall, XBrushes.Black, new XPoint(leftX, y));
            gfx.DrawString(ngayVao, fontSmall, XBrushes.Black,
                new XPoint(leftX + 90, y));

            gfx.DrawString("SỐ CHỨNG TỪ:", fontSmall, XBrushes.Black, new XPoint(rightX, y));
            gfx.DrawString(hd.SoHD, fontSmall, XBrushes.Black,
                new XPoint(rightX + 80, y));
            y += 14;

            gfx.DrawString("NGÀY/ GIỜ RA:", fontSmall, XBrushes.Black, new XPoint(leftX, y));
            gfx.DrawString(ngayRa, fontSmall, XBrushes.Black,
                new XPoint(leftX + 90, y));

            gfx.DrawString("THU NGÂN:", fontSmall, XBrushes.Black, new XPoint(rightX, y));
            // TODO: thay bằng tên thu ngân thực tế
            gfx.DrawString("TƯỜNG VY", fontSmall, XBrushes.Black,
                new XPoint(rightX + 80, y));
            y += 20;

            // Tên khách hàng + điểm
            int diemTichLuy = 0; // TODO: lấy từ dữ liệu nếu có
            gfx.DrawString("TÊN KHÁCH HÀNG: " + hd.TenKH, fontNormal, XBrushes.Black, new XPoint(leftX, y));
            y += 14;
            gfx.DrawString("ĐIỂM TÍCH LŨY: " + diemTichLuy, fontSmall, XBrushes.Black, new XPoint(leftX, y));
            y += 18;

            // ======================= CHI TIẾT DỊCH VỤ ========================
            gfx.DrawString("CHI TIẾT DỊCH VỤ", fontBold10, XBrushes.Black, new XPoint(leftX, y));
            y += 8;

            // ---- Bảng ĐẶT SÂN ----
            double tableX = marginLeft;
            double tableWidth = pageWidth - marginLeft - marginRight;
            double rowHeight = 16;

            // Cột: ĐẶT SÂN | BẮT ĐẦU | KẾT THÚC | ĐƠN GIÁ | SỐ GIỜ | THÀNH TIỀN
            double[] colWidths = { 70, 60, 60, 60, 50, 70 };
            double[] colX = new double[colWidths.Length];
            colX[0] = tableX;
            for (int i = 1; i < colWidths.Length; i++)
                colX[i] = colX[i - 1] + colWidths[i - 1];

            // Header row
            gfx.DrawString("ĐẶT SÂN", fontSmall, XBrushes.Black, new XPoint(colX[0], y));
            gfx.DrawString("BẮT ĐẦU", fontSmall, XBrushes.Black, new XPoint(colX[1], y));
            gfx.DrawString("KẾT THÚC", fontSmall, XBrushes.Black, new XPoint(colX[2], y));
            gfx.DrawString("ĐƠN GIÁ", fontSmall, XBrushes.Black, new XPoint(colX[3], y));
            gfx.DrawString("SỐ GIỜ", fontSmall, XBrushes.Black, new XPoint(colX[4], y));
            gfx.DrawString("THÀNH TIỀN", fontSmall, XBrushes.Black, new XPoint(colX[5], y));
            y += rowHeight;

            // ====== DÒNG DỮ LIỆU ĐẶT SÂN ======
            foreach (var item in chiTiets)
            {
                if (!HasProperty(item, "Loai") || item.Loai != "San") continue;

                string tenSan = item.TenSan;
                DateTime batDau = item.BatDau;
                DateTime ketThuc = item.KetThuc;
                decimal donGia = item.DonGia;
                double soGio = item.SoGio;
                decimal thanhTien = item.ThanhTien;

                gfx.DrawString(tenSan, fontSmall, XBrushes.Black, new XPoint(colX[0], y));
                gfx.DrawString(batDau.ToString("HH:mm"), fontSmall, XBrushes.Black, new XPoint(colX[1], y));
                gfx.DrawString(ketThuc.ToString("HH:mm"), fontSmall, XBrushes.Black, new XPoint(colX[2], y));
                gfx.DrawString(donGia.ToString("N0"), fontSmall, XBrushes.Black, new XPoint(colX[3], y));
                gfx.DrawString(soGio.ToString("0.##"), fontSmall, XBrushes.Black, new XPoint(colX[4], y));
                gfx.DrawString(thanhTien.ToString("N0"), fontSmall, XBrushes.Black, new XPoint(colX[5], y));

                y += rowHeight;
            }


            // ======================= THUÊ VỢT ===============================
            if (soLuongVot > 0)
            {

                gfx.DrawString("THUÊ VỢT", fontBold10, XBrushes.Black, new XPoint(leftX, y));
                y += 14;

                gfx.DrawString("SỐ LƯỢNG: " + soLuongVot, fontSmall, XBrushes.Black, new XPoint(leftX, y));
                gfx.DrawString("ĐƠN GIÁ: " + donGiaVot.ToString("N0"), fontSmall, XBrushes.Black,
                    new XPoint(leftX + 140, y));
                decimal tienVot = soLuongVot * donGiaVot;
                gfx.DrawString("THÀNH TIỀN: " + tienVot.ToString("N0"), fontSmall, XBrushes.Black,
                    new XPoint(leftX + 260, y));
                y += 18;
            }


            // ======================= TỔNG TIỀN & THANH TOÁN =================
            decimal tongTien = hd.TongTien;
            decimal tongThanhToan = hd.TongTien; // nếu có giảm giá / COD thì chỉnh lại

            gfx.DrawString("TỔNG GIÁ TRỊ HÓA ĐƠN", fontBold10, XBrushes.Black, new XPoint(tableX, y));
            gfx.DrawString(tongTien.ToString("N0"), fontBold10, XBrushes.Black,
                new XPoint(pageWidth - marginRight - 80, y));
            y += 16;

            gfx.DrawString("TỔNG GIÁ TIỀN THANH TOÁN", fontBold10, XBrushes.Black, new XPoint(tableX, y));
            gfx.DrawString(tongThanhToan.ToString("N0"), fontBold10, XBrushes.Black,
                new XPoint(pageWidth - marginRight - 80, y));
            y += 20;

            // Khách trả / Thối lại (mẫu giống hình, bên phải nhỏ hơn)
            decimal khachTra = tongThanhToan;              // TODO: gán từ UI
            decimal tienThoi = khachTra - tongThanhToan;   // TODO: nếu có

            gfx.DrawString("Khách trả: " + khachTra.ToString("N0"),
                fontItalic9, XBrushes.Black,
                new XPoint(pageWidth - marginRight - 130, y));
            y += 12;
            gfx.DrawString("Thối lại: " + tienThoi.ToString("N0"),
                fontItalic9, XBrushes.Black,
                new XPoint(pageWidth - marginRight - 130, y));
            y += 24;

            // ======================= FOOTER CẢM ƠN ==========================
            gfx.DrawString("CẢM ƠN VÀ HẸN GẶP LẠI Ở LẦN CHƠI TIẾP THEO!",
                fontNormal, XBrushes.Black,
                new XRect(0, y, pageWidth, 20), XStringFormats.TopCenter);
            y += 18;

            string hdCode = $"#HD-{hd.NgayLap:ddMMyyyy}_{hd.SoHD}";
            gfx.DrawString($"{hd.NgayLap:dd/MM/yyyy HH:mm}     {hdCode}",
                fontSmall, XBrushes.Black,
                new XRect(0, y, pageWidth, 20), XStringFormats.TopCenter);

            // ======================= LƯU FILE ===============================
            SaveFileDialog dlg = new SaveFileDialog
            {
                Title = "Lưu hóa đơn PDF",
                Filter = "PDF File|*.pdf",
                FileName = $"HoaDon_{hd.SoHD}.pdf"
            };

            if (dlg.ShowDialog() == true)
            {
                document.Save(dlg.FileName);
                MessageBox.Show("Đã lưu hóa đơn thành công!");
            }
        }

        /// <summary>
        /// Helper nhỏ: kiểm tra dynamic có property không để tránh crash
        /// </summary>
        private bool HasProperty(dynamic obj, string name)
        {
            return obj.GetType().GetProperty(name) != null;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            // Mặc định chọn "Tiền mặt"
            txtSDT.Text="";
            BtnDatLaiVot();
            LoadData();
        }

    }
}
