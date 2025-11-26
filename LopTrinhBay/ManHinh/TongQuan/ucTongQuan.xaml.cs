using QuanLiSanCauLong.LopTrinhBay.ManHinh.DatSan;
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
using QuanLiSanCauLong.LopTrinhBay.ManHinh.DatSan;
using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopNghiepVu;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.TongQuan
{
    public partial class ucTongQuan : UserControl
    {
        DatSanBLL datSanBLL = new DatSanBLL();
        SanBLL sanBLL = new SanBLL();
        KhachHangBLL khachHangBLL = new KhachHangBLL();
        ThanhToanBLL thanhToanBLL = new ThanhToanBLL();
        public ucTongQuan()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData() 
        { 
            dgLichDatGanDay.ItemsSource = datSanBLL.LayDatSanGanDay();
            dgTinhTrangSan.ItemsSource = sanBLL.LayTatCaSan();
            dgTopHoiVien.ItemsSource = khachHangBLL.LayTopHoiVien(6);
            txtDanhThuHomNay.Text = thanhToanBLL.LayDoanhThuHomNay().ToString("N0") + " Đ";
            txtSoLuot.Text = datSanBLL.DemTongSoDatSanHomNay().ToString();
            txtHoiVienMoi.Text = khachHangBLL.LayTongKhachHangMoiTrongThang().ToString();
            txtTyLeLapDay.Text = datSanBLL.TinhTyLeLapDay().ToString("P1");
        }

        private void btnMoDatSan_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var win = new frmTaoLichDat
            {
                Owner = Window.GetWindow(this)
            };
            bool? kq =win.ShowDialog();
            if (kq == true)
            {
                // Xử lý khi người dùng nhấn nút "OK" trong frmTaoLichDat
                LoadData();
            }

        }
    }
}
