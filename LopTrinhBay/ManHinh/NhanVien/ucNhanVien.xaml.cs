using QuanLiSanCauLong.LopNghiepVu;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NhanVienModel = QuanLiSanCauLong.LopDuLieu.NhanVien;
using QuanLiSanCauLong.LopNghiepVu;
using System.Collections.Generic;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.NhanVien
{
    /// <summary>
    /// Interaction logic for ucNhanVien.xaml
    /// </summary>
    public partial class ucNhanVien : UserControl
    {
        // ====== INotifyPropertyChanged ======


        public ucNhanVien()
        {
            InitializeComponent();
            LoadNhanVien();


            // Khởi tạo dữ liệu demo

        }

        // Nút "Thêm nhân viên" trên header
        private void BtnThemNhanVien_Click(object sender, RoutedEventArgs e)
        {
            var win = new frmThemNhanVien();

            Window parent = Window.GetWindow(this);
            if (parent != null)
            {
                win.Owner = parent;
            }

            bool? result = win.ShowDialog();

            if (result == true)
            {
                // Sau khi thêm xong thì reload lại danh sách nhân viên (sau này nối với DB)
                LoadNhanVien();

            }
        }
        private void LoadNhanVien() { 
            NhanVienBLL nhanVienBLL = new NhanVienBLL();
            dgNhanVien.ItemsSource = new ObservableCollection<QuanLiSanCauLong.LopDuLieu.NhanVien>(nhanVienBLL.LayDanhSachNhanVien());
            txtTongNV.Text = nhanVienBLL.TongSoNhanVien().ToString();
            txtTongNVFT.Text = nhanVienBLL.TongSoNVFullTime().ToString();
            txtTongNVPT.Text = nhanVienBLL.TongSoNVPartTime().ToString();
        }
        private void BtnSuaNhanVien_Click(object sender, RoutedEventArgs e)
        {
            if (dgNhanVien.SelectedItem is NhanVienModel selectedNhanVien)
            {
                if (selectedNhanVien.VaiTro == "Admin")
                {
                    MessageBox.Show("Admin không thể sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                var win = new frmSuaNhanVien(selectedNhanVien);
                Window parent = Window.GetWindow(this);
                if (parent != null)
                {
                    win.Owner = parent;
                }
                bool? result = win.ShowDialog();
                if (result == true)
                {
                    // Sau khi sửa xong thì reload lại danh sách nhân viên (sau này nối với DB)
                    LoadNhanVien();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một nhân viên để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void BtnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmResult = MessageBox.Show("Bạn có chắc chắn muốn reset mật khẩu của nhân viên này về '123456' không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmResult != MessageBoxResult.Yes)
            {
                return; // Người dùng chọn "No", không thực hiện reset mật khẩu
            }
            if (dgNhanVien.SelectedItem is NhanVienModel selectedNhanVien)
            {
                if (selectedNhanVien.VaiTro == "Admin")
                {
                    MessageBox.Show("Admin không thể reset password.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                NhanVienBLL nhanVienBLL = new NhanVienBLL();
                bool? result = nhanVienBLL.DoiMatKhau(selectedNhanVien.MaNV,"123456");
                if (result == true)
                {
                    // Sau khi sửa xong thì reload lại danh sách nhân viên (sau này nối với DB)
                    MessageBox.Show("Reset mật khẩu thành công! Mật khẩu mới là: 123456", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadNhanVien();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một nhân viên để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e) 
        { 
            string timKiem = tbSearch.Text.Trim().ToLower();
            NhanVienBLL nhanVienBLL = new NhanVienBLL();
            List<NhanVienModel> nhanViens = nhanVienBLL.LayDanhSachNhanVien();
            var filtered = nhanViens.Where(nv =>
             (string.IsNullOrEmpty(timKiem) ||
                 (nv.TenNV != null && nv.TenNV.ToLower().Contains(timKiem)))

            ).ToList();
            dgNhanVien.ItemsSource = filtered;
        }
    }
}

  