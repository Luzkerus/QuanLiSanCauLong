using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopNghiepVu;
using System;
using System.Collections.Generic;
using System.Data;
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

        private BangGiaBLL bangGiaBLL = new BangGiaBLL();

        public List<San> DanhSachSan { get; set; }
        public DataTable BangGiaChung { get; set; }

        public int TongSoSan => DanhSachSan?.Count ?? 0;
        public int TongSoSanKhongBaoTri => DanhSachSan?.Count(s => s.TrangThai != "Bảo trì") ?? 0;
        public int TongSoSanBaoTri => DanhSachSan?.Count(s => s.TrangThai == "Bảo trì") ?? 0;


        public ucQuanLySan()
        {
            InitializeComponent();
            LoadData();
      
        }
        private void LoadData()
        {
            try
            {
                DanhSachSan = sanBLL.LayTatCaSan();
                BangGiaChung = bangGiaBLL.LayBangGiaChung();
                DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                DanhSachSan = new List<San>();
                BangGiaChung = new DataTable();
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

        private void btnCauHinhGiaSan(object sender, RoutedEventArgs e)
        {
            frmCauHinhGia frmCauHinhGia = new frmCauHinhGia();
            frmCauHinhGia.ShowDialog();
        }

        private void btnThemKhungGio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool ok = bangGiaBLL.ThemBangGiaMau();

                if (ok)
                {
                    MessageBox.Show("✅ Đã thêm khung giờ mẫu!", "Thông báo",
                                    MessageBoxButton.OK, MessageBoxImage.Information);

                    // Load lại dữ liệu nếu có DataGrid hiển thị bảng giá
                    BangGiaChung = bangGiaBLL.LayBangGiaChung();
                    DataContext = null;
                    DataContext = this;
                }
                else
                {
                    MessageBox.Show("❌ Thêm khung giờ mẫu thất bại!", "Lỗi",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm khung giờ mẫu: " + ex.Message,
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnXoaKhungGio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgBangGia.SelectedItem is DataRowView selectedRow)
                {
                    // Lấy mã khung giờ từ dòng được chọn
                    int maBangGia = Convert.ToInt32(selectedRow["MaBangGia"]);
                    string gioBatDau = selectedRow["GioBatDau"].ToString();
                    string gioKetThuc = selectedRow["GioKetThuc"].ToString();

                    MessageBoxResult confirm = MessageBox.Show(
                        $"Bạn có chắc muốn xóa khung giờ {gioBatDau} - {gioKetThuc} không?",
                        "Xác nhận xóa",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                    );

                    if (confirm == MessageBoxResult.Yes)
                    {
                        bool xoaThanhCong = bangGiaBLL.XoaBangGia(maBangGia);

                        if (xoaThanhCong)
                        {
                            MessageBox.Show("✅ Đã xóa khung giờ khỏi cơ sở dữ liệu!", "Thông báo",
                                            MessageBoxButton.OK, MessageBoxImage.Information);

                            // Load lại bảng giá
                            BangGiaChung = bangGiaBLL.LayBangGiaChung();
                            DataContext = null;
                            DataContext = this;
                        }
                        else
                        {
                            MessageBox.Show("❌ Không thể xóa khung giờ!", "Lỗi",
                                            MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn dòng cần xóa!", "Thông báo",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa khung giờ: " + ex.Message,
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
