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

using QuanLiSanCauLong.LopTrinhBay.ManHinh.KhoPOS;

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
                    DataContext = this;
            };
        }

        private void btnNhapHang(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);

            var frm = new frmNhapHang();


            if (parentWindow != null)
            {
                frm.Owner = parentWindow;
                frm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            frm.ShowDialog();
        }

        private void btnLichSuNhap(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);

            var frm = new frmLichsunhap();


            if (parentWindow != null)
            {
                frm.Owner = parentWindow;
                frm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            frm.ShowDialog();
        }

        private void btnThanhToan(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);

            var frm = new frmPhieuThanhToanPOS();


            if (parentWindow != null)
            {
                frm.Owner = parentWindow;
                frm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            frm.ShowDialog();
        }
    }
}

