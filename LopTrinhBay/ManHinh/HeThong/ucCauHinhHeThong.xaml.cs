using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopNghiepVu;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.HeThong
{
    public partial class ucCauHinhHeThong : UserControl, INotifyPropertyChanged
    {
        private readonly CauHinhHeThongBLL _cauHinhBLL = new CauHinhHeThongBLL();
        private readonly NhanVienBLL _nhanVienBLL = new NhanVienBLL();
        public ucCauHinhHeThong()
        {
            InitializeComponent();
            DataContext = this;

            // Demo default (đặt theo ảnh)
           
        }
      

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
        private void LoadCauHinhVaoUI()
        {
            // Giả định rằng DataContext của tab này là chính UserControl/Window
            // và các thuộc tính CanhBaoNoShow, SoSanToiDa,... tồn tại trong DataContext này.
            if (this.DataContext == null) return;

            try
            {
                var config = _cauHinhBLL.LayCauHinhHeThong();

                // Ánh xạ dữ liệu vào Properties đã được Binding trong XAML

                // Lấy DataContext để gán (thường là ViewModel hoặc chính UserControl/Window)
                dynamic dataContext = this.DataContext;

                // Đặt sân & Cảnh báo
                dataContext.CanhBaoNoShow = config.CanhBaoNoShow;
                dataContext.SoSanToiDa = config.SoSanToiDa;
                dataContext.SoSlotToiDa = config.SoSlotToiDa;

                // Bảo mật & Kho
                dataContext.TimeoutPhien = config.TimeoutPhien;
                dataContext.NguongTonKhoThap = config.NguongTonKhoThap;

                // Optional: Thông báo tải thành công (có thể bỏ qua)
                // Console.WriteLine("Tải cấu hình thành công.");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải cấu hình hệ thống: {ex.Message}", "Lỗi Tải Dữ Liệu", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Đảm bảo sự kiện chỉ xử lý khi TabItem được chọn (AddedItems)
            if (e.Source is TabControl tabControl && e.AddedItems.Count > 0)
            {
                TabItem selectedTab = e.AddedItems[0] as TabItem;

                // Kiểm tra nếu Tab được chọn là Tab Cấu hình hệ thống (sử dụng Name đã đặt)
                if (selectedTab != null && selectedTab.Name == "TabCauHinhHeThong")
                {
                    // 1. KIỂM TRA QUYỀN TRUY CẬP ADMIN
                    // Giả định: SessionManager.CurrentUser.VaiTro là chuỗi hoặc enum VaiTro
                    // Giả định: VaiTro là Admin được định nghĩa ở đâu đó
                    const string ADMIN_ROLE_NAME = "Admin";

                    // Thay thế SessionManager bằng cách bạn quản lý phiên đăng nhập
                    if (SessionManager.IsLoggedIn && SessionManager.CurrentUser.VaiTro == ADMIN_ROLE_NAME)
                    {
                        // Nếu là Admin, tiến hành tải dữ liệu cấu hình
                        LoadCauHinhVaoUI();
                    }
                    else
                    {
                        // Nếu không phải Admin:
                        MessageBox.Show("Bạn không có quyền truy cập vào Cấu hình Hệ thống.", "Từ chối truy cập", MessageBoxButton.OK, MessageBoxImage.Stop);

                        // Chuyển người dùng về tab đầu tiên (hoặc tab mặc định)
                        // Giả định TabControl của bạn có một tab mặc định (ví dụ: tab đầu tiên)
                        if (tabControl.Items.Count > 0)
                        {
                            tabControl.SelectedIndex = 0;
                        }

                        // Dừng xử lý
                        e.Handled = true;
                    }
                }
            }
        }




        private int _canhBaoNoShow, _soSanToiDa, _soSlotToiDa, _timeoutPhien, _nguongTonKhoThap;




        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            // Dùng SetProperty để thông báo thay đổi cho UI
            set => SetProperty(ref _isAdmin, value);
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

        // ====== EVENT: nút Thêm vai trò (tab Phân quyền) ======
        public ICommand CmdSave => new RelayCommand(_ =>
        {
            try
            {
                // 1. Map từ Properties sang DTO
                CauHinhHeThong configToSave = new CauHinhHeThong
                {
                    CanhBaoNoShow = this.CanhBaoNoShow,
                    SoSanToiDa = this.SoSanToiDa,
                    SoSlotToiDa = this.SoSlotToiDa,
                    TimeoutPhien = this.TimeoutPhien,
                    NguongTonKhoThap = this.NguongTonKhoThap,
                    // Bỏ các thuộc tính đã loại khỏi DTO/DAL (GioMoCua, DiemTichLuyPer10000,...)
                };

                // 2. Gọi BLL để lưu và xác thực
                if (_cauHinhBLL.LuuCauHinhHeThong(configToSave))
                {
                    MessageBox.Show("Đã lưu cấu hình hệ thống thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Lỗi xảy ra trong DAL nhưng không ném exception (ví dụ: mất kết nối nhưng không bắt được)
                    MessageBox.Show("Lưu cấu hình thất bại, không có dữ liệu nào được cập nhật.", "Lỗi Lưu Trữ", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (ArgumentException aex)
            {
                // Lỗi xác thực nghiệp vụ từ BLL
                MessageBox.Show($"Lỗi nghiệp vụ: {aex.Message}", "Lỗi Cấu Hình", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                // Lỗi hệ thống/DAL
                MessageBox.Show($"Lỗi hệ thống khi lưu cấu hình: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });
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
