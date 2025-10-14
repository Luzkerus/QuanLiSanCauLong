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

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.QuanLySan
{
    /// <summary>
    /// Interaction logic for frmThemSanMoi.xaml
    /// </summary>
    public partial class frmThemSanMoi : Window
    {
        public frmThemSanMoi()
        {
            InitializeComponent();
            this.Opacity = 0; // bắt đầu mờ hoàn toàn

            // Khi form load thì chạy animation
            this.Loaded += (s, e) =>
            {
                var fadeIn = new System.Windows.Media.Animation.DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250));
                this.BeginAnimation(Window.OpacityProperty, fadeIn);
            };
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnDongForm(object sender, MouseButtonEventArgs e)
        {
            this.Close(); // Đóng form hiện tại
        }
    }
}
