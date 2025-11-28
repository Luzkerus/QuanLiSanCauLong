using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopNghiepVu;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NhanVienModel = QuanLiSanCauLong.LopDuLieu.NhanVien;


namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.NhanVien
{
    public partial class frmThemNhanVien : Window
    {
        // ===== INotifyPropertyChanged =====
        

        // ===== Constructor =====
        public frmThemNhanVien()
        {
            InitializeComponent();
            cmbVaiTro.SelectionChanged += CmbVaiTro_SelectionChanged;

            // dùng luôn chính window làm DataContext
            DataContext = this;

            // Giá trị mặc định

            // Nếu muốn default loại là Full-time:
            // VaiTro = "Full-time";  // sẽ tự sinh mã luôn
        }
        private void CmbVaiTro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbVaiTro.SelectedItem is ComboBoxItem selected)
            {
                string vaiTro = selected.Content.ToString();

                NhanVienBLL bll = new NhanVienBLL();
                txtMaNV.Text = bll.TaoMaNhanVienTheoVaiTro(vaiTro);
            }
        }


        // ===== Sinh mã nhân viên theo loại =====


        // ===== Event UI =====
        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ======== Validate dữ liệu ========
                string ten = txtTen.Text.Trim();
                string sdt = txtSDT.Text.Trim();
                string email = txtEmail.Text.Trim();
                string vaiTro = cmbVaiTro.Text;
                string trangThai = cmbTrangThai.Text;

                if (string.IsNullOrWhiteSpace(ten))
                {
                    MessageBox.Show("Tên nhân viên không được để trống!");
                    txtTen.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(sdt))
                {
                    MessageBox.Show("Số điện thoại không được để trống!");
                    txtSDT.Focus();
                    return;
                }

                // Regex kiểm tra SĐT: chỉ cho phép 10-11 số
                if (!System.Text.RegularExpressions.Regex.IsMatch(sdt, @"^\d{10,11}$"))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ! Phải từ 10 đến 11 chữ số.");
                    txtSDT.Focus();
                    return;
                }

                // Email nếu có nhập thì kiểm tra định dạng
                if (!string.IsNullOrWhiteSpace(email))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(email,
                        @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        MessageBox.Show("Email không hợp lệ!");
                        txtEmail.Focus();
                        return;
                    }
                }
                var result = MessageBox.Show(
           $"Bạn có chắc muốn tạo nhân viên: {ten} ?",
           "Xác nhận tạo nhân viên",
           MessageBoxButton.YesNo,
           MessageBoxImage.Question
       );

                if (result != MessageBoxResult.Yes)
                    return; // Người dùng chọn No, dừng
                // ======== Tạo object nhân viên ========
                NhanVienModel nv = new NhanVienModel
                {
                    TenNV = ten,
                    SDT = sdt,
                    Email = email,
                    VaiTro = vaiTro,
                    NgayVaoLam = dpNgayVaoLam.SelectedDate ?? DateTime.Now,
                    TrangThai = trangThai,
                    GhiChu = txtGhiChu.Text.Trim()
                };

                // ======== Gọi BLL thêm nhân viên ========
                NhanVienBLL bll = new NhanVienBLL();
                bool ok = bll.ThemNhanVien(nv);

                if (ok)
                {
                    MessageBox.Show(
                        $"Thêm thành công!\n" +
                        $"Mã NV: {nv.MaNV}\n" +
                        $"Username: {nv.Username}\n" +
                        "Password mặc định: 123456"
                    );
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Lỗi thêm nhân viên!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

    }
}
