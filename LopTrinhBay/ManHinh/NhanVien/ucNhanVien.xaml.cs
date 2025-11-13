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


namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.NhanVien
{
    /// <summary>
    /// Interaction logic for ucNhanVien.xaml
    /// </summary>
    public partial class ucNhanVien : UserControl
    {
        public ucNhanVien()
        {
            InitializeComponent();

            // TODO: nếu bạn có ViewModel thì gán DataContext ở đây
            // this.DataContext = new NhanVienViewModel();
        }

        // Nút "Thêm nhân viên" trên header
        private void BtnThemNhanVien_Click(object sender, RoutedEventArgs e)
        {
            // Tạo window thêm nhân viên
            var win = new frmThemNhanVien();

            // Gán Owner là window cha (frmTongQuan / frmMain ...)
            Window parent = Window.GetWindow(this);
            if (parent != null)
            {
                win.Owner = parent;
            }

            // Mở dạng dialog
            bool? result = win.ShowDialog();

            // Nếu lưu thành công (DialogResult = true trong BtnSave của frmThemNhanVien)
            if (result == true)
            {
                // Sau khi thêm xong thì reload lại danh sách nhân viên
                ReloadNhanVien();
            }
        }

        // Hàm reload danh sách nhân viên – bạn tự nối với DB / ViewModel của bạn
        private void ReloadNhanVien()
        {
            // Ví dụ nếu đang dùng ViewModel:
            // if (DataContext is NhanVienViewModel vm)
            // {
            //     vm.LoadNhanVien();
            // }

            // Nếu chưa có ViewModel, bạn có thể TODO ở đây để sau này bổ sung.
        }
    }
}
