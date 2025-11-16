using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.HeThong
{
    public partial class frmThemVaiTro : Window, INotifyPropertyChanged
    {
        // ====== INotifyPropertyChanged ======
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // ==== Các thuộc tính demo để binding (tùy bạn dùng hoặc bỏ) ====
        private string _tenVaiTro;
        public string TenVaiTro
        {
            get => _tenVaiTro;
            set { _tenVaiTro = value; OnPropertyChanged(); }
        }

        private string _moTa;
        public string MoTa
        {
            get => _moTa;
            set { _moTa = value; OnPropertyChanged(); }
        }

        public frmThemVaiTro()
        {
            InitializeComponent();
            DataContext = this; // dùng luôn window làm DataContext
        }

        // ====== EVENT HEADER KÉO FORM ======
        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        // ====== NÚT THU NHỎ ======
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // ====== NÚT ĐÓNG (X) ======
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // ====== NÚT HỦY ======
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // ====== NÚT LƯU VAI TRÒ ======
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // TODO: validate & lưu vai trò vào DB / danh sách

            // Ví dụ: chỉ cần tên vai trò không trống
            if (string.IsNullOrWhiteSpace(TenVaiTro))
            {
                MessageBox.Show("Vui lòng nhập tên vai trò.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Nếu lưu thành công:
            DialogResult = true;
            Close();
        }
    }
}
