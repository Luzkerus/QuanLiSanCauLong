using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.TongQuan
{
    public partial class frmTongQuan : Window
    {
        // ===== KPI & dữ liệu mock =====
        public decimal DoanhThuHomNay { get; set; }
        public string BienDongDoanhThu { get; set; }
        public int SoLuotDatSan { get; set; }
        public string BienDongLuotDat { get; set; }
        public int HoiVienMoi { get; set; }
        public string BienDongHoiVien { get; set; }
        public double TiLeLapDay { get; set; }
        public string BienDongLapDay { get; set; }

        // KHÔNG dùng target-typed new()
        public List<ItemTinhTrangSan> TinhTrangSan { get; set; } = new List<ItemTinhTrangSan>();
        public List<ItemTopHoiVien> TopHoiVien { get; set; } = new List<ItemTopHoiVien>();
        public List<ItemLichDat> LichDatGanDay { get; set; } = new List<ItemLichDat>();

        public frmTongQuan()
        {
            InitializeComponent();

            // ====== DỮ LIỆU MẪU (dashboard) ======
            DoanhThuHomNay = 12_450_000M;
            BienDongDoanhThu = "↑ +12.5%";
            SoLuotDatSan = 48;
            BienDongLuotDat = "↑ +8.2%";
            HoiVienMoi = 23;
            BienDongHoiVien = "↑ +15.3%";
            TiLeLapDay = 0.85;
            BienDongLapDay = "↓ −2.1%";

            TinhTrangSan = new List<ItemTinhTrangSan>
            {
                new ItemTinhTrangSan("Sân 1","Đang chơi","Đến 15:30"),
                new ItemTinhTrangSan("Sân 2","Trống","Đến 16:00"),
                new ItemTinhTrangSan("Sân 3","Đã đặt","Đến 15:00"),
                new ItemTinhTrangSan("Sân 4","Đang chơi","Đến 18:00"),
                new ItemTinhTrangSan("Sân 5","Đang chơi","Đến 19:30"),
                new ItemTinhTrangSan("Sân 6","Trống",""),
            };

            TopHoiVien = new List<ItemTopHoiVien>
            {
                new ItemTopHoiVien("Nguyễn Văn A",28,2450),
                new ItemTopHoiVien("Trần Thị B",24,1890),
                new ItemTopHoiVien("Lê Văn C",19,1560),
                new ItemTopHoiVien("Phạm Thị D",16,1320),
            };

            LichDatGanDay = new List<ItemLichDat>
            {
                new ItemLichDat("Sân 1","Nguyễn Văn A","14:00 - 15:30","Đang chơi"),
                new ItemLichDat("Sân 3","Trần Thị B","15:00 - 16:00","Đã đặt"),
                new ItemLichDat("Sân 2","Lê Văn C","16:00 - 17:30","Đã đặt"),
                new ItemLichDat("Sân 4","Phạm Thị D","17:00 - 18:00","Đã đặt"),
                new ItemLichDat("Sân 5","Hoàng Văn E","18:00 - 19:30","Đã đặt"),
            };

            DataContext = this;
        }

        // ============================================================
        //  HELPER: Mở form thực tế theo danh sách ứng viên
        // ============================================================
        private void OpenFirst(params string[] candidateFullTypeNames)
        {
            var asms = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var fullName in candidateFullTypeNames.Where(s => !string.IsNullOrWhiteSpace(s)))
            {
                var t = asms.Select(a => a.GetType(fullName, false, false))
                            .FirstOrDefault(tp => tp != null);
                if (t == null) continue;

                // Nếu form đang mở rồi thì kích hoạt
                var existed = Application.Current.Windows.OfType<Window>()
                                 .FirstOrDefault(w => w.GetType().FullName == fullName);
                if (existed != null)
                {
                    existed.Activate();
                    Close();
                    return;
                }

                try
                {
                    var instance = Activator.CreateInstance(t);
                    if (instance is Window w)
                    {
                        w.Show();
                        Close();
                        return;
                    }
                    if (instance is Page p)
                    {
                        var host = new Window
                        {
                            Title = p.Title,
                            Content = p,
                            Width = 1200,
                            Height = 800,
                            WindowStartupLocation = WindowStartupLocation.CenterScreen
                        };
                        host.Show();
                        Close();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không mở được: " + fullName + "\nChi tiết: " + ex.Message,
                                    "Lỗi mở màn hình", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            MessageBox.Show("⚠️ Không tìm thấy form tương ứng. Kiểm tra lại namespace/tên lớp.",
                            "Không tìm thấy màn hình", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        // ============================================================
        //  CÁC HÀM ĐIỀU HƯỚNG MENU
        // ============================================================
        private void Menu_QuanLySan(object sender, MouseButtonEventArgs e)
        {
            OpenFirst("QuanLiSanCauLong.LopTrinhBay.ManHinh.QuanLySan.frmQuanLySan");
        }

        private void Menu_DatSan(object sender, MouseButtonEventArgs e)
        {
            OpenFirst("QuanLiSanCauLong.LopTrinhBay.ManHinh.DatSan.LichDatSan",
                      "QuanLiSanCauLong.LopTrinhBay.ManHinh.DatSan.frmTaoLichDat");
        }

        private void btnDatSanMoi(object sender, MouseButtonEventArgs e)
        {
            OpenFirst("QuanLiSanCauLong.LopTrinhBay.ManHinh.DatSan.frmTaoLichDat",
                      "QuanLiSanCauLong.LopTrinhBay.ManHinh.DatSan.LichDatSan");
        }

        private void Menu_KhachHang(object sender, MouseButtonEventArgs e)
        {
            OpenFirst("QuanLiSanCauLong.LopTrinhBay.ManHinh.KhachHoiVien.frmKhachHoiVien",
                      "QuanLiSanCauLong.LopTrinhBay.ManHinh.KhachHoiVien.DanhSachKhach");
        }

        private void Menu_ThanhToan(object sender, MouseButtonEventArgs e)
        {
            OpenFirst("QuanLiSanCauLong.LopTrinhBay.ManHinh.ThanhToan.frmThanhToan");
        }

        private void Menu_KhoPOS(object sender, MouseButtonEventArgs e)
        {
            OpenFirst("QuanLiSanCauLong.LopTrinhBay.ManHinh.BanKhoChoThue.frmBanKhoChoThue",
                      "QuanLiSanCauLong.LopTrinhBay.ManHinh.BanKhoChoThue.DanhSachHang");
        }

        private void Menu_NhanVien(object sender, MouseButtonEventArgs e)
        {
            OpenFirst("QuanLiSanCauLong.LopTrinhBay.ManHinh.HeThong.frmNhanVien",
                      "QuanLiSanCauLong.LopTrinhBay.ManHinh.HeThong.NhanVienView");
        }

        private void Menu_BaoCao(object sender, MouseButtonEventArgs e)
        {
            OpenFirst("QuanLiSanCauLong.LopTrinhBay.ManHinh.BaoCao.frmBaoCao",
                      "QuanLiSanCauLong.LopTrinhBay.ManHinh.BaoCao.BaoCaoTongQuat");
        }

        private void Menu_CaiDat(object sender, MouseButtonEventArgs e)
        {
            OpenFirst("QuanLiSanCauLong.LopTrinhBay.ManHinh.HeThong.frmPhanQuyen",
                      "QuanLiSanCauLong.LopTrinhBay.ManHinh.HeThong.CaiDatView",
                      "QuanLiSanCauLong.LopTrinhBay.ManHinh.XacThuc.frmPhanQuyen");
        }
    }

    // ===== Top-level DTOs (không nested để XAML luôn nhận ra) =====
    public class ItemTinhTrangSan
    {
        public string TenSan { get; set; }
        public string TrangThai { get; set; } // Trống | Đang chơi | Đã đặt | Bảo trì
        public string Den { get; set; }
        public ItemTinhTrangSan(string ten, string tt, string den)
        { TenSan = ten; TrangThai = tt; Den = den; }
    }

    public class ItemTopHoiVien
    {
        public string HoTen { get; set; }
        public int LuotChoi { get; set; }
        public int Diem { get; set; }
        public string TenVietTat
        {
            get
            {
                var parts = (HoTen ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var last2 = parts.Reverse().Take(2).Reverse().ToArray();
                var s = "";
                foreach (var p in last2) if (p.Length > 0) s += char.ToUpperInvariant(p[0]);
                return s;
            }
        }
        public ItemTopHoiVien(string ho, int luot, int diem)
        { HoTen = ho; LuotChoi = luot; Diem = diem; }
    }

    public class ItemLichDat
    {
        public string TenSan { get; set; }
        public string KhachHang { get; set; }
        public string KhoangGio { get; set; }
        public string TrangThai { get; set; }
        public ItemLichDat(string san, string kh, string gio, string tt)
        { TenSan = san; KhachHang = kh; KhoangGio = gio; TrangThai = tt; }
    }
}
