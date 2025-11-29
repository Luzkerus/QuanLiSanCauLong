using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using QuanLiSanCauLong.LopNghiepVu;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.HeThong;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.Controls
{
    public partial class ucSidebar : UserControl
    {
        private const double ExpandedWidth = 220;   // sidebar lớn
        private const double CollapsedWidth = 72;   // sidebar nhỏ (chỉ icon)

        public ucSidebar()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                // Set lại width theo trạng thái hiện tại
                UpdateWidth();
                ApplyActive(SelectedKey); // set active theo SelectedKey khi load
                LoadCurrentUser();
            };
        }
        private void LoadCurrentUser()
        {
            // Kiểm tra SessionManager để lấy thông tin đã lưu sau khi đăng nhập
            if (SessionManager.IsLoggedIn)
            {
                var user = SessionManager.CurrentUser;

                // Giả định ucSidebar có x:Name="sidebarControl"
                AdminName = user.TenNV;
                AdminRole = user.VaiTro;
            }
        }
        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            // THÊM: Hộp thoại xác nhận trước khi đăng xuất
            MessageBoxResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất khỏi hệ thống không?", // Nội dung thông báo
                "Xác nhận Đăng xuất", // Tiêu đề hộp thoại
                MessageBoxButton.YesNo, // Các nút hiển thị (Có, Không)
                MessageBoxImage.Question // Biểu tượng câu hỏi
            );

            // Kiểm tra kết quả
            if (result == MessageBoxResult.Yes)
            {
                // 1. Thực hiện thao tác đăng xuất (xóa phiên làm việc)
                SessionManager.Logout();

                // 2. Chuyển người dùng về màn hình đăng nhập (sử dụng ShowDialog)

                // Tìm cửa sổ cha (MainWindow)
                Window parentWindow = Window.GetWindow(this);

                // Giả định MainWindow là cửa sổ chính
                if (parentWindow is MainWindow mainWindow)
                {
                    // Mở form đăng nhập mới dưới dạng Dialog (blocking)
                    // Bạn cần đảm bảo frmDangNhap là một Window, không phải UserControl
                    frmDangNhap frmDangNhap = new frmDangNhap();
                    frmDangNhap.Show(); // Nên dùng Show() thay vì ShowDialog() ở đây

                    // Đóng cửa sổ chính sau khi đã mở màn hình đăng nhập
                    // Đặt lệnh Close() sau lệnh Show() hoặc ShowDialog()
                    mainWindow.Close();
                }
            }
            // Nếu người dùng chọn No, không làm gì cả và giữ nguyên màn hình.
        }
        // ================= TRẠNG THÁI THU GỌN/MỞ RỘNG =================

        /// <summary>
        /// true  = sidebar lớn (icon + chữ)
        /// false = sidebar nhỏ (chỉ icon)
        /// </summary>
        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(
                nameof(IsExpanded),
                typeof(bool),
                typeof(ucSidebar),
                new PropertyMetadata(true, OnIsExpandedChanged));

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = (ucSidebar)d;
            uc.UpdateWidth();
        }

        /// <summary>
        /// Cập nhật Width của UserControl theo IsExpanded
        /// </summary>
        private void UpdateWidth()
        {
            this.Width = IsExpanded ? ExpandedWidth : CollapsedWidth;
        }

        /// <summary>
        /// Sự kiện click nút thu gọn / mở rộng (gắn trong XAML: Click="BtnToggle_Click")
        /// </summary>
        private void BtnToggle_Click(object sender, RoutedEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }

        // ====== DP: SelectedKey (overview / courts / booking / customers / payment / pos / staff / reports / settings) ======
        public string SelectedKey
        {
            get => (string)GetValue(SelectedKeyProperty);
            set => SetValue(SelectedKeyProperty, value);
        }
        public static readonly DependencyProperty SelectedKeyProperty =
            DependencyProperty.Register(nameof(SelectedKey), typeof(string), typeof(ucSidebar),
                new PropertyMetadata("overview", OnSelectedKeyChanged));

        private static void OnSelectedKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = (ucSidebar)d;
            uc.ApplyActive(e.NewValue as string);
        }

        // ====== DP: AdminName / AdminRole ======
        public string AdminName
        {
            get => (string)GetValue(AdminNameProperty);
            set => SetValue(AdminNameProperty, value);
        }
        public static readonly DependencyProperty AdminNameProperty =
            DependencyProperty.Register(nameof(AdminName), typeof(string), typeof(ucSidebar),
                new PropertyMetadata());

        public string AdminRole
        {
            get => (string)GetValue(AdminRoleProperty);
            set => SetValue(AdminRoleProperty, value);
        }
        public static readonly DependencyProperty AdminRoleProperty =
            DependencyProperty.Register(nameof(AdminRole), typeof(string), typeof(ucSidebar),
                new PropertyMetadata("Quản lý"));

        // ====== Event điều hướng ======
        public event EventHandler<string> NavigateRequested;

        // Handler chung cho tất cả menu item
        private void MenuItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border b && b.Tag is string key)
            {
                SelectedKey = key; // sẽ gọi ApplyActive qua OnSelectedKeyChanged
                NavigateRequested?.Invoke(this, key);
            }
        }

        // Đổi style active/normal theo SelectedKey
        private void ApplyActive(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) key = "overview";

            // Lấy style từ resource
            var activeStyle = TryFindResource("MenuItemActive") as Style;
            var normalStyle = TryFindResource("MenuItemNormal") as Style;

            // Tìm tất cả Border menu trong StackPanel row=1
            var root = this.Content as Border;
            if (root?.Child is Grid grid && VisualTreeHelper.GetChildrenCount(grid) >= 2)
            {
                var menuPanel = grid.Children
                    .OfType<StackPanel>()
                    .FirstOrDefault(sp => Grid.GetRow(sp) == 1);

                if (menuPanel != null)
                {
                    foreach (var child in menuPanel.Children.OfType<Border>())
                    {
                        var isActive = (child.Tag as string)?.Equals(key, StringComparison.OrdinalIgnoreCase) == true;
                        child.Style = isActive ? activeStyle : normalStyle;

                        // Đổi màu chữ icon + label khi active để đảm bảo đọc tốt
                        var stack = child.Child as StackPanel;
                        if (stack != null)
                        {
                            var icon = stack.Children.OfType<TextBlock>().FirstOrDefault();
                            var label = stack.Children.OfType<TextBlock>().Skip(1).FirstOrDefault();

                            if (isActive)
                            {
                                if (icon != null) icon.Foreground = new SolidColorBrush(Colors.White);
                                if (label != null)
                                {
                                    label.Foreground = new SolidColorBrush(Colors.White);
                                    label.FontWeight = FontWeights.SemiBold;
                                }
                            }
                            else
                            {
                                if (icon != null) icon.ClearValue(TextBlock.ForegroundProperty);
                                if (label != null)
                                {
                                    // về lại màu chuẩn
                                    label.Foreground = TryFindResource("ColText") as Brush ?? Brushes.Black;
                                    label.FontWeight = FontWeights.Normal;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
