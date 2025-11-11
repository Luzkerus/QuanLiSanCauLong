using Microsoft.Win32;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.NhanVien
{
    public partial class frmThemNhanVien : Window
    {
        public NhanVienCreateDto Result { get; private set; }
        public string AvatarPath { get; set; }

        public frmThemNhanVien()
        {
            InitializeComponent();
            dpNgayVaoLam.SelectedDate = DateTime.Today;
        }

        private void BtnChonAnh_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Ảnh (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Title = "Chọn ảnh đại diện"
            };
            if (ofd.ShowDialog() == true)
            {
                AvatarPath = ofd.FileName;
                try
                {
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.UriSource = new Uri(AvatarPath);
                    bmp.EndInit();
                    imgAvatar.Source = bmp;
                    watermarkAvatar.Visibility = Visibility.Collapsed;
                }
                catch
                {
                    MessageBox.Show("Không đọc được ảnh đã chọn.", "Ảnh đại diện", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnHuy_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            // Validate
            var hoten = tbHoTen.Text?.Trim();
            var sdt = tbSDT.Text?.Trim();
            var email = tbEmail.Text?.Trim();
            var luongText = tbLuong.Text?.Trim();

            if (string.IsNullOrEmpty(hoten))
            {
                MessageBox.Show("Vui lòng nhập Họ và tên.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Information);
                tbHoTen.Focus(); return;
            }
            if (string.IsNullOrEmpty(sdt) || !Regex.IsMatch(sdt, @"^\+?\d{9,15}$"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ.", "Thiếu/ sai thông tin", MessageBoxButton.OK, MessageBoxImage.Information);
                tbSDT.Focus(); return;
            }
            if (!string.IsNullOrEmpty(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Email không hợp lệ.", "Sai định dạng", MessageBoxButton.OK, MessageBoxImage.Information);
                tbEmail.Focus(); return;
            }

            decimal luong = 0;
            if (!string.IsNullOrEmpty(luongText))
            {
                if (!decimal.TryParse(luongText.Replace(",", ""), NumberStyles.Number, CultureInfo.InvariantCulture, out luong))
                {
                    MessageBox.Show("Lương cơ bản không hợp lệ.", "Sai định dạng", MessageBoxButton.OK, MessageBoxImage.Information);
                    tbLuong.Focus(); return;
                }
            }

            var loai = (cbLoaiNV.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString();
            var chucdanh = (cbChucDanh.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString();
            var trangthai = (cbTrangThai.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString();
            var ngayVao = dpNgayVaoLam.SelectedDate ?? DateTime.Today;

            Result = new NhanVienCreateDto
            {
                HoTen = hoten,
                SoDienThoai = sdt,
                Email = email,
                LoaiNhanVien = loai,        // "Nv Full-time"/"Nv Part-time"
                ChucDanh = chucdanh,        // "Quản lý"...
                NgayVaoLam = ngayVao,
                TrangThai = trangthai,      // "Đang làm việc"...
                LuongCoBan = luong,
                GhiChu = tbGhiChu.Text?.Trim(),
                AvatarPath = AvatarPath
            };

            DialogResult = true;
            Close();
        }

        // thêm 3 hàm này:
        private void Header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                DragMove(); // kéo thả cửa sổ
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }

    public class NhanVienCreateDto
    {
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string LoaiNhanVien { get; set; } // Full-time / Part-time
        public string ChucDanh { get; set; }     // Quản lý / Thu ngân / ...
        public DateTime NgayVaoLam { get; set; }
        public string TrangThai { get; set; }    // Đang làm / Nghỉ / Tạm khóa
        public decimal LuongCoBan { get; set; }
        public string GhiChu { get; set; }
        public string AvatarPath { get; set; }
    }
}
