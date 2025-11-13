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

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.DatSan
{
    public partial class frmTaoLichDat : Window
    {
        public frmTaoLichDat()
        {
            InitializeComponent();

            // Gán ngày mặc định cho ô "Ngày tạo đơn"
            txtNgayTaoDon.Text = DateTime.Now.ToString("dd/MM/yyyy");
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

