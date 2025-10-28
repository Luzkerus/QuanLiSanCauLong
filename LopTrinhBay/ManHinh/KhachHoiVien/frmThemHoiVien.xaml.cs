using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;

namespace QuanLiSanCauLong.LopTrinhBay.KhachHoiVien
{
    public partial class frmThemHoiVien : Window
    {
        public frmThemHoiVien()
        {
            InitializeComponent();
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            lblError.Text = "";
            var ten = txtTen.Text?.Trim();
            var sdt = txtSdt.Text?.Trim();

            if (string.IsNullOrWhiteSpace(ten))
            { lblError.Text = "Vui lòng nhập Tên hội viên."; txtTen.Focus(); return; }

            // Theo SRS hiện hành: 10 số nội địa, bắt đầu bằng 0 (có thể nâng lên E.164 nếu bạn đổi SRS)
            if (!Regex.IsMatch(sdt ?? "", @"^0\d{9}$"))
            { lblError.Text = "Số điện thoại không hợp lệ (0xxxxxxxxx)."; txtSdt.Focus(); return; }

            try
            {
//                //using (var cn = new SqlConnection(AppConfig.ConnectionString))
//                using (var cmd = cn.CreateCommand())
//                {
//                    cn.Open();

//                    // Check trùng SDT
//                    cmd.CommandText = "SELECT 1 FROM dbo.KhachHang WHERE SDT = @sdt";
//                    cmd.Parameters.AddWithValue("@sdt", sdt);
//                    var exists = cmd.ExecuteScalar() != null;
//                    if (exists)
//                    { lblError.Text = "Số điện thoại đã tồn tại."; return; }

//                    // Thêm hội viên (Email bỏ, dùng DEFAULT cho các cột khác)
//                    cmd.Parameters.Clear();
//                    cmd.CommandText = @"
//INSERT INTO dbo.KhachHang (SDT, TenKH, Email, LoaiKH)
//VALUES (@sdt, @ten, NULL, N'HoiVien');";
//                    cmd.Parameters.AddWithValue("@sdt", sdt);
//                    cmd.Parameters.AddWithValue("@ten", ten);
//                    cmd.ExecuteNonQuery();
//                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                lblError.Text = "Không thể lưu. Chi tiết: " + ex.Message;
            }
        }
    }
}
