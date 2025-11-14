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
using QuanLiSanCauLong.LopDuLieu;
using System.Windows.Media.Imaging;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.DatSan
{
    public partial class frmTaoLichDat : Window
    {
        private KhachHangBLL khBLL = new KhachHangBLL();
        private SanBLL sanBLL = new SanBLL();
        private BangGiaBLL bgBLL = new BangGiaBLL();
        private List<ChiTietDatSan> GioDat = new List<ChiTietDatSan>();
        public int GioCount => GioDat.Count;

        public frmTaoLichDat()
        {
            InitializeComponent();
            LoadData();
            // Gán ngày mặc định cho ô "Ngày tạo đơn"
            txtNgayTaoDon.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        private void txtSDT_TextChanged(object sender, RoutedEventArgs e)
        {
            LoadKhachHang(txtSDT.Text);
        }
        private void LoadData()
        {
            if (bgBLL.LayBangGiaChung() != null)
            {
                dgBangGiaPreview.ItemsSource = bgBLL.LayBangGiaChung().DefaultView;
            }
        }
        private void LoadKhachHang(string sdt)
        {
            if (string.IsNullOrEmpty(sdt))
            {
                txtTenKH.Text = "";
                txtEmail.Text = "";
                return;
            }

            var kh = khBLL.LayKhachHangTheoSDT(sdt);
            if (kh != null)
            {
                txtTenKH.Text = kh.Ten;
                txtEmail.Text = kh.Email;
            }
            else
            {
                txtTenKH.Text = "";
                txtEmail.Text = "";
            }
        }

        private void dpNgayDat_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpNgayDat.SelectedDate != null)
            {
                LoadSanTheoNgay(dpNgayDat.SelectedDate.Value);
            }
        }

        private void LoadSanTheoNgay(DateTime ngayDat)
        {
            var dsSan = sanBLL.LaySanHoatDongTheoNgay(ngayDat);

            cboSan.ItemsSource = dsSan;
            cboSan.DisplayMemberPath = "TenSan";
            cboSan.SelectedIndex = -1;
        }


        private void BtnThemVaoGio_Click(object sender, RoutedEventArgs e)
        {
            if (cboSan.SelectedItem == null || dpNgayDat.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày và sân.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TimeSpan.TryParse(txtGioBatDau.Text, out var gioBatDau) ||
                !TimeSpan.TryParse(txtGioKetThuc.Text, out var gioKetThuc))
            {
                MessageBox.Show("Giờ bắt đầu hoặc giờ kết thúc không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (gioBatDau >= gioKetThuc)
            {
                MessageBox.Show("Giờ kết thúc phải lớn hơn giờ bắt đầu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var san = (San)cboSan.SelectedItem;

            // Kiểm tra trùng ngày + sân + giờ
            bool trungLap = GioDat.Any(g =>
                g.MaSan == san.MaSan &&
                g.NgayDat == dpNgayDat.SelectedDate.Value &&
                ((gioBatDau >= g.GioBatDau && gioBatDau < g.GioKetThuc) ||
                 (gioKetThuc > g.GioBatDau && gioKetThuc <= g.GioKetThuc) ||
                 (gioBatDau <= g.GioBatDau && gioKetThuc >= g.GioKetThuc))
            );

            if (trungLap)
            {
                MessageBox.Show("Sân này đã được đặt trong khoảng thời gian này.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Tạo chi tiết đặt sân
            var chiTiet = new ChiTietDatSan
            {
                MaSan = san.MaSan,
                TenSanCached = san.TenSan,
                NgayDat = dpNgayDat.SelectedDate.Value,
                GioBatDau = gioBatDau,
                GioKetThuc = gioKetThuc,
                DonGia = bgBLL.TinhDonGia(dpNgayDat.SelectedDate.Value, gioBatDau, gioKetThuc),
                PhuThuLe = bgBLL.TinhPhuThu(dpNgayDat.SelectedDate.Value, gioBatDau, gioKetThuc),
                ThanhTien = bgBLL.TinhTongTien(dpNgayDat.SelectedDate.Value, gioBatDau, gioKetThuc) 
            };

            // Thêm vào giỏ
            GioDat.Add(chiTiet);
            dgChiTietDat.ItemsSource = null;
            dgChiTietDat.ItemsSource = GioDat;
            CapNhatGioCount();


        }
        // Cập nhật ComboBox (loại sân vừa thêm khỏi list)
        private void BtnXoaGio_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is ChiTietDatSan chiTiet)
            {
                // Xóa khỏi giỏ
                GioDat.Remove(chiTiet);

                // Cập nhật DataGrid
                dgChiTietDat.ItemsSource = null;
                dgChiTietDat.ItemsSource = GioDat;
                CapNhatGioCount();

                // Nếu muốn, cập nhật lại danh sách ComboBox để chọn sân
                if (dpNgayDat.SelectedDate != null)
                    LoadSanTheoNgay(dpNgayDat.SelectedDate.Value);
            }

        }
        private void CapNhatGioCount()
        {
            // Gán lại DataContext để refresh Binding
            this.DataContext = null;
            this.DataContext = this;
        }


        private void BtnTaoDon_Click(object sender, RoutedEventArgs e)
        {
            if (GioDat.Count == 0)
            {
                MessageBox.Show("Chưa có sân nào trong giỏ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string sdt = txtSDT.Text.Trim();
            if (string.IsNullOrEmpty(sdt))
            {
                MessageBox.Show("Chưa có khách hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var bll = new DatSanBLL();
            bool kq = bll.TaoDon(sdt, GioDat);

            if (kq)
            {
                MessageBox.Show("Tạo đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                GioDat.Clear();
                dgChiTietDat.ItemsSource = null;
                CapNhatGioCount();
            }
            else
                MessageBox.Show("Lỗi khi lưu đơn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }


        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { /* ignore */ }
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        
    }
}

