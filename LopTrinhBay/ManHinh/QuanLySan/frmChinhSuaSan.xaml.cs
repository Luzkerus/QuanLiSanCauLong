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

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.QuanLySan
{
    /// <summary>
    /// Interaction logic for frmChinhSuaSan.xaml
    /// </summary>
    public partial class frmChinhSuaSan : Window
    {
        private readonly SanBLL sanBLL = new SanBLL();
        private San sanHienTai;
        public frmChinhSuaSan(San sanCanSua)
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                var fadeIn = new System.Windows.Media.Animation.DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250));
                this.BeginAnimation(Window.OpacityProperty, fadeIn);
            };
            sanHienTai = sanCanSua;

            // Hiển thị thông tin sân hiện tại
            txtTenSan.Text = sanCanSua.TenSan;
            cboTrangThai.Text = sanCanSua.TrangThai;
            txtGiaNgayThuong.Text = sanCanSua.GiaNgayThuong.ToString();
            txtGiaCuoiTuan.Text = sanCanSua.GiaCuoiTuan.ToString();
            txtGiaLeTet.Text = sanCanSua.GiaLeTet.ToString();
            dpNgayBaoTri.SelectedDate = sanCanSua.NgayBaoTri;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void BtnDongForm(object sender, MouseButtonEventArgs e)
        {
            this.Close(); // Đóng form hiện tại
        }
        private void btnCapNhat(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTenSan.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên sân.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtTenSan.Focus();
                    return;
                }

                if (cboTrangThai.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn trạng thái của sân.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    cboTrangThai.Focus();
                    return;
                }

                if (!decimal.TryParse(txtGiaNgayThuong.Text, out decimal giaNgayThuong) || giaNgayThuong <= 0)
                {
                    MessageBox.Show("Giá ngày thường không hợp lệ. Vui lòng nhập số hợp lệ.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtGiaNgayThuong.Focus();
                    return;
                }

                if (!decimal.TryParse(txtGiaCuoiTuan.Text, out decimal giaCuoiTuan) || giaCuoiTuan <= 0)
                {
                    MessageBox.Show("Giá cuối tuần không hợp lệ. Vui lòng nhập số hợp lệ.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtGiaCuoiTuan.Focus();
                    return;
                }

                if (!decimal.TryParse(txtGiaLeTet.Text, out decimal giaLeTet) || giaLeTet <= 0)
                {
                    MessageBox.Show("Giá lễ tết không hợp lệ. Vui lòng nhập số hợp lệ.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtGiaLeTet.Focus();
                    return;
                }

                if (dpNgayBaoTri.SelectedDate == null)
                {
                    MessageBox.Show("Vui lòng chọn ngày bảo trì.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpNgayBaoTri.Focus();
                    return;
                }

                // Cập nhật thông tin sân từ các trường nhập liệu
                sanHienTai.TenSan = txtTenSan.Text;
                sanHienTai.TrangThai = cboTrangThai.Text;
                sanHienTai.GiaNgayThuong = decimal.Parse(txtGiaNgayThuong.Text);
                sanHienTai.GiaCuoiTuan = decimal.Parse(txtGiaCuoiTuan.Text);
                sanHienTai.GiaLeTet = decimal.Parse(txtGiaLeTet.Text);
                sanHienTai.NgayBaoTri = dpNgayBaoTri.SelectedDate;
                // Gọi phương thức cập nhật sân trong BLL
                bool ketQua = sanBLL.CapNhatSan(sanHienTai);
                if (ketQua)
                {
                    MessageBox.Show("Cập nhật sân thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true; // Đánh dấu là cập nhật thành công
                    this.Close(); // Đóng form sau khi lưu thành công
                }
                else
                {
                    MessageBox.Show("Cập nhật sân thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật sân: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
