using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopNghiepVu;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.QuanLySan
{
    public partial class frmQuanLySan : Window
    {
        private SanBLL sanBLL = new SanBLL();
        public List<San> DanhSachSan { get; set; }

        public int TongSoSan => DanhSachSan?.Count ?? 0;
        public int TongSoSanKhongBaoTri => DanhSachSan?.Count(s => s.TrangThai != "Bảo trì") ?? 0;
        public int TongSoSanBaoTri => DanhSachSan?.Count(s => s.TrangThai == "Bảo trì") ?? 0;

        public frmQuanLySan()
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
            themSanMoiWindow.Owner = this;
            bool? result = themSanMoiWindow.ShowDialog(); // chờ form con đóng lại

            if (result == true)
            {
                TaiLaiDanhSachSan(); // load lại danh sách khi thêm thành công
            }
        }

        private void btn_Sua(object sender, RoutedEventArgs e)
        {
            frmChinhSuaSan ChinhSuaSanWindow = new frmChinhSuaSan();
            ChinhSuaSanWindow.Owner = this;
            ChinhSuaSanWindow.ShowDialog();
        }

        private void btn_Xoa(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Nút Xóa hoạt động!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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

    }
}
