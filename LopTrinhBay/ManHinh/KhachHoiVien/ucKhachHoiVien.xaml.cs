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
using QuanLiSanCauLong.LopNghiepVu;
using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopTrinhBay.KhachHoiVien;
namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.KhachHoiVien
{
    /// <summary>
    /// Interaction logic for ucKhachHoiVien.xaml
    /// </summary>
    public partial class ucKhachHoiVien : UserControl
    {
        private KhachHangBLL khachHangBLL = new KhachHangBLL();
        public ucKhachHoiVien()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            List<KhachHang> dsKhachHang = khachHangBLL.LayTatCaKhachHang();
            dgKhachHang.ItemsSource = dsKhachHang;
            txtTongHoiVien.Text = khachHangBLL.LayTongHoiVien().ToString();
            txtTongKhachHang.Text = khachHangBLL.LayTongKhachHang().ToString();
            txtTongDiem.Text = khachHangBLL.LayTongDiemTichLuy().ToString();
            txtTongKhachHangMoi.Text = khachHangBLL.LayTongKhachHangMoiTrongThang().ToString();
        }

        private void chkOnlyMembers_Checked(object sender, RoutedEventArgs e) => FilterDataGrid();
        private void chkOnlyMembers_Unchecked(object sender, RoutedEventArgs e) => FilterDataGrid();
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterDataGrid();
        }

        private void FilterDataGrid()
        {
            List<KhachHang> dsKhachHang = khachHangBLL.LayTatCaKhachHang();
            string search = txtSearch.Text?.Trim().ToLower() ?? "";
            bool onlyMembers = chkOnlyMembers.IsChecked == true;

            var filtered = dsKhachHang.Where(kh =>
                // Lọc theo checkbox
                (!onlyMembers || kh.LoaiKhach == "Hội viên") &&
                // Lọc theo text
                (string.IsNullOrEmpty(search) ||
                 (kh.Ten != null && kh.Ten.ToLower().Contains(search)) ||
                 (kh.SDT != null && kh.SDT.Contains(search)) ||
                 (kh.SDTPhu != null && kh.SDTPhu.Contains(search)) ||  // <-- thêm dấu || này
                 (kh.Email != null && kh.Email.ToLower().Contains(search))
                )
            ).ToList();

            dgKhachHang.ItemsSource = filtered;
        }
        private void btnSuaKhachHang(object sender, RoutedEventArgs e)
        {
            // Lấy khách hàng hiện tại từ DataGrid (CommandParameter hoặc SelectedItem)
            if (dgKhachHang.SelectedItem is KhachHang kh)
            {
                frmThemHoiVien suaKhachHang = new frmThemHoiVien(kh);
                suaKhachHang.Owner = Window.GetWindow(this); // Set Owner để form nổi trên parent
                bool? result = suaKhachHang.ShowDialog();

                if (result == true)
                {
                    // Refresh DataGrid sau khi sửa
                    LoadData();
                }
            }
        }


    }
}
