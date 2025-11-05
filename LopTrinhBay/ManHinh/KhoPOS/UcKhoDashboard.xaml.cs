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

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.KhoPOS
{
    /// <summary>
    /// Interaction logic for UcKhoDashboard.xaml
    /// </summary>
    public partial class UcKhoDashboard : UserControl
    {
        public UcKhoDashboard()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                if (DataContext == null)
                    DataContext = new UcKhoDashboard();
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
    }
}
