using QuanLiSanCauLong.LopNghiepVu;
using QuanLiSanCauLong.LopTruyCapDuLieu;
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
using System.ComponentModel; // cho ICollectionView

using System.Windows.Shapes;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.DatSan
{
    /// <summary>
    /// Interaction logic for ucDatSan.xaml
    /// </summary>
    public partial class ucDatSan : UserControl
    {
        DatSanBLL bll = new DatSanBLL();
        private ICollectionView viewDanhSachDatSan;
        private List<ChiTietDatSanVM> danhSachGoc;


        public ucDatSan()
        {
            InitializeComponent();
            LoadData();

        }
        private void LoadData()
        {
            danhSachGoc = bll.LayTatCaDatSan();
            viewDanhSachDatSan = CollectionViewSource.GetDefaultView(danhSachGoc);
            lvDanhSachDatSan.ItemsSource = viewDanhSachDatSan;
            CapNhatKpi();
        }
        private void CapNhatKpi()
        {
            var ds = bll.LayTatCaDatSan();

            txtTongDonHomNay.Text = ds.Count.ToString();
            txtSoDangChoi.Text = ds.Count(x => x.TrangThai == "Đang chơi").ToString();
            txtSoDaDat.Text = ds.Count(x => x.TrangThai == "Đã đặt" || x.TrangThai == "Chưa bắt đầu").ToString();
            txtSoHoanThanh.Text = ds.Count(x => x.TrangThai == "Hoàn thành").ToString();
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private void btnDatSanMoi_Click(object sender, RoutedEventArgs e)
        {
            frmTaoLichDat taoLichDat = new frmTaoLichDat();
            taoLichDat.Owner = Window.GetWindow(this);

            bool? kq = taoLichDat.ShowDialog();
            if (kq == true)
            {
                LoadData();
            }
        }

        private void BatDau_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var chiTiet = btn?.DataContext as ChiTietDatSanVM;
            if (chiTiet == null)
            {
                MessageBox.Show("Không lấy được chi tiết!");
                return;
            }
            DateTime now = DateTime.Now;
            DateTime gioBatDauThucTe = chiTiet.NgayDat.Date + chiTiet.GioBatDau;
            if (now > gioBatDauThucTe.AddMinutes(15))
            {
                ChiTietDatSanDAL ctdal = new ChiTietDatSanDAL();
                bool kq = ctdal.CapNhatTrangThai(chiTiet.MaChiTiet, "Đã hủy");

                if (kq)
                {
                    chiTiet.TrangThai = "Đã hủy";
                    lvDanhSachDatSan.Items.Refresh();
                    CapNhatKpi();

                    MessageBox.Show(
                        $"Chi tiết {chiTiet.MaChiTiet} đã tự động hủy vì trễ hơn 15 phút.",
                        "Đã hủy",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return; // dừng luôn, không cho bắt đầu
                }
            }
            if (now < gioBatDauThucTe)
            {
                MessageBox.Show(
                    $"Chưa đến giờ bắt đầu!\nGiờ bắt đầu: {chiTiet.GioBatDau}",
                    "Không thể bắt đầu",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            if (chiTiet.TrangThai != "Chưa bắt đầu" && chiTiet.TrangThai != "Đã đặt")
            {
                MessageBox.Show($"Chi tiết {chiTiet.MaChiTiet} không thể bắt đầu vì trạng thái hiện tại: {chiTiet.TrangThai}",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ChiTietDatSanDAL dal = new ChiTietDatSanDAL();
            bool ok = dal.CapNhatTrangThai(chiTiet.MaChiTiet, "Đang chơi");

            if (ok)
            {
                chiTiet.TrangThai = "Đang chơi";
                lvDanhSachDatSan.Items.Refresh();
                CapNhatKpi();

                MessageBox.Show($"Chi tiết {chiTiet.MaChiTiet} đã được bắt đầu.",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Không thể cập nhật trạng thái cho chi tiết {chiTiet.MaChiTiet}.",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void KetThuc_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var chiTiet = btn?.DataContext as ChiTietDatSanVM;
            if (chiTiet == null) return;

            if (chiTiet.TrangThai != "Đang chơi")
            {
                MessageBox.Show($"Chi tiết {chiTiet.MaChiTiet} chỉ có thể kết thúc khi đang chơi.",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ChiTietDatSanDAL dal = new ChiTietDatSanDAL();
            bool ok = dal.CapNhatTrangThai(chiTiet.MaChiTiet, "Hoàn thành");

            if (ok)
            {
                chiTiet.TrangThai = "Hoàn thành";
                lvDanhSachDatSan.Items.Refresh();
                CapNhatKpi();

                MessageBox.Show($"Chi tiết {chiTiet.MaChiTiet} đã hoàn thành.",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Không thể cập nhật trạng thái cho chi tiết {chiTiet.MaChiTiet}.",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Huy_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var chiTiet = btn?.DataContext as ChiTietDatSanVM;
            if (chiTiet == null) return;

            if (chiTiet.TrangThai == "Hoàn thành" ||chiTiet.TrangThai =="Đang chơi")
            {
                MessageBox.Show($"Chi tiết {chiTiet.MaChiTiet} đang chơi hoặc đã hoàn thành, không thể hủy.",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ChiTietDatSanDAL dal = new ChiTietDatSanDAL();
            bool ok = dal.CapNhatTrangThai(chiTiet.MaChiTiet, "Đã hủy");

            if (ok)
            {
                chiTiet.TrangThai = "Đã hủy";
                lvDanhSachDatSan.Items.Refresh();
                CapNhatKpi();

                MessageBox.Show($"Chi tiết {chiTiet.MaChiTiet} đã bị hủy.",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Không thể cập nhật trạng thái cho chi tiết {chiTiet.MaChiTiet}.",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            if (viewDanhSachDatSan == null) return;

            string searchText = txtSearch.Text?.Trim().ToLower();
            string trangThai = (cboTrangThai.SelectedItem as ComboBoxItem)?.Content?.ToString();
            DateTime? selectedDate = dpNgayDat.SelectedDate;

            viewDanhSachDatSan.Filter = obj =>
            {
                var item = obj as ChiTietDatSanVM;
                if (item == null) return false;

                // 1. Filter theo trạng thái
                bool trangThaiOk = true;
                if (!string.IsNullOrEmpty(trangThai) && trangThai != "Tất cả trạng thái")
                    trangThaiOk = item.TrangThai == trangThai;

                // 2. Filter theo search text (tên, SDT, sân)
                bool searchOk = true;
                if (!string.IsNullOrEmpty(searchText))
                {
                    searchOk = (item.TenKH?.ToLower().Contains(searchText) ?? false)
                            || (item.SDT?.ToLower().Contains(searchText) ?? false)
                            || (item.TenSanCached?.ToLower().Contains(searchText) ?? false);
                }

                // 3. Filter theo ngày
                bool dateOk = true;
                if (selectedDate.HasValue)
                    dateOk = item.NgayDat.Date == selectedDate.Value.Date;

                return trangThaiOk && searchOk && dateOk;
            };

            viewDanhSachDatSan.Refresh();
        }

        // Event handler cho DatePicker
        private void dpNgayDat_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void cboTrangThai_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }
        private void BtnClearDate_Click(object sender, RoutedEventArgs e)
        {
            dpNgayDat.SelectedDate = null;
            ApplyFilter(); // khi ngày null → xem tất cả
        }



    }
}
