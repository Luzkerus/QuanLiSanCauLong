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
    /// Interaction logic for frmThemSanMoi.xaml
    /// </summary>
    public partial class frmThemSanMoi : Window
    {
        private SanBLL sanBLL = new SanBLL();
        public frmThemSanMoi()
        {
            InitializeComponent();
            this.Opacity = 0; // bắt đầu mờ hoàn toàn

            // Khi form load thì chạy animation
            this.Loaded += (s, e) =>
            {
                var fadeIn = new System.Windows.Media.Animation.DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250));
                this.BeginAnimation(Window.OpacityProperty, fadeIn);
            };
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnDongForm(object sender, MouseButtonEventArgs e)
        {
            this.Close(); // Đóng form hiện tại
        }

        private void BtnThemMoi(object sender, MouseButtonEventArgs e)
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



                if (dpNgayBaoTri.SelectedDate == null)
                {
                    MessageBox.Show("Vui lòng chọn ngày bảo trì.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpNgayBaoTri.Focus();
                    return;
                }

                San san = new San
                {
                    TenSan = txtTenSan.Text.Trim(),
                    TrangThai = cboTrangThai.Text,
                    NgayBaoTri = dpNgayBaoTri.SelectedDate
                };

                bool ketQua = sanBLL.ThemSanMoi(san);

                if (ketQua)
                {
                    MessageBox.Show("Thêm sân mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;  // 👉 báo lại cho form cha là thêm thành công
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Không thể thêm sân. Vui lòng kiểm tra lại dữ liệu.", "Thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Đã xảy ra lỗi trong quá trình thêm sân.\n\nChi tiết lỗi:\n{ex.Message}",
                    "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
