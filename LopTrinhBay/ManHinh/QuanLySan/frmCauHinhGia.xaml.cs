using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopNghiepVu;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.QuanLySan
{
    public partial class frmCauHinhGia : Window
    {
        private BangGiaChung _bangGia;
        private decimal _giaCoBan ;
        private decimal _pctPhuThu ;
        public frmCauHinhGia(BangGiaChung bangGia)
        {
            InitializeComponent();
            _bangGia = bangGia;
            InitToday();
            LoadFromBangGia();
            //LoadDefaults();
            RecalcPreview();
        }

        private void InitToday()
        {
            var now = DateTime.Now;
            lblToday.Text = now.ToString("dddd, dd/MM/yyyy", new CultureInfo("vi-VN"));

            //bool isWeekend = now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday;
            bool isHoliday = IsHoliday(now);
            bool isSpecial =  isHoliday;

            lblSpecial.Text = isSpecial ? "Có" : "Không";
        }
        private bool _isLoading = false;

        private void LoadFromBangGia()
        {
            if (_bangGia == null) return;

            _isLoading = true;

            txtGioBatDau.Text = _bangGia.GioBatDau.ToString(@"hh\:mm");
            txtGioKetThuc.Text = _bangGia.GioKetThuc.ToString(@"hh\:mm");

            _giaCoBan = _bangGia.DonGia;
            _pctPhuThu = _bangGia.PhuThuLePercent ?? 0m;

            txtGiaCoBan.Text = _giaCoBan.ToString("0", CultureInfo.InvariantCulture);
            txtPhuThu.Text = _pctPhuThu.ToString("0", CultureInfo.InvariantCulture);

            cbLoaiNgay.SelectedIndex = _bangGia.LoaiNgay == "Thứ 2-Thứ 6" ? 0 : 1;

            _isLoading = false;
        }


        private void LoadDefaults()
        {
            _giaCoBan = 120000m;
            _pctPhuThu = 5m;

            txtGiaCoBan.Text = _giaCoBan.ToString("0", CultureInfo.InvariantCulture);
            txtPhuThu.Text = _pctPhuThu.ToString("0", CultureInfo.InvariantCulture);
        }

        private bool IsHoliday(DateTime date)
        {
            // TODO: backend xác định ngày lễ thực tế
            return false;
        }

        private static readonly Regex _numRegex = new Regex(@"^[0-9]+([.,][0-9]{0,2})?$");

        private static decimal ParseDecimal(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return 0m;
            input = input.Replace(',', '.');
            return decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out var val) ? val : 0m;
        }

        private void RecalcPreview()
        {
            var now = DateTime.Now;
            //bool isWeekend = now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday;
            bool isHoliday = IsHoliday(now);
            bool isSpecial = isHoliday;

            decimal gia = _giaCoBan;
            string formula = $"Công thức: {gia:0}";
            decimal giaApDung = gia;

            if (isSpecial && _pctPhuThu > 0)
            {
                giaApDung = gia * (1 + _pctPhuThu / 100m);
                formula += $" × (1 + {_pctPhuThu:0}% phụ thu đặc biệt)";
            }

            lblGiaApDung.Text = giaApDung.ToString("#,0", new CultureInfo("vi-VN"));
            lblGiaFormula.Text = formula;
        }

        private void Numeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var tb = (System.Windows.Controls.TextBox)sender;
            var proposed = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength)
                                  .Insert(tb.SelectionStart, e.Text);
            e.Handled = !_numRegex.IsMatch(proposed);
        }

        private void AnyInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_isLoading) return; // ⛔ Bỏ qua nếu đang load dữ liệu ban đầu

            _giaCoBan = ParseDecimal(txtGiaCoBan.Text);
            _pctPhuThu = ParseDecimal(txtPhuThu.Text);
            RecalcPreview();
        }

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
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // --- Bổ sung: Validation - Kiểm tra dữ liệu đầu vào ---

            // 1. Kiểm tra Giờ Bắt Đầu và Giờ Kết Thúc
            TimeSpan gioBatDau;
            TimeSpan gioKetThuc;

            // Sử dụng TryParse để kiểm tra định dạng hh:mm
            if (!TimeSpan.TryParse(txtGioBatDau.Text, out gioBatDau) ||
                !TimeSpan.TryParse(txtGioKetThuc.Text, out gioKetThuc))
            {
                MessageBox.Show("Vui lòng nhập giờ bắt đầu và giờ kết thúc hợp lệ theo định dạng hh:mm.", "Lỗi Dữ Liệu",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Dừng lại nếu dữ liệu không hợp lệ
            }

            // 2. Kiểm tra tính hợp lệ về thời gian (Giờ Kết Thúc phải sau Giờ Bắt Đầu)
            if (gioKetThuc <= gioBatDau)
            {
                MessageBox.Show("Giờ kết thúc phải sau Giờ bắt đầu.", "Lỗi Logic Thời Gian",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Kiểm tra các trường số (Đơn giá, Phụ thu)
            decimal donGia;
            decimal phuThu;

            // Giả định ParseDecimal là một phương thức custom dùng để parse decimal và xử lý định dạng tiền tệ/khoảng trắng
            if (!decimal.TryParse(txtGiaCoBan.Text, out donGia) || donGia <= 0)
            {
                MessageBox.Show("Vui lòng nhập Đơn giá cơ bản hợp lệ (phải lớn hơn 0).", "Lỗi Dữ Liệu",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Không cần kiểm tra phuThu <= 0 vì phụ thu có thể là 0, nhưng nên kiểm tra parse
            if (!decimal.TryParse(txtPhuThu.Text, out phuThu))
            {
                // Sử dụng ParseDecimal của bạn nếu nó xử lý format tốt hơn
                try { phuThu = ParseDecimal(txtPhuThu.Text); }
                catch
                {
                    MessageBox.Show("Vui lòng nhập Phần trăm Phụ thu lễ hợp lệ.", "Lỗi Dữ Liệu",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }


            // --- Thực hiện Lưu Dữ Liệu nếu Validation thành công ---
            try
            {
                // Cập nhật thông tin mới từ form vào đối tượng _bangGia
                _bangGia.GioBatDau = gioBatDau;
                _bangGia.GioKetThuc = gioKetThuc;
                _bangGia.DonGia = donGia;
                _bangGia.LoaiNgay = cbLoaiNgay.Text;
                _bangGia.PhuThuLePercent = phuThu;

                var bll = new BangGiaBLL();
                // Bổ sung: Có thể kiểm tra lỗi trùng lặp/logic nghiệp vụ ở đây
                bool result = bll.SuaBangGia(_bangGia);

                if (result)
                {
                    MessageBox.Show("✅ Cập nhật bảng giá thành công!", "Thông Báo",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true; // Để form cha biết load lại DataGrid
                    this.Close(); // Thường đóng form sau khi lưu thành công
                }
                else
                {
                    // Thêm chi tiết lỗi nếu BLL có thể trả về thông báo lỗi cụ thể
                    MessageBox.Show("❌ Không thể cập nhật bảng giá! (Lỗi nghiệp vụ hoặc Database)", "Lỗi Lưu",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi hệ thống (database down, lỗi kết nối, lỗi BLL/DAL không mong muốn)
                MessageBox.Show($"⚠️ Đã xảy ra lỗi hệ thống khi lưu: {ex.Message}", "Lỗi Hệ Thống",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            LoadFromBangGia();
            RecalcPreview();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
