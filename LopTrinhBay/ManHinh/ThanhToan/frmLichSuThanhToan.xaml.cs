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

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.ThanhToan
{
    /// <summary>
    /// Interaction logic for frmLichSuThanhToan.xaml
    /// </summary>
    public partial class frmLichSuThanhToan : Window
    {
        public frmLichSuThanhToan()
        {
            InitializeComponent();
        }
        // Kéo thả cửa sổ khi giữ chuột ở header
        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Cho phép kéo cửa sổ khi giữ chuột trái
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { /* ignore errors khi click nhanh */ }
            }
        }

        // Thu nhỏ cửa sổ
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // Đóng cửa sổ
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
