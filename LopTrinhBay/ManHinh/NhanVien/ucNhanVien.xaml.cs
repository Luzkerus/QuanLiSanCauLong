using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.NhanVien
{
    /// <summary>
    /// Interaction logic for ucNhanVien.xaml
    /// </summary>
    public partial class ucNhanVien : UserControl, INotifyPropertyChanged
    {
        // ====== INotifyPropertyChanged ======
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // ====== Thuộc tính bind từ XAML ======
        public ObservableCollection<NhanVienItem> NhanViens { get; set; }
        public ICollectionView NhanViensView { get; set; }

        public ObservableCollection<CaLamItem> CaLamHomNay { get; set; }

        private string _tuKhoa;
        public string TuKhoa
        {
            get => _tuKhoa;
            set
            {
                if (_tuKhoa != value)
                {
                    _tuKhoa = value;
                    OnPropertyChanged(nameof(TuKhoa));
                    // filter danh sách khi nhập ô tìm kiếm
                    NhanViensView?.Refresh();
                }
            }
        }

        private int _tongNhanVien;
        public int TongNhanVien
        {
            get => _tongNhanVien;
            set { _tongNhanVien = value; OnPropertyChanged(nameof(TongNhanVien)); }
        }

        private int _soFullTime;
        public int SoFullTime
        {
            get => _soFullTime;
            set { _soFullTime = value; OnPropertyChanged(nameof(SoFullTime)); }
        }

        private int _soPartTime;
        public int SoPartTime
        {
            get => _soPartTime;
            set { _soPartTime = value; OnPropertyChanged(nameof(SoPartTime)); }
        }

        public ucNhanVien()
        {
            InitializeComponent();

            // Dùng chính UserControl làm DataContext
            this.DataContext = this;

            // Khởi tạo dữ liệu demo
            LoadDemoData();
        }

        // Nút "Thêm nhân viên" trên header
        private void BtnThemNhanVien_Click(object sender, RoutedEventArgs e)
        {
            var win = new frmThemNhanVien();

            Window parent = Window.GetWindow(this);
            if (parent != null)
            {
                win.Owner = parent;
            }

            bool? result = win.ShowDialog();

            if (result == true)
            {
                // Sau khi thêm xong thì reload lại danh sách nhân viên (sau này nối với DB)
                ReloadNhanVien();
            }
        }

        // Tạm thời demo: chỉ tính lại KPI từ list hiện tại
        private void ReloadNhanVien()
        {
            TinhKPI();
            NhanViensView?.Refresh();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Nếu cần xử lý chọn dòng thì code thêm ở đây
        }

        // ====== Tạo dữ liệu demo để chạy thử ======
        private void LoadDemoData()
        {
            NhanViens = new ObservableCollection<NhanVienItem>
            {
                new NhanVienItem
                {
                    HoTen = "Nguyễn Văn A",
                    Note = "Quản lý",
                    SoDienThoai = "0909123456",
                    Email = "nva@cauchicuuchi.vn",
                    VaiTroText = "Quản lý",
                    NgayVaoLam = new DateTime(2023, 5, 10),
                    TrangThaiText = "Đang làm",
                    Username = "nva.manager",
                    Password = "Pass@123"
                },
                new NhanVienItem
                {
                    HoTen = "Trần Thị B",
                    Note = "Thu ngân",
                    SoDienThoai = "0912345678",
                    Email = "ttb@cauchicuuchi.vn",
                    VaiTroText = "Full-time",
                    NgayVaoLam = new DateTime(2024, 1, 3),
                    TrangThaiText = "Đang làm",
                    Username = "tranthib",
                    Password = "B123456!"
                },
                new NhanVienItem
                {
                    HoTen = "Lê Minh C",
                    Note = "Nhân viên part-time",
                    SoDienThoai = "0987123456",
                    Email = "lmc@cauchicuuchi.vn",
                    VaiTroText = "Part-time",
                    NgayVaoLam = new DateTime(2024, 9, 1),
                    TrangThaiText = "Nghỉ phép",
                    Username = "leminhc",
                    Password = "C@pt2024"
                },
                new NhanVienItem
                {
                    HoTen = "Phạm Thu D",
                    Note = "Lễ tân",
                    SoDienThoai = "0938456123",
                    Email = "ptd@cauchicuuchi.vn",
                    VaiTroText = "Full-time",
                    NgayVaoLam = new DateTime(2023, 11, 20),
                    TrangThaiText = "Đang làm",
                    Username = "phamthud",
                    Password = "ThuD@2023"
                }
            };

            // View để bind vào DataGrid + filter theo Từ khóa
            NhanViensView = CollectionViewSource.GetDefaultView(NhanViens);
            NhanViensView.Filter = NhanVienFilter;

            // Dữ liệu ca làm việc hôm nay (demo)
            CaLamHomNay = new ObservableCollection<CaLamItem>
            {
                new CaLamItem
                {
                    HoTen = "Nguyễn Văn A",
                    Ngay = DateTime.Today,
                    Ca = "Ca tối (18:00 - 23:00)",
                    CheckIn = new TimeSpan(17, 55, 0),
                    CheckOut = new TimeSpan(23, 5, 0),
                    TrangThai = "Đã check-out"
                },
                new CaLamItem
                {
                    HoTen = "Trần Thị B",
                    Ngay = DateTime.Today,
                    Ca = "Ca chiều (13:00 - 18:00)",
                    CheckIn = new TimeSpan(12, 50, 0),
                    CheckOut = new TimeSpan(18, 2, 0),
                    TrangThai = "Đã check-out"
                },
                new CaLamItem
                {
                    HoTen = "Lê Minh C",
                    Ngay = DateTime.Today,
                    Ca = "Ca tối (18:00 - 23:00)",
                    CheckIn = new TimeSpan(18, 5, 0),
                    CheckOut = new TimeSpan(0, 0, 0),
                    TrangThai = "Đang làm"
                }
            };

            // Tính KPI
            TinhKPI();

            // Notify cho XAML
            OnPropertyChanged(nameof(NhanViens));
            OnPropertyChanged(nameof(NhanViensView));
            OnPropertyChanged(nameof(CaLamHomNay));
        }

        private bool NhanVienFilter(object obj)
        {
            var nv = obj as NhanVienItem;
            if (nv == null) return false;

            if (string.IsNullOrWhiteSpace(TuKhoa)) return true;

            var keyword = TuKhoa.Trim().ToLower();

            return (nv.HoTen != null && nv.HoTen.ToLower().Contains(keyword))
                   || (nv.SoDienThoai != null && nv.SoDienThoai.ToLower().Contains(keyword))
                   || (nv.Email != null && nv.Email.ToLower().Contains(keyword))
                   || (nv.Username != null && nv.Username.ToLower().Contains(keyword));
        }

        private void TinhKPI()
        {
            TongNhanVien = NhanViens?.Count ?? 0;
            SoFullTime = NhanViens?.Count(n => n.VaiTroText == "Full-time") ?? 0;
            SoPartTime = NhanViens?.Count(n => n.VaiTroText == "Part-time") ?? 0;
        }
    }

    // ====== Model cho nhân viên (phù hợp với XAML hiện tại) ======
    public class NhanVienItem
    {
        public string HoTen { get; set; }
        public string Note { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string VaiTroText { get; set; }
        public DateTime NgayVaoLam { get; set; }
        public string TrangThaiText { get; set; }

        // Thêm Username / Password để bind lên DataGrid
        public string Username { get; set; }
        public string Password { get; set; }

        // Dùng cho AvatarTemplate
        public string Initials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(HoTen)) return "?";
                var parts = HoTen.Trim().Split(' ');
                if (parts.Length == 1)
                    return parts[0].Substring(0, 1).ToUpper();

                var last = parts[parts.Length - 1];
                return (parts[0].Substring(0, 1) + last.Substring(0, 1)).ToUpper();
            }
        }
    }

    // ====== Model cho ca làm việc hôm nay ======
    public class CaLamItem
    {
        public string HoTen { get; set; }
        public DateTime Ngay { get; set; }
        public string Ca { get; set; }
        public TimeSpan CheckIn { get; set; }
        public TimeSpan CheckOut { get; set; }
        public string TrangThai { get; set; }
    }
}
