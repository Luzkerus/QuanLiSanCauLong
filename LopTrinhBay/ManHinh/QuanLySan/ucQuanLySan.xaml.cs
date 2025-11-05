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
    /// Interaction logic for ucQuanLySan.xaml
    /// </summary>
    public partial class ucQuanLySan : UserControl
    {
        private SanBLL sanBLL = new SanBLL();
        public List<San> DanhSachSan { get; set; }

        public int TongSoSan => DanhSachSan?.Count ?? 0;
        public int TongSoSanKhongBaoTri => DanhSachSan?.Count(s => s.TrangThai != "Bảo trì") ?? 0;
        public int TongSoSanBaoTri => DanhSachSan?.Count(s => s.TrangThai == "Bảo trì") ?? 0;

        public ucQuanLySan()
        {
            InitializeComponent();

            try
            {
                DanhSachSan = sanBLL.LayTatCaSan();
                DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                DanhSachSan = new List<San>();
            }
        }

        private void btnThemSanMoi(object sender, RoutedEventArgs e)
        {
            frmThemSanMoi themSanMoiWindow = new frmThemSanMoi();
        
            bool? result = themSanMoiWindow.ShowDialog(); // chờ form con đóng lại

            if (result == true)
            {
                TaiLaiDanhSachSan(); // load lại danh sách khi thêm thành công
            }
        }

        // Nút Cấu hình giá sân (Chưa làm)

        private void btn_Sua(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            San sanDuocChon = btn?.DataContext as San;  // Lấy sân gắn với nút sửa

            if (sanDuocChon == null)
            {
                MessageBox.Show("Không xác định được sân cần sửa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            frmChinhSuaSan formSua = new frmChinhSuaSan(sanDuocChon);
     
            bool? result = formSua.ShowDialog();

            if (result == true)
            {
                TaiLaiDanhSachSan(); // Reload danh sách sau khi sửa thành công
            }
        }

        private void TaiLaiDanhSachSan()
        {
            try
            {
                DanhSachSan = sanBLL.LayTatCaSan();
                DataContext = null;   // reset binding
                DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải lại danh sách: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btn_Xoa(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lấy đối tượng San từ Button (nút trong ItemTemplate)
                Button btn = sender as Button;
                San san = btn?.DataContext as San;

                if (san == null)
                {
                    MessageBox.Show("Không xác định được sân cần xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Hỏi xác nhận
                MessageBoxResult result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa sân \"{san.TenSan}\" không?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    bool xoaThanhCong = sanBLL.XoaSan(san.MaSan);

                    if (xoaThanhCong)
                    {
                        MessageBox.Show($"✅ Đã xóa sân \"{san.TenSan}\" thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        TaiLaiDanhSachSan(); // Cập nhật lại danh sách hiển thị
                    }
                    else
                    {
                        MessageBox.Show($"❌ Không thể xóa sân \"{san.TenSan}\"!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa sân: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
