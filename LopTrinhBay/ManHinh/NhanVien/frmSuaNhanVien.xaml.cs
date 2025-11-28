using QuanLiSanCauLong.LopDuLieu;
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
using NhanVienModel= QuanLiSanCauLong.LopDuLieu.NhanVien;
using QuanLiSanCauLong.LopNghiepVu;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.NhanVien
{
    /// <summary>
    /// Interaction logic for frmSuaNhanVien.xaml
    /// </summary>
    public partial class frmSuaNhanVien : Window
    {
        NhanVienModel nvCu;
        public frmSuaNhanVien(NhanVienModel nv)
        {
            InitializeComponent();
            nvCu = nv;
            LoadNhanVien(nv);

        }
        private void LoadNhanVien(NhanVienModel nv)
        {
            txtMaNV.Text = nv.MaNV;
            txtTen.Text = nv.TenNV;
            txtEmail.Text = nv.Email;
            txtSDT.Text = nv.SDT;
            dpNgayVaoLam.Text = nv.NgayVaoLam.ToString("dd/MM/yyyy");
            txtGhiChu.Text = nv.GhiChu;
            switch (nv.VaiTro)
            {
                case "Quản lý":
                    cmbVaiTro.SelectedIndex = 0;
                    break;
                case "Full-time":
                    cmbVaiTro.SelectedIndex = 1;
                    break;
                case "Part-time":
                    cmbVaiTro.SelectedIndex = 2;
                    break;
                default:
                    cmbVaiTro.SelectedIndex = -1;
                    break;
            }

            int trangthaiIndex = -1;
            if (nv.TrangThai == "Đang làm")
                trangthaiIndex = 0;
            else if (nv.TrangThai == "Tạm ngưng")
                trangthaiIndex = 1;
            else trangthaiIndex = 2;

            cmbTrangThai.SelectedIndex = trangthaiIndex;
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // 1. Kiểm tra SĐT
            if (string.IsNullOrWhiteSpace(txtSDT.Text))
            {
                MessageBox.Show("Số điện thoại không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Regex kiểm tra SĐT (chỉ số, 9-11 chữ số)
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtSDT.Text, @"^\d{9,11}$"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. Kiểm tra Email (cho phép trống)
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(txtEmail.Text);
                    if (addr.Address != txtEmail.Text)
                    {
                        MessageBox.Show("Email không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("Email không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // 3. Xác nhận trước khi lưu
            var result = MessageBox.Show("Bạn có chắc chắn muốn lưu thay đổi?",
                                         "Xác nhận",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            // 4. Tạo đối tượng mới
            NhanVienModel nvMoi = new NhanVienModel
            {
                MaNV = txtMaNV.Text,
                TenNV = txtTen.Text,
                Email = txtEmail.Text,
                SDT = txtSDT.Text,
                NgayVaoLam = DateTime.Parse(dpNgayVaoLam.Text),
                GhiChu = txtGhiChu.Text,
                VaiTro = (cmbVaiTro.SelectedItem as ComboBoxItem)?.Content.ToString(),
                TrangThai = (cmbTrangThai.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Username = nvCu.Username,
                PasswordHash = nvCu.PasswordHash
            };

            // 5. Lưu dữ liệu
            NhanVienBLL nhanVienBLL = new NhanVienBLL();
            if (nhanVienBLL.SuaNhanVienVaMaNV(nvMoi,nvCu.MaNV))
            {
                // Cập nhật thành công
                // 6. Thông báo thành công
                MessageBox.Show("Lưu thông tin nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Lưu thông tin nhân viên thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

          
            

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Lưu thông tin nhân viên sau khi sửa
            // Hiện tại chỉ đóng form
            this.Close();
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            // Lưu thông tin nhân viên sau khi sửa
            // Hiện tại chỉ đóng form
            this.Close();
        }

        private void CmbVaiTro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbVaiTro.SelectedItem is ComboBoxItem selected)
            {
                string vaiTro = selected.Content.ToString();

                NhanVienBLL bll = new NhanVienBLL();
                if (nvCu.VaiTro != vaiTro) // Chỉ đổi mã khi thay đổi vai trò
                    txtMaNV.Text = bll.TaoMaNhanVienTheoVaiTro(vaiTro);
            }
        }
    }
}
