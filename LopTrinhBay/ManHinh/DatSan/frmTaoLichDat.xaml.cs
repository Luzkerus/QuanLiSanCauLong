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
        // public int GioCount => GioDat.Count;
        private readonly CauHinhHeThongBLL _cauHinhBLL = new CauHinhHeThongBLL();
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
            var chiTietBLL = new ChiTietDatSanBLL();

            bool trungTrongDB = chiTietBLL.KiemTraTrungLich(
                    san.MaSan,
                    dpNgayDat.SelectedDate.Value,
                    gioBatDau,
                    gioKetThuc);

            if (trungTrongDB)
            {
                MessageBox.Show("Khoảng thời gian này đã có người đặt trong hệ thống!", 
                                "Trùng lịch", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Warning);
                return;
}

            int soSlotToiDa = 10; // Giá trị mặc định an toàn
            int soSanToiDa = 5;   // Giá trị mặc định an toàn

            try
            {
                // Lấy DTO cấu hình
                var config = _cauHinhBLL.LayCauHinhHeThong();

                // Cập nhật giá trị giới hạn từ cấu hình
                if (config.SoSlotToiDa > 0)
                {
                    soSlotToiDa = config.SoSlotToiDa;
                }
                if (config.SoSanToiDa > 0)
                {
                    soSanToiDa = config.SoSanToiDa;
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu không lấy được cấu hình
                Console.WriteLine($"Lỗi khi tải cấu hình giới hạn đặt sân: {ex.Message}");
            }

            // 2. Kiểm tra giới hạn số SLOT
            if (GioDat.Count >= soSlotToiDa)
            {
                MessageBox.Show($"Bạn chỉ được đặt tối đa {soSlotToiDa} slot trong giỏ. Vui lòng thanh toán hoặc xóa bớt.", "Giới hạn slot", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Kiểm tra giới hạn số SÂN KHÁC NHAU
            var distinctSan = GioDat.Select(g => g.MaSan).Distinct().ToList();

            // Kiểm tra: Nếu số lượng sân khác nhau hiện tại BẰNG giới hạn VÀ sân đang chọn KHÔNG phải là sân đã có
            // (Nếu sân đang chọn đã có trong giỏ thì vẫn được thêm slot cho sân đó)
            if (distinctSan.Count >= soSanToiDa && !distinctSan.Contains(san.MaSan))
            {
                MessageBox.Show($"Bạn chỉ được đặt tối đa {soSanToiDa} sân khác nhau trong giỏ.", "Giới hạn sân", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            ClearInputFields();


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
        // Cập nhật số lượng sân trong giỏ
        private void CapNhatGioCount()
        {
            txtGioCount.Text = $"{GioDat.Count} sân";
        }



        // Xóa các trường nhập sau khi thêm vào giỏ
        private void ClearInputFields()
        {
            dpNgayDat.SelectedDate = null;
            cboSan.SelectedIndex = -1;

            txtGioBatDau.Text = "";
            txtGioKetThuc.Text = "";
        }

        // Xử lý sự kiện khi nhấn nút "Tạo đơn"
        private void BtnTaoDon_Click(object sender, RoutedEventArgs e)
        {
            if (GioDat.Count == 0)
            {
                MessageBox.Show("Chưa có sân nào trong giỏ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!KiemTraSDT(txtSDT.Text))
            {
                MessageBox.Show("Số điện thoại không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. Email
            if (!KiemTraEmail(txtEmail.Text))
            {
                MessageBox.Show("Email không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string sdt = txtSDT.Text.Trim();
            string tenKH = txtTenKH.Text.Trim();
            string emailKH = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(sdt))
            {
                MessageBox.Show("Chưa có số điện thoại khách hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var kh = khBLL.LayKhachHangTheoSDT(sdt);

            // Nếu khách hàng chưa có, tạo mới
            if (kh == null)
            {
                kh = new KhachHang
                {
                    SDT = sdt,
                    Ten = string.IsNullOrEmpty(tenKH) ? "Khách mới" : tenKH,
                    Email = emailKH
                };

                bool taoKH = khBLL.ThemKhachHangMoi(kh); // cần phương thức trong BLL để tạo khách
                if (!taoKH)
                {
                    MessageBox.Show("Không thể tạo khách hàng mới!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // Tạo đơn với khách hàng đã có hoặc vừa tạo
            var bll = new DatSanBLL();
            bool kq = bll.TaoDon(kh.SDT, GioDat);

            if (kq)
            {
                MessageBox.Show("Tạo đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                // Thay vì clear giỏ, trả về DialogResult = true và đóng form
                this.DialogResult = true;
                this.Close();
            }
            else
                MessageBox.Show("Lỗi khi lưu đơn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        // Xử lý sự kiện PreviewTextInput để chỉ cho phép nhập số và dấu ":"
        private void Gio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Chỉ cho phép nhập số (0-9) và dấu hai chấm (:)
            if (char.IsDigit(e.Text, 0) || e.Text == ":")
            {
                e.Handled = false; // Cho phép nhập
            }
            else
            {
                e.Handled = true;  // Chặn nhập
            }
        }
        // Xử lý sự kiện TextChanged để định dạng giờ
        private void Gio_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            // Lọc ra chỉ các chữ số
            string currentDigits = new string(textBox.Text.Where(char.IsDigit).ToArray());
            int caretIndex = textBox.CaretIndex;

            // 1. Giới hạn số chữ số tối đa là 4 (hhmm)
            if (currentDigits.Length > 4)
            {
                currentDigits = currentDigits.Substring(0, 4);
                // Điều chỉnh vị trí con trỏ sau khi cắt bớt
                caretIndex = 5;
            }

            string newFormattedText = currentDigits;

            // 2. Tự động chèn dấu ":" nếu có đủ 2 chữ số
            if (currentDigits.Length >= 2)
            {
                // Chèn dấu ":" vào vị trí thứ 2
                newFormattedText = currentDigits.Insert(2, ":");
            }

            // 3. Cập nhật TextBox nếu văn bản đã thay đổi
            if (textBox.Text != newFormattedText)
            {
                textBox.Text = newFormattedText;

                // 4. Điều chỉnh vị trí con trỏ (cần được thực hiện sau khi gán Text)
                if (caretIndex >= 2 && caretIndex < 5 && newFormattedText.Length == 5)
                {
                    // Nếu con trỏ nằm ở vị trí thứ 2, sau khi chèn ":" nó phải là 3
                    // Nếu con trỏ nằm ở vị trí thứ 3/4, sau khi chèn ":" nó phải tăng 1
                    if (textBox.CaretIndex == 2 && currentDigits.Length == 2)
                    {
                        textBox.CaretIndex = 3;
                    }
                    else
                    {
                        textBox.CaretIndex = newFormattedText.Length < 5 ? newFormattedText.Length : 5;
                    }
                }
                else
                {
                    textBox.CaretIndex = newFormattedText.Length < 5 ? newFormattedText.Length : 5;
                }
            }

            // 5. Kiểm tra giới hạn giá trị (23:59)
            if (textBox.Text.Length == 5)
            {
                string hourStr = textBox.Text.Substring(0, 2);
                string minuteStr = textBox.Text.Substring(3, 2);
                bool textChangedDueToValidation = false;

                if (int.TryParse(hourStr, out int hour) && hour > 23)
                {
                    hourStr = "23";
                    textChangedDueToValidation = true;
                }

                if (int.TryParse(minuteStr, out int minute) && minute > 59)
                {
                    minuteStr = "59";
                    textChangedDueToValidation = true;
                }

                if (textChangedDueToValidation)
                {
                    textBox.Text = hourStr + ":" + minuteStr;
                    // Đặt con trỏ về cuối sau khi sửa
                    textBox.CaretIndex = 5;
                }
            }
        }

        // Kiểm tra định dạng số điện thoại
        private bool KiemTraSDT(string sdt)
        {
            if (string.IsNullOrWhiteSpace(sdt)) return false;

            // Chỉ chứa số
            if (!sdt.All(char.IsDigit))
                return false;

            // Độ dài hợp lệ
            return sdt.Length >= 9 && sdt.Length <= 11;
        }
        private bool KiemTraEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true; // cho phép bỏ trống

            try
            {
                var mail = new System.Net.Mail.MailAddress(email);
                return mail.Address == email;
            }
            catch { return false; }
        }




        // Xử lý kéo thả cửa sổ
        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { /* ignore */ }
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Huy_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}

