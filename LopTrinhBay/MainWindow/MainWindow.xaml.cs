using QuanLiSanCauLong.LopNghiepVu;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.BaoCao;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.DatSan;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.HeThong;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.KhachHoiVien;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.KhoPOS;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.NhanVien;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.QuanLySan;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.ThanhToan;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.TongQuan;
using System;
using System.Windows;

// using ... các màn khác

namespace QuanLiSanCauLong
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Mặc định nạp trang Tổng quan/Overview vào vùng nội dung
            // Ví dụ: MainFrame.Content = new TongQuat.ucTongQuan(); 
            // hoặc 1 UserControl tổng quan:
           MainFrame.Content = new ucTongQuan(); // thay bằng màn thật 
        }


        // TẤT CẢ trang chức năng chỉ chạy ở đây (ô trắng)
        private void Sidebar_NavigateRequested(object sender, string key)
        {
            switch (key)
            {
                case "overview": MainFrame.Content = new ucTongQuan(); break;
                case "courts": MainFrame.Content = new ucQuanLySan(); break;
                case "booking": MainFrame.Content = new ucDatSan(); break;
                case "customers": MainFrame.Content = new ucKhachHoiVien(); break;
                case "payment": MainFrame.Content = new ucThanhToan(); break;
                case "pos": MainFrame.Content = new UcKhoDashboard(); break;
                case "staff": MainFrame.Content = new ucNhanVien(); break;
                case "reports": MainFrame.Content = new ucBaoCao(); break;
                case "settings": MainFrame.Content = new ucCauHinhHeThong(); break;
            }
        }

    }
}
