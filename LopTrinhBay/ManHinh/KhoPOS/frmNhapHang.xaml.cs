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
        // Trong frmNhapHang.xaml.cs
        private void txtNhaCungCap_TextChanged(object sender, TextChangedEventArgs e)
        {
            string nhapLieu = txtNhaCungCap.Text.Trim();

            if (nhapLieu.Length > 0)
            {
                PhieuNhapBLL phieuBLL = new PhieuNhapBLL();

                // Gọi hàm BLL để lấy gợi ý (giả sử đã định nghĩa như bước trước)
                List<string> goiYList = phieuBLL.LayDanhSachNhaCungCapGoiY(nhapLieu);

                lstNhaCungCapGoiY.ItemsSource = goiYList;

                if (goiYList.Count > 0)
                {
                    popupGoiYNCC.IsOpen = true;
                }
                else
                {
                    popupGoiYNCC.IsOpen = false;
                }
            }
            else
            {
                // Đóng popup nếu không có nhập liệu
                popupGoiYNCC.IsOpen = false;
            }
        }
        // Trong frmNhapHang.xaml.cs
        private void lstNhaCungCapGoiY_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstNhaCungCapGoiY.SelectedItem != null)
            {
                // Đặt giá trị được chọn vào TextBox
                txtNhaCungCap.Text = lstNhaCungCapGoiY.SelectedItem.ToString();

                // Đóng popup
                popupGoiYNCC.IsOpen = false;

                // Đặt con trỏ về cuối TextBox
                txtNhaCungCap.SelectionStart = txtNhaCungCap.Text.Length;
            }
        }
        // Trong frmNhapHang.xaml.cs
        private void txtNhaCungCap_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Chỉ xử lý khi Popup đang mở
            if (popupGoiYNCC.IsOpen)
            {
                // 1. Xử lý phím MŨI TÊN XUỐNG (Down Arrow)
                if (e.Key == Key.Down)
                {
                    // Chuyển focus xuống ListBox và chọn mục đầu tiên
                    if (lstNhaCungCapGoiY.Items.Count > 0)
                    {
                        lstNhaCungCapGoiY.Focus();
                        lstNhaCungCapGoiY.SelectedIndex = 0;
                        e.Handled = true;
                    }
                }
                // 2. Xử lý phím TAB (Mục tiêu của bạn)
                else if (e.Key == Key.Tab)
                {
                    // Kiểm tra xem ListBox có mục nào được chọn hoặc có dữ liệu không
                    if (lstNhaCungCapGoiY.Items.Count > 0)
                    {
                        // Nếu chưa có mục nào được highlight, chọn mục đầu tiên
                        if (lstNhaCungCapGoiY.SelectedItem == null)
                        {
                            lstNhaCungCapGoiY.SelectedIndex = 0;
                        }

                        // Cập nhật TextBox với mục đã chọn
                        txtNhaCungCap.Text = lstNhaCungCapGoiY.SelectedItem.ToString();

                        // Đóng popup
                        popupGoiYNCC.IsOpen = false;

                        // KHÔNG đặt e.Handled = true.
                        // Điều này cho phép sự kiện Tab tiếp tục lan truyền, 
                        // giúp focus tự động chuyển sang control tiếp theo sau khi chọn.
                    }
                }
                // 3. Xử lý phím ENTER
                else if (e.Key == Key.Enter)
                {
                    if (lstNhaCungCapGoiY.Items.Count > 0)
                    {
                        if (lstNhaCungCapGoiY.SelectedItem == null)
                        {
                            lstNhaCungCapGoiY.SelectedIndex = 0;
                        }

                        txtNhaCungCap.Text = lstNhaCungCapGoiY.SelectedItem.ToString();
                        popupGoiYNCC.IsOpen = false;
                        e.Handled = true;
                    }
                }
            }
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
            //dpHSD.Text = "";


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
                // 1. Kiểm tra dữ liệu
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

                // 2. Tạo phiếu để hiển thị preview
                PhieuNhap phieu = new PhieuNhap
                {
                    NgayNhap = dpngayNhap.SelectedDate ?? DateTime.Now,
                    GhiChu = txtGhiChu.Text,
                    NhaCungCap = txtNhaCungCap.Text,
                    TongTien = decimal.Parse(txtTongThanhToan.Text.Replace(" đ", ""))
                };

                PhieuNhapBLL phieuBLL = new PhieuNhapBLL();
                phieu.SoPhieu = phieuBLL.TaoSoPhieu();

                // 3. Lấy chi tiết từ DataGrid
                List<ChiTietPhieuNhap> chiTiets = dgChiTiet.Items.Cast<ChiTietPhieuNhap>().ToList();

                // 4. Tạo nội dung preview
                string noiDung = TaoNoiDungXacNhan(phieu, chiTiets);

                // 5. Hiển popup xác nhận
                var confirm = MessageBox.Show(noiDung, "Xác nhận lưu phiếu", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirm != MessageBoxResult.Yes)
                    return; // Hủy → không lưu

                // 6. Gán mã chi tiết và lưu DB
                int stt = 1;
                foreach (var row in chiTiets)
                {
                    row.MaChiTiet = $"{phieu.SoPhieu}-{stt:000}";
                    stt++;
                }

                phieuBLL.LuuPhieuNhap(phieu, chiTiets);

                MessageBox.Show($"Lưu phiếu {phieu.SoPhieu} thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // Reset UI
                dgChiTiet.Items.Clear();
                LoadPhieu();
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private string TaoNoiDungXacNhan(PhieuNhap phieu, List<ChiTietPhieuNhap> chiTiets)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("XÁC NHẬN LƯU PHIẾU");
            sb.AppendLine("--------------------------------------");
            sb.AppendLine($"Số phiếu: {phieu.SoPhieu}");
            sb.AppendLine($"Ngày nhập: {phieu.NgayNhap:dd/MM/yyyy}");
            sb.AppendLine($"Nhà cung cấp: {phieu.NhaCungCap}");
            sb.AppendLine($"Ghi chú: {phieu.GhiChu}");
            sb.AppendLine($"Tổng tiền: {phieu.TongTien:N0} đ");
            sb.AppendLine("--------------------------------------");
            sb.AppendLine("CHI TIẾT PHIẾU:");

            foreach (var ct in chiTiets)
            {
                sb.AppendLine($"- {ct.TenHang} | SL: {ct.SoLuong} | Giá: {ct.GiaNhap:N0} | CK: {ct.ChietKhau:N0} | Lô: {ct.SoLo} | HSD: {ct.HSD:dd/MM/yyyy}");
            }

            return sb.ToString();
        }

        // Logic chính để tạo lại/cập nhật mã hàng
        private void CapNhatMaHang()
        {
            string donViTinh = cboDVT.Text.Trim();
            string tenHang = txtTenHang.Text.Trim();

            if (string.IsNullOrEmpty(tenHang))
                return;

            HangHoaBLL hangHoaBLL = new HangHoaBLL();

            // Lấy danh sách mã đã có trong DataGrid
            List<string> maTrongGrid = dgChiTiet.Items
                .Cast<ChiTietPhieuNhap>()
                .Select(x => x.MaHang)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            // Kiểm tra DB trước
            string maHangDaCo = hangHoaBLL.LayMaHangByTenVaDVT(tenHang, donViTinh);

            if (maHangDaCo != null)
            {
                txtMaHang.Text = maHangDaCo;
            }
            else
            {
                txtMaHang.Text = hangHoaBLL.TaoMaHang(donViTinh, maTrongGrid);
            }
        }
        // Trong frmNhapHang.xaml.cs, thêm sau các hàm của Nhà cung cấp (hoặc ở cuối lớp)

        private void txtTenHang_TextChanged(object sender, TextChangedEventArgs e)
        {
            string nhapLieu = txtTenHang.Text.Trim();

            // Đảm bảo TextChanged cho Tên hàng không bị gọi lặp lại khi chọn mục gợi ý
            // Bằng cách kiểm tra xem sự thay đổi có phải do chọn từ ListBox không (dùng logic tạm)


            if (nhapLieu.Length > 0)
            {
                HangHoaBLL hangHoaBLL = new HangHoaBLL();

                // Gọi hàm BLL để lấy gợi ý Tên Hàng
                List<string> goiYList = hangHoaBLL.LayDanhSachTenHangGoiY(nhapLieu);

                lstTenHangGoiY.ItemsSource = goiYList;

                if (goiYList.Count > 0)
                {
                    popupGoiYTenHang.IsOpen = true;
                }
                else
                {
                    popupGoiYTenHang.IsOpen = false;
                }
            }
            else
            {
                // Đóng popup nếu không có nhập liệu
                popupGoiYTenHang.IsOpen = false;
                CapNhatMaHang(); // Gọi lại để xóa Mã hàng nếu Tên hàng bị xóa
            }

            // Tạm thời bỏ gọi CapNhatMaHang ở đây để tránh gọi quá nhiều lần
            //CapNhatMaHang(); 
        }
        // Trong frmNhapHang.xaml.cs
        private void lstTenHangGoiY_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstTenHangGoiY.SelectedItem != null)
            {
                // Đặt giá trị được chọn vào TextBox
                txtTenHang.Text = lstTenHangGoiY.SelectedItem.ToString();

                // CẬP NHẬT MÃ HÀNG ngay sau khi chọn Tên hàng
                CapNhatMaHang();

                // Đóng popup
                popupGoiYTenHang.IsOpen = false;

                // Đặt con trỏ về cuối TextBox
                txtTenHang.SelectionStart = txtTenHang.Text.Length;
            }
        }


        private void cboDVT_LostFocus(object sender, RoutedEventArgs e)
        {
            // Cập nhật khi người dùng rời khỏi ComboBox, đảm bảo giá trị đã ổn định
            CapNhatMaHang();
        }
        // Trong frmNhapHang.xaml.cs
        private void txtTenHang_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (popupGoiYTenHang.IsOpen)
            {
                if (e.Key == Key.Down)
                {
                    // Chuyển focus xuống ListBox và chọn mục đầu tiên
                    if (lstTenHangGoiY.Items.Count > 0)
                    {
                        lstTenHangGoiY.Focus();
                        lstTenHangGoiY.SelectedIndex = 0;
                        e.Handled = true; // Ngăn không cho TextBox xử lý phím Down
                    }
                }
                else if (e.Key == Key.Enter || e.Key == Key.Tab) // Xử lý Enter và Tab cùng lúc
                {
                    if (lstTenHangGoiY.Items.Count > 0)
                    {
                        // Chọn mục đầu tiên nếu chưa chọn
                        if (lstTenHangGoiY.SelectedItem == null)
                        {
                            lstTenHangGoiY.SelectedIndex = 0;
                        }

                        // Commit giá trị
                        txtTenHang.Text = lstTenHangGoiY.SelectedItem.ToString();

                        // CẬP NHẬT MÃ HÀNG
                        CapNhatMaHang();

                        // Đóng popup
                        popupGoiYTenHang.IsOpen = false;

                        // Nếu là Enter thì chặn (e.Handled=true) để không chuyển focus
                        // Nếu là Tab thì KHÔNG chặn để chuyển focus sang ô tiếp theo
                        if (e.Key == Key.Enter)
                        {
                            e.Handled = true;
                        }
                    }
                }
            }
        }

       
    }
}
