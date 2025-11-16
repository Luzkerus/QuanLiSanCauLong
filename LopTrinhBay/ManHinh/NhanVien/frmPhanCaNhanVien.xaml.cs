using System.Windows;
using System.Windows.Input;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.NhanVien
{
    public partial class frmPhanCaNhanVien : Window
    {
        public frmPhanCaNhanVien()
        {
            InitializeComponent();
            // DataContext = new PhanCaViewModel(); // khi bạn có ViewModel
        }

        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnAddShift_Click(object sender, RoutedEventArgs e)
        {
            // TODO: gom dữ liệu bên trái -> add vào DanhSachCa (List/ObservableCollection trong ViewModel)
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            // TODO: reset các field form bên trái
        }

        private void BtnSaveAll_Click(object sender, RoutedEventArgs e)
        {
            // TODO: lưu DanhSachCa xuống DB
            DialogResult = true;
            Close();
        }
    }
}
