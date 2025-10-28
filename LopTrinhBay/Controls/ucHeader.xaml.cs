using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Threading;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.Controls
{
    public partial class ucHeader : UserControl
    {
        private readonly DispatcherTimer timer = new DispatcherTimer();

        public ucHeader()
        {
            InitializeComponent();

            UpdateDateTimeText();

            // Cập nhật mỗi phút (có thể đổi sang mỗi giây nếu cần)
            timer.Interval = TimeSpan.FromMinutes(1);
            timer.Tick += (s, e) => UpdateDateTimeText();
            timer.Start();
        }

        private void UpdateDateTimeText()
        {
            var culture = new CultureInfo("vi-VN");
            var now = DateTime.Now;

            // Ví dụ: "Thứ Ba, 28 tháng 10, 2025"
            string formatted = $"{culture.DateTimeFormat.GetDayName(now.DayOfWeek)}, {now:dd 'tháng' MM, yyyy}";
            formatted = char.ToUpper(formatted[0], culture) + formatted.Substring(1); // viết hoa chữ đầu
            txtDate.Text = formatted;
        }
    }
}
