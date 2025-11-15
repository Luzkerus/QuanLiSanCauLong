using QuanLiSanCauLong.LopNghiepVu;
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;

namespace QuanLiSanCauLong.LopTrinhBay.KhachHoiVien
{
    public partial class frmThemHoiVien : Window
    {
        private KhachHang khachHangSua;

        public frmThemHoiVien(KhachHang kh = null)
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                var fadeIn = new System.Windows.Media.Animation.DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250));
                this.BeginAnimation(Window.OpacityProperty, fadeIn);
            };

            if (kh != null)
            {
                khachHangSua = kh;
                LoadData(khachHangSua);
            }
        }

        private void LoadData(KhachHang kh)
        {
            txtTen.Text = kh.Ten;
            txtEmail.Text = kh.Email;
            txtSdt.Text = kh.SDT;       // SDT hiện tại (khóa chính, không sửa trực tiếp)
            txtSdtPhu.Text = kh.SDTPhu; // SDT mới tạm thời (có thể sửa)
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            lblError.Text = "";

            // Validate form
            if (!ValidateInput(out string errMsg))
            {
                lblError.Text = errMsg;
                return;
            }

            // Lấy dữ liệu từ form
            khachHangSua.Ten = txtTen.Text.Trim();
            khachHangSua.Email = txtEmail.Text.Trim();
            khachHangSua.SDTPhu = string.IsNullOrWhiteSpace(txtSdtPhu.Text) ? null : txtSdtPhu.Text.Trim();

            string sdtMoi = string.IsNullOrWhiteSpace(txtSdt.Text) ? khachHangSua.SDT : txtSdt.Text.Trim();
            string sdtPhuMoi = string.IsNullOrWhiteSpace(txtSdtPhu.Text) ? null : txtSdtPhu.Text.Trim();

            try
            {
                KhachHangBLL bll = new KhachHangBLL();

                // Kiểm tra trùng SDT mới với SDT chính hoặc SDT phụ
                if (!string.IsNullOrWhiteSpace(sdtMoi) && bll.KiemTraTrungSDTChinh(sdtMoi, khachHangSua.SDT))
                {
                    lblError.Text = "Số điện thoại chính mới đã tồn tại.";
                    return;
                }

                if (!string.IsNullOrWhiteSpace(sdtPhuMoi) && bll.KiemTraTrungSDTPhu(sdtPhuMoi, khachHangSua.SDT))
                {
                    lblError.Text = "Số điện thoại phụ mới đã tồn tại.";
                    return;
                }

                // Cập nhật Khách hàng (SDT mới, Ten, Email, SDTPhu)
                bll.CapNhatKhachHang(khachHangSua, sdtMoi);

                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                lblError.Text = "Không thể lưu. Chi tiết: " + ex.Message;
            }
        }

        private bool ValidateInput(out string errMsg)
        {
            errMsg = "";

            if (string.IsNullOrWhiteSpace(txtTen.Text))
            {
                errMsg = "Vui lòng nhập tên khách hàng.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtSdt.Text))
            {
                if (!Regex.IsMatch(txtSdt.Text.Trim(), @"^0\d{9}$"))
                {
                    errMsg = "Số điện thoại mới không hợp lệ (0xxxxxxxxx).";
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtSdtPhu.Text))
            {
                if (!Regex.IsMatch(txtSdtPhu.Text.Trim(), @"^0\d{9}$"))
                {
                    errMsg = "Số điện thoại phụ không hợp lệ (0xxxxxxxxx).";
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                if (!Regex.IsMatch(txtEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    errMsg = "Email không hợp lệ.";
                    return false;
                }
            }

            return true;
        }


    }
}
