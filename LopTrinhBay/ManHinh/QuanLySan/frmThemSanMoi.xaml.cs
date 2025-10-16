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
                San san = new San
                {
                    TenSan = txtTenSan.Text.Trim(),
                    TrangThai = ((ComboBoxItem)cboTrangThai.SelectedItem).Content.ToString(),
                    GiaNgayThuong = decimal.Parse(txtGiaNgayThuong.Text),
                    GiaCuoiTuan = decimal.Parse(txtGiaCuoiTuan.Text),
                    GiaLeTet = decimal.Parse(txtGiaLeTet.Text),
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
                    MessageBox.Show("Thêm sân thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
