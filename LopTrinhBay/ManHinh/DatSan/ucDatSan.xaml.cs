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

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.DatSan
{
    /// <summary>
    /// Interaction logic for ucDatSan.xaml
    /// </summary>
    public partial class ucDatSan : UserControl
    {
        public ucDatSan()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnDatSanMoi_Click(object sender, RoutedEventArgs e)
        {
            frmTaoLichDat taoLichDat = new frmTaoLichDat();

            taoLichDat.ShowDialog();
        }
    }
}
