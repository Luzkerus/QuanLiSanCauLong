using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.Controls
{
    public partial class ucSidebar : UserControl
    {
        public ucSidebar()
        {
            InitializeComponent();
            Loaded += (s, e) => ApplyActive(SelectedKey); // set active theo SelectedKey khi load
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
            DependencyProperty.Register(nameof(AdminName), typeof(string), typeof(ucSidebar), new PropertyMetadata("Admin User"));

        public string AdminRole
        {
            get => (string)GetValue(AdminRoleProperty);
            set => SetValue(AdminRoleProperty, value);
        }
        public static readonly DependencyProperty AdminRoleProperty =
            DependencyProperty.Register(nameof(AdminRole), typeof(string), typeof(ucSidebar), new PropertyMetadata("Quản lý"));

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
                                if (label != null) { label.Foreground = new SolidColorBrush(Colors.White); label.FontWeight = FontWeights.SemiBold; }
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
