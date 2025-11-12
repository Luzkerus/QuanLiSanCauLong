using QuanLiSanCauLong.LopTrinhBay.ManHinh.DatSan;
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

using QuanLiSanCauLong.LopTrinhBay.ManHinh.DatSan;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.TongQuan
{
    public partial class ucTongQuan : UserControl
    {
        public ucTongQuan()
        {
            InitializeComponent();
        }

        private void btnMoDatSan_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var win = new frmTaoLichDat
            {
                Owner = Window.GetWindow(this)
            };
            win.ShowDialog();
        }
    }
}
