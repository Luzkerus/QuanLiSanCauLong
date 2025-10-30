using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.QuanLySan
{
    public partial class frmCauHinhGia : Window
    {
        private decimal _giaCoBan = 0m;
        private decimal _pctPhuThu = 0m;

        public frmCauHinhGia()
        {
            InitializeComponent();
            InitToday();
            LoadDefaults();
            RecalcPreview();
        }

        private void InitToday()
        {
            var now = DateTime.Now;
            lblToday.Text = now.ToString("dddd, dd/MM/yyyy", new CultureInfo("vi-VN"));

            bool isWeekend = now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday;
            bool isHoliday = IsHoliday(now);
            bool isSpecial = isWeekend || isHoliday;

            lblSpecial.Text = isSpecial ? "Có" : "Không";
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
            bool isWeekend = now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday;
            bool isHoliday = IsHoliday(now);
            bool isSpecial = isWeekend || isHoliday;

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
            _giaCoBan = ParseDecimal(txtGiaCoBan.Text);
            _pctPhuThu = ParseDecimal(txtPhuThu.Text);
            RecalcPreview();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Lưu qua backend
            MessageBox.Show("Đã lưu cấu hình giá đặc biệt (demo).", "Cấu hình giá",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            LoadDefaults();
            RecalcPreview();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
