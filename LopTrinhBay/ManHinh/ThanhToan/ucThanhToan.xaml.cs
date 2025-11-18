using Microsoft.CSharp.RuntimeBinder;
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

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.ThanhToan
{
    /// <summary>
    /// Interaction logic for ucThanhToan.xaml
    /// </summary>
    public partial class ucThanhToan : UserControl
    {
        private readonly KhachHangBLL khBLL = new KhachHangBLL();
        public ucThanhToan()
        {
            InitializeComponent();
        }

        private void btnLichSuThanhToan_Click(object sender, RoutedEventArgs e)
        {
            var frm = new frmLichSuThanhToan();
            frm.ShowDialog();
        }
        private void txtSDT_TextChanged(object sender, RoutedEventArgs e)
        {
            LoadKhachHang(txtSDT.Text);
            LoadChiTietChuaThanhToan(txtSDT.Text);
        }
        private void LoadChiTietChuaThanhToan(string sdt)
        {
            var chiTietBLL = new ChiTietDatSanBLL();
            var danhSach = chiTietBLL.LayDanhSachChuaThanhToan(sdt);
            dataGridChuaThanhToan.ItemsSource = danhSach;
            TinhVaHienThiTongTienSan();
        }
        private void LoadKhachHang(string sdt)
        {
            if (string.IsNullOrEmpty(sdt))
            {
                txtTenKH.Text = "";
                return;
            }

            var kh = khBLL.LayKhachHangTheoSDT(sdt);
            if (kh != null)
            {
                txtTenKH.Text = kh.Ten;
            }
            else
            {
                txtTenKH.Text = "";
            }
        }

        private void TinhVaHienThiTongTienSan()
        {
            // Lấy nguồn dữ liệu từ DataGrid
            if (dataGridChuaThanhToan.ItemsSource is IEnumerable<dynamic> danhSachChiTiet)
            {
                decimal tongTienSan = 0;

                // Lặp qua các mục và tính tổng nếu mục đó được check
                foreach (var chiTiet in danhSachChiTiet)
                {
                    // Kiểm tra xem mục có thuộc tính IsChecked và giá trị là true không
                    bool isChecked = false;
                    try
                    {
                        isChecked = chiTiet.IsChecked; // Giả định có thuộc tính IsChecked
                    }
                    catch (RuntimeBinderException)
                    {
                        // Xử lý nếu thuộc tính không tồn tại, có thể bỏ qua hoặc báo lỗi
                        // Trong trường hợp này, nếu không có IsChecked, ta bỏ qua mục đó.
                    }

                    if (isChecked)
                    {
                        decimal thanhTien = 0;
                        try
                        {
                            thanhTien = chiTiet.ThanhTien; // Giả định có thuộc tính ThanhTien
                        }
                        catch (RuntimeBinderException)
                        {
                            // Xử lý nếu thuộc tính không tồn tại
                        }

                        tongTienSan += thanhTien;
                    }
                }

                // Hiển thị tổng tiền
                txtTongTienSan.Text = string.Format("{0:N0} VNĐ", tongTienSan);
            }
            else
            {
                txtTongTienSan.Text = "0 VNĐ";
            }
        }
        private void Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            TinhVaHienThiTongTienSan(); // Tính lại ngay
        }

        private void Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            TinhVaHienThiTongTienSan(); // Tính lại ngay
        }

        // Event cho "Chọn tất cả" (nếu dùng checkbox ở header)
        private void ChkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            if (dataGridChuaThanhToan.ItemsSource is IEnumerable<dynamic> items)
            {
                foreach (var item in items)
                {
                    try { item.IsChecked = true; }
                    catch { }
                }
            }
            TinhVaHienThiTongTienSan();
        }

        private void ChkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            if (dataGridChuaThanhToan.ItemsSource is IEnumerable<dynamic> items)
            {
                foreach (var item in items)
                {
                    try { item.IsChecked = false; }
                    catch { }
                }
            }
            TinhVaHienThiTongTienSan();
        }

        // Event thay thế nếu dùng RowEditEnding (cho trường hợp checkbox không trigger trực tiếp)
        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            TinhVaHienThiTongTienSan(); // Gọi khi edit row kết thúc (bao gồm check)
        }
    }
}
