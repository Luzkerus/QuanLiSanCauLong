using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopNghiepVu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class frmNhapHang : Window
    {
        public frmNhapHang()
        {
            InitializeComponent();
            LoadPhieu();
            var today = DateTime.Today;
            dpHSD.DisplayDateStart = today.AddDays(30); // Ngày nhỏ nhất: 30 ngày sau hôm nay
            dpHSD.SelectedDate = today.AddDays(30);
        }
        private void LoadPhieu()
        {
            var today = DateTime.Today;
            dpngayNhap.SelectedDate = today;
            PhieuNhapBLL phieuBLL = new PhieuNhapBLL();
            txtSoPhieu.Text = phieuBLL.TaoSoPhieu();
            txtNhaCungCap.Text = "";
            txtGhiChu.Text = "";
            txtTongThanhToan.Text = "0 đ";

        }
        private void btnThemHang_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaHang.Text) ||
      string.IsNullOrWhiteSpace(txtTenHang.Text) ||
      string.IsNullOrWhiteSpace(cboDVT.Text) ||
      string.IsNullOrWhiteSpace(txtSoLuong.Text) ||
      string.IsNullOrWhiteSpace(txtGiaNhap.Text) ||
      string.IsNullOrWhiteSpace(txtChietKhau.Text) ||
      string.IsNullOrWhiteSpace(txtSoLo.Text) ||
      string.IsNullOrWhiteSpace(dpHSD.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Dừng thêm nếu có trường rỗng
            }

            // Chuyển đổi số và ngày, kèm try-catch để tránh lỗi
            if (!int.TryParse(txtSoLuong.Text, out int soLuong))
            {
                MessageBox.Show("Số lượng phải là số nguyên.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtGiaNhap.Text, out decimal giaNhap))
            {
                MessageBox.Show("Giá nhập phải là số.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtChietKhau.Text, out decimal chietKhau))
            {
                MessageBox.Show("Chiết khấu phải là số.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DateTime.TryParse(dpHSD.Text, out DateTime hsd))
            {
                MessageBox.Show("Ngày HSD không hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Code to add item to the purchase order
            ChiTietPhieuNhap chiTietPhieuNhap = new ChiTietPhieuNhap
            {
                MaHang = txtMaHang.Text,
                TenHang = txtTenHang.Text,
                DVT = cboDVT.Text,
                SoLuong = int.Parse(txtSoLuong.Text),
                GiaNhap = decimal.Parse(txtGiaNhap.Text),
                ChietKhau = decimal.Parse(txtChietKhau.Text),
                //VAT = decimal.Parse(txtVAT.Text),
                SoLo = txtSoLo.Text,
                HSD = DateTime.Parse(dpHSD.Text)

            };
            dgChiTiet.Items.Add(chiTietPhieuNhap);
            CapNhatTongThanhToan();
            txtMaHang.Text = "";
            txtTenHang.Text = "";
            txtChietKhau.Text ="";
            txtGiaNhap.Text = "";
            txtSoLo.Text = "";
            txtSoLuong.Text = "";
            dpHSD.Text = "";


        }
        private void CapNhatTongThanhToan()
        {
            decimal tongTien = 0;
            foreach (ChiTietPhieuNhap item in dgChiTiet.Items)
            {
                tongTien += item.ThanhTien;
            }
            txtTongThanhToan.Text = tongTien.ToString("N0") + " đ";
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Cho phép số (0-9), dấu chấm/phẩy thập phân (.), và dấu trừ (-)
            // Chỉ cần kiểm tra nếu ký tự nhập vào KHÔNG PHẢI là số
            if (!Regex.IsMatch(e.Text, @"[0-9\.-]"))
            {
                e.Handled = true;
            }

            TextBox tb = sender as TextBox;

            // Chặn nhiều dấu chấm
            if (e.Text == "." && tb.Text.Contains("."))
            {
                e.Handled = true;
            }
        }
        private void NumberTextBox(object sender, TextCompositionEventArgs e)
        {
            // Biểu thức Regex mới: chỉ tìm kiếm các ký tự KHÔNG PHẢI là số từ 0 đến 9.
            // Nếu ký tự nhập vào không nằm trong dải [0-9], thì sẽ bị chặn.
            Regex regex = new Regex("[^0-9]+");

            // Nếu ký tự nhập vào không phải là số (0-9), e.Handled sẽ là true, 
            // và ký tự đó sẽ không được đưa vào TextBox.
            e.Handled = regex.IsMatch(e.Text);
        }


        private void XoaDong_Click(object sender, RoutedEventArgs e)
        {
            // Lấy Button
            var btn = sender as Button;
            if (btn == null) return;

            // Lấy dòng hàng cần xóa từ Tag
            var dong = btn.Tag;

            if (dong != null)
            {
                dgChiTiet.Items.Remove(dong);
                CapNhatTongThanhToan();
            }
        }
        private void btnLuuPhieu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Tạo phiếu
                if (dpngayNhap.SelectedDate == null)
                {
                    MessageBox.Show("Vui lòng chọn ngày nhập!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtNhaCungCap.Text))
                {
                    MessageBox.Show("Vui lòng nhập nhà cung cấp!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (dgChiTiet.Items.Count == 0)
                {
                    MessageBox.Show("Phiếu nhập không có chi tiết nào!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                PhieuNhap phieu = new PhieuNhap
                {
                    NgayNhap = dpngayNhap.SelectedDate ?? DateTime.Now,
                    GhiChu = txtGhiChu.Text,
                    NhaCungCap = txtNhaCungCap.Text,
                    TongTien = decimal.Parse(txtTongThanhToan.Text.Replace(" đ",""))
                };

                // Tạo số phiếu: PN + yyyyMMdd + 0001
                PhieuNhapBLL phieuBLL = new PhieuNhapBLL();
                phieu.SoPhieu = phieuBLL.TaoSoPhieu();

                // 2. Lấy danh sách chi tiết từ DataGrid
                List<ChiTietPhieuNhap> chiTiets = new List<ChiTietPhieuNhap>();
                int stt = 1;
                foreach (ChiTietPhieuNhap row in dgChiTiet.Items)
                {
                    // Tạo Mã chi tiết theo số phiếu + thứ tự
                    row.MaChiTiet = $"{phieu.SoPhieu}-{stt:000}";
                    stt++;


                    chiTiets.Add(row);
                }

                // 3. Lưu phiếu và chi tiết
                phieuBLL.LuuPhieuNhap(phieu, chiTiets);

                MessageBox.Show($"Lưu phiếu {phieu.SoPhieu} thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // Xóa DataGrid và reset UI nếu muốn
                dgChiTiet.Items.Clear();
                LoadPhieu();
                DialogResult = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Logic chính để tạo lại/cập nhật mã hàng
        private void CapNhatMaHang()
        {
            // Lấy Đơn Vị Tính.
            // Dùng .Text để lấy giá trị chính xác, dù là chọn từ danh sách hay tự gõ
            string donViTinh = cboDVT.Text.Trim();
            string tenHang = txtTenHang.Text.Trim(); // Giả định bạn vẫn có txtTenHang

            if (string.IsNullOrEmpty(tenHang))
            {
                // Không xử lý nếu tên hàng chưa có
                return;
            }

            HangHoaBLL hangHoaBLL = new HangHoaBLL();

            // Logic kiểm tra và tạo/lấy mã hàng
            string maHangDaCo = hangHoaBLL.LayMaHangByTenVaDVT(tenHang, donViTinh);

            if (maHangDaCo != null)
            {
                txtMaHang.Text = maHangDaCo;
            }
            else
            {
                // Tạo mã hàng mới (truyền DVT vào để tạo mã)
                txtMaHang.Text = hangHoaBLL.TaoMaHang(donViTinh);
            }
        }


        private void cboDVT_LostFocus(object sender, RoutedEventArgs e)
        {
            // Cập nhật khi người dùng rời khỏi ComboBox, đảm bảo giá trị đã ổn định
            CapNhatMaHang();
        }

        private void txtTenHang_TextChanged(object sender, RoutedEventArgs e)
        {
            // Cập nhật khi người dùng RỜI KHỎI ô Tên Hàng
            CapNhatMaHang();
        }
    }
}
