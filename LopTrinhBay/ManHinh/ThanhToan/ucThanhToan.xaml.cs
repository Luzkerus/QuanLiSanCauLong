using Microsoft.CSharp.RuntimeBinder;
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
using Microsoft.Win32;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
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

        private void TaoHoaDonPDF(QuanLiSanCauLong.LopDuLieu.ThanhToan hd, List<dynamic> chiTiets, int soLuongVot, decimal donGiaVot)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Phiếu Thanh Toán";
            PdfPage page = document.AddPage();
            page.Size = PdfSharpCore.PageSize.A6;

            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont fontTitle = new XFont("Arial", 16, XFontStyle.Bold);
            XFont fontNormal = new XFont("Arial", 10);
            XFont fontSmall = new XFont("Arial", 8);

            double pageWidth = page.Width;

            // ======= CĂN GIỮA TIÊU ĐỀ =======
            gfx.DrawString("PHIẾU THANH TOÁN", fontTitle, XBrushes.Black,
                new XRect(0, 20, pageWidth, 20), XStringFormats.TopCenter);

            gfx.DrawString("SÂN CẦU LÔNG CỦ CHI", fontNormal, XBrushes.Black,
                new XRect(0, 50, pageWidth, 20), XStringFormats.TopCenter);

            gfx.DrawString("72, Tỉnh lộ 15, Ấp Phú Thuận, Xã Phú Hòa Đông, TP. Hồ Chí Minh", fontSmall, XBrushes.Black,
                new XRect(0, 70, pageWidth, 20), XStringFormats.TopCenter);

            gfx.DrawString("Tel: 0966752642", fontSmall, XBrushes.Black,
                new XRect(0, 85, pageWidth, 20), XStringFormats.TopCenter);

            double y = 110;

            // ===== Ngày giờ =====
            string ngayGio = hd.NgayLap.ToString("dd.MM.yyyy HH:mm");

            gfx.DrawString($"NGÀY/ GIỜ : {ngayGio}", fontSmall, XBrushes.Black, new XPoint(10, y));
            y += 15;


            gfx.DrawString($"SỐ CHỨNG TỪ: {hd.SoHD}", fontSmall, XBrushes.Black, new XPoint(10, y));
            y += 20;

            gfx.DrawString($"TÊN KHÁCH HÀNG: {hd.TenKH}", fontNormal, XBrushes.Black, new XPoint(10, y));
            y += 15;

            // ======= ĐẶT SÂN =======
            gfx.DrawString("ĐẶT SÂN", fontNormal, XBrushes.Black, new XPoint(10, y));
            y += 20;




            gfx.DrawString($"TỔNG TIỀN SÂN: {hd.TongTienSan} VNĐ", fontNormal, XBrushes.Black, new XPoint(10,y));


            y += 15;

            // ======= THUÊ VỢT =======
            if (soLuongVot > 0)
            {
                gfx.DrawString("THUÊ VỢT", fontNormal, XBrushes.Black, new XPoint(10, y));
                y += 15;

                gfx.DrawString($"SỐ LƯỢNG: {soLuongVot}", fontNormal, XBrushes.Black, new XPoint(10, y));
                y += 12;

                gfx.DrawString($"THÀNH TIỀN: {(soLuongVot * donGiaVot):N0}", fontNormal, XBrushes.Black, new XPoint(10, y));
                y += 12;

                gfx.DrawString($"ĐƠN GIÁ: {donGiaVot:N0}", fontNormal, XBrushes.Black, new XPoint(10, y));
                y += 20;
            }

            y += 15;

            // ======= TỔNG TIỀN (CĂN GIỮA) =======
            gfx.DrawString("TỔNG GIÁ TRỊ HÓA ĐƠN", fontNormal, XBrushes.Black,
                new XRect(0, y, pageWidth, 20), XStringFormats.TopCenter);

            y += 18;

            gfx.DrawString($"{hd.TongTien:N0} VNĐ", fontTitle, XBrushes.Black,
                new XRect(0, y, pageWidth, 20), XStringFormats.TopCenter);

            y += 30;

            gfx.DrawString("CẢM ƠN VÀ HẸN GẶP LẠI Ở LẦN CHƠI TIẾP THEO!", fontNormal, XBrushes.Black,
                new XRect(0, y, pageWidth, 20), XStringFormats.TopCenter);

            y += 20;

            // ======= MÃ HÓA ĐƠN + NGÀY =======
            string hdCode = $"#HD-{hd.NgayLap:ddMMyyyy}_{hd.SoHD}";
            gfx.DrawString($"{hd.NgayLap:dd/MM/yyyy HH:mm}   {hdCode}", fontSmall, XBrushes.Black,
                new XRect(0, y, pageWidth, 20), XStringFormats.TopCenter);

            // Lưu file bằng SaveFileDialog
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Lưu hóa đơn PDF";
            dlg.Filter = "PDF File|*.pdf";
            dlg.FileName = $"HoaDon_{hd.SoHD}.pdf";

            if (dlg.ShowDialog() == true)
            {
                document.Save(dlg.FileName);
                MessageBox.Show("Đã lưu hóa đơn thành công!");
            }
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
