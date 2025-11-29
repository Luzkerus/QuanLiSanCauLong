using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLiSanCauLong.LopNghiepVu;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.HeThong
{
    public partial class ucCauHinhHeThong : UserControl, INotifyPropertyChanged
    {
        public ucCauHinhHeThong()
        {
            InitializeComponent();
            DataContext = this;

            // Demo default (đặt theo ảnh)
            GioMoCua = "06:00";
            GioDongCua = "22:00";
            KhoangTGToiThieu = 30;

            DiemTichLuyPer10000 = 1;
            NguongDiemUuDai = 1000;
            TiLeUuDai = 5;

            CanhBaoNoShow = 15;
            SoSanToiDa = 5;
            SoSlotToiDa = 10;

            TimeoutPhien = 30;
            NguongTonKhoThap = 20;
        }
        private readonly NhanVienBLL _nhanVienBLL = new NhanVienBLL();

        private void btnDoiMK_Click(object sender, RoutedEventArgs e)
        {
            if (!SessionManager.IsLoggedIn)
            {
                MessageBox.Show("Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Lấy các giá trị từ PasswordBox
            // PHẢI đảm bảo các PasswordBox có x:Name="TxtOldPassword", "TxtNewPassword", "TxtConfirmNewPassword" trong XAML
            string oldPassword = TxtOldPassword.Password;
            string newPassword = TxtNewPassword.Password;
            string confirmPassword = TxtConfirmNewPassword.Password;

            var user = SessionManager.CurrentUser;

            // --- 1. Xác thực đầu vào cơ bản ---
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Vui lòng điền đầy đủ các trường mật khẩu.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu mới và xác nhận mật khẩu không khớp.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPassword.Length < 6)
            {
                MessageBox.Show("Mật khẩu mới phải có ít nhất 6 ký tự.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // --- 2. Xác thực mật khẩu cũ ---
            try
            {
                // Sử dụng logic đăng nhập/hash để so sánh mật khẩu cũ nhập vào
                string oldPasswordHash = _nhanVienBLL.HashPassword(oldPassword);

                if (!oldPasswordHash.Equals(user.PasswordHash, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Mật khẩu cũ không chính xác.", "Lỗi bảo mật", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // --- 3. Cập nhật mật khẩu mới ---
                if (_nhanVienBLL.DoiMatKhau(user.MaNV, newPassword))
                {
                    // Cập nhật thành công trong DB, cần cập nhật lại đối tượng CurrentUser trong Session
                    // Gọi lại hàm HashPassword để lấy Hash mới cho mật khẩu mới
                    user.PasswordHash = _nhanVienBLL.HashPassword(newPassword);

                    // Xóa dữ liệu trên form
                    TxtOldPassword.Clear();
                    TxtNewPassword.Clear();
                    TxtConfirmNewPassword.Clear();

                    MessageBox.Show("Đổi mật khẩu thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật mật khẩu. Vui lòng thử lại.", "Lỗi cập nhật", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (ArgumentException aex)
            {
                // Bắt lỗi từ BLL (như MaNV không hợp lệ,...)
                MessageBox.Show(aex.Message, "Lỗi nghiệp vụ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi hệ thống: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // ====== Bindable properties ======
        private string _gioMoCua, _gioDongCua;
        private int _khoangTgToiThieu, _diemTichLuyPer10000, _nguongDiemUuDai, _tiLeUuDai;
        private int _canhBaoNoShow, _soSanToiDa, _soSlotToiDa, _timeoutPhien, _nguongTonKhoThap;

        public string GioMoCua
        {
            get => _gioMoCua;
            set { _gioMoCua = value; OnPropertyChanged(); }
        }

        public string GioDongCua
        {
            get => _gioDongCua;
            set { _gioDongCua = value; OnPropertyChanged(); }
        }

        public int KhoangTGToiThieu
        {
            get => _khoangTgToiThieu;
            set { _khoangTgToiThieu = value; OnPropertyChanged(); }
        }

        public int DiemTichLuyPer10000
        {
            get => _diemTichLuyPer10000;
            set { _diemTichLuyPer10000 = value; OnPropertyChanged(); }
        }

        public int NguongDiemUuDai
        {
            get => _nguongDiemUuDai;
            set { _nguongDiemUuDai = value; OnPropertyChanged(); }
        }

        public int TiLeUuDai
        {
            get => _tiLeUuDai;
            set { _tiLeUuDai = value; OnPropertyChanged(); }
        }

        public int CanhBaoNoShow
        {
            get => _canhBaoNoShow;
            set { _canhBaoNoShow = value; OnPropertyChanged(); }
        }

        public int SoSanToiDa
        {
            get => _soSanToiDa;
            set { _soSanToiDa = value; OnPropertyChanged(); }
        }

        public int SoSlotToiDa
        {
            get => _soSlotToiDa;
            set { _soSlotToiDa = value; OnPropertyChanged(); }
        }

        public int TimeoutPhien
        {
            get => _timeoutPhien;
            set { _timeoutPhien = value; OnPropertyChanged(); }
        }

        public int NguongTonKhoThap
        {
            get => _nguongTonKhoThap;
            set { _nguongTonKhoThap = value; OnPropertyChanged(); }
        }

        private System.Collections.IEnumerable members;
        public System.Collections.IEnumerable Members
        {
            get => members;
            set => SetProperty(ref members, value);
        }

        // ====== Save command ======
        public ICommand CmdSave => new RelayCommand(_ =>
        {
            // TODO: map vào lớp cấu hình và lưu DB/appsettings
            MessageBox.Show("Đã lưu cấu hình!", "Cấu hình hệ thống",
                MessageBoxButton.OK, MessageBoxImage.Information);
        });

        // ====== EVENT: nút Thêm vai trò (tab Phân quyền) ======
        private void BtnThemVaiTro_Click(object sender, RoutedEventArgs e)
        {
            // Mở form thêm vai trò
            var win = new frmThemVaiTro();

            // Lấy window cha để đặt Owner (CenterOwner sẽ canh giữa)
            Window parent = Window.GetWindow(this);
            if (parent != null)
                win.Owner = parent;

            bool? result = win.ShowDialog();

            if (result == true)
            {
                // TODO: reload danh sách vai trò sau khi thêm
                // ví dụ:
                // if (DataContext is HeThongViewModel vm)
                // {
                //     vm.LoadDanhSachVaiTro();
                // }
            }
        }

        // (nếu bạn vẫn còn ToggleButton_Checked trong XAML thì để trống cũng không sao)
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
        }

        // ====== INotifyPropertyChanged ======
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string n = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

        protected bool SetProperty<T>(ref T field, T newValue,
                                      [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _run;
        private readonly Func<object, bool> _can;

        public RelayCommand(Action<object> run, Func<object, bool> can = null)
        {
            _run = run;
            _can = can;
        }

        public bool CanExecute(object p) => _can?.Invoke(p) ?? true;
        public void Execute(object p) => _run?.Invoke(p);
        public event EventHandler CanExecuteChanged { add { } remove { } }
    }
}
