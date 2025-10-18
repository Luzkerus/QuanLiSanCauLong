using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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


namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.KhachHoiVien
{
    public partial class frmKhachHoiVien : Window, INotifyPropertyChanged
    {
        public ObservableCollection<CustomerVm> Customers { get; } = new();
        public ICollectionView CustomersView { get; private set; }

        // KPI
        private int _kpiTongKhach;
        public int KpiTongKhach { get => _kpiTongKhach; set { _kpiTongKhach = value; OnPropertyChanged(); } }
        private int _kpiHoiVien;
        public int KpiHoiVien { get => _kpiHoiVien; set { _kpiHoiVien = value; OnPropertyChanged(); } }
        private int _kpiTongDiem;
        public int KpiTongDiem { get => _kpiTongDiem; set { _kpiTongDiem = value; OnPropertyChanged(); } }
        private int _kpiMoiThang;
        public int KpiMoiThang { get => _kpiMoiThang; set { _kpiMoiThang = value; OnPropertyChanged(); } }

        // Search & Filters
        private string _searchText;
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); CustomersView.Refresh(); } }

        private bool _filterAll = true;
        public bool FilterAll { get => _filterAll; set { _filterAll = value; if (value) { FilterHoiVien = false; FilterVangLai = false; } OnPropertyChanged(); CustomersView.Refresh(); } }

        private bool _filterHoiVien;
        public bool FilterHoiVien { get => _filterHoiVien; set { _filterHoiVien = value; if (value) FilterAll = false; OnPropertyChanged(); CustomersView.Refresh(); } }

        private bool _filterVangLai;
        public bool FilterVangLai { get => _filterVangLai; set { _filterVangLai = value; if (value) FilterAll = false; OnPropertyChanged(); CustomersView.Refresh(); } }

        // Commands
        public ICommand CmdViewDetail { get; }
        public ICommand CmdOffer { get; }
        public ICommand CmdMore { get; }

        public frmKhachHoiVien()
        {
            InitializeComponent();
            DataContext = this;

            CmdViewDetail = new RelayCommand(o => ViewDetail(o as CustomerVm));
            CmdOffer = new RelayCommand(o => Offer(o as CustomerVm));
            CmdMore = new RelayCommand(o => More(o as CustomerVm));

            CustomersView = CollectionViewSource.GetDefaultView(Customers);
            CustomersView.Filter = FilterCustomer;

            LoadData();
        }

        private void LoadData()
        {
            // TODO: hook DAL → lấy từ bảng KhachHang, DiemTichLuy_Log, DatSan/HoaDon (tổng chi tiêu & lượt chơi)
            // Dữ liệu mock bám SRS: SDT = PK, LoaiKH {VangLai, HoiVien}
            Customers.Clear();
            Customers.Add(new CustomerVm("0901234567", "Nguyễn Văn A", "nguyenvana@gmail.com", "HoiVien", 2450, 28, 5600000, new DateTime(2024, 1, 15)));
            Customers.Add(new CustomerVm("0907654321", "Trần Thị B", "tran.b@example.com", "VangLai", 120, 7, 950000, new DateTime(2025, 10, 1)));
            Customers.Add(new CustomerVm("0912345678", "Lê Minh C", null, "HoiVien", 50, 3, 210000, new DateTime(2025, 5, 10)));
            Customers.Add(new CustomerVm("0933334444", "Khách D", null, "HoiVien", 1800, 17, 4200000, new DateTime(2023, 11, 2)));
            Customers.Add(new CustomerVm("0942221111", "Khách E", null, "VangLai", 0, 1, 100000, new DateTime(2025, 10, 10)));

            // KPI
            KpiTongKhach = Customers.Count;
            KpiHoiVien = Customers.Count(c => c.LoaiKH == "HoiVien");
            KpiTongDiem = Customers.Sum(c => c.DiemTichLuy);
            var monthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            KpiMoiThang = Customers.Count(c => c.NgayDangKy >= monthStart);
        }

        private bool FilterCustomer(object obj)
        {
            if (obj is not CustomerVm c) return false;

            // tab filter
            if (FilterHoiVien && c.LoaiKH != "HoiVien") return false;
            if (FilterVangLai && c.LoaiKH != "VangLai") return false;

            // search
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var s = SearchText.Trim().ToLowerInvariant();
                if (!(c.TenKH?.ToLowerInvariant().Contains(s) == true ||
                      c.SDT?.ToLowerInvariant().Contains(s) == true ||
                      c.Email?.ToLowerInvariant().Contains(s) == true))
                    return false;
            }
            return true;
        }

        private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO: mở dialog thêm khách (frmThemKhach). Ở đây demo thêm nhanh.
            var n = new CustomerVm("09" + new Random().Next(10000000, 99999999),
                                   "Khách mới", null, "VangLai", 0, 0, 0, DateTime.Now);
            Customers.Add(n);
            // cập nhật KPI
            KpiTongKhach = Customers.Count;
            KpiMoiThang += 1;
        }

        private void ViewDetail(CustomerVm c)
        {
            if (c == null) return;
            MessageBox.Show($"[Hồ sơ] {c.TenKH}\nSĐT: {c.SDT}\nĐiểm: {c.DiemTichLuy}\nLượt chơi: {c.LuotChoi}\nTổng chi tiêu: {c.TongChiTieu:N0}₫", "Chi tiết");
            // TODO: điều hướng sang frmChiTietKhach.xaml (tab Lịch sử chơi, POS, Điểm thưởng)
        }

        private void Offer(CustomerVm c)
        {
            if (c == null) return;
            // Quy tắc ưu đãi theo SRS: >=1000 điểm → gợi ý -5%
            if (c.DiemTichLuy >= 1000)
                MessageBox.Show($"Đề xuất: Tạo ưu đãi -5% cho {c.TenKH} (điểm {c.DiemTichLuy}).");
            else
                MessageBox.Show($"{c.TenKH} chưa đạt ngưỡng ưu đãi (điểm {c.DiemTichLuy}/1000).");
            // TODO: mở form tạo Voucher / gắn cờ ưu đãi
        }

        private void More(CustomerVm c)
        {
            if (c == null) return;
            MessageBox.Show("Hành động khác: Gửi Zalo OA, Gắn tag CRM, Xuất Excel lịch sử...", "Thao tác");
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion
    }

    public class CustomerVm : INotifyPropertyChanged
    {
        public string SDT { get; }
        public string TenKH { get; }
        public string Email { get; }
        public string LoaiKH { get; } // "HoiVien" / "VangLai"
        public int DiemTichLuy { get; }
        public int LuotChoi { get; }
        public decimal TongChiTieu { get; }
        public DateTime NgayDangKy { get; }

        public string Initials => !(string.IsNullOrWhiteSpace(TenKH))
            ? string.Join("", TenKH.Split(' ').Where(p => p.Length > 0).TakeLast(2).Select(p => p[0])).ToUpper()
            : "KH";

        // Badge colors
        public Brush BadgeBg => LoaiKH == "HoiVien" ? (Brush)new SolidColorBrush(Color.FromRgb(232, 255, 245)) : new SolidColorBrush(Color.FromRgb(245, 247, 250));
        public Brush BadgeBorder => LoaiKH == "HoiVien" ? (Brush)new SolidColorBrush(Color.FromRgb(15, 191, 132)) : new SolidColorBrush(Color.FromRgb(210, 220, 230));
        public Brush BadgeFg => LoaiKH == "HoiVien" ? Brushes.DarkGreen : Brushes.SlateGray;

        public CustomerVm(string sdt, string ten, string email, string loai, int diem, int luot, decimal chi, DateTime ngay)
        {
            SDT = sdt; TenKH = ten; Email = email; LoaiKH = loai; DiemTichLuy = diem; LuotChoi = luot; TongChiTieu = chi; NgayDangKy = ngay;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string n = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _exec;
        private readonly Predicate<object> _can;
        public RelayCommand(Action<object> exec, Predicate<object> can = null) { _exec = exec; _can = can; }
        public bool CanExecute(object parameter) => _can?.Invoke(parameter) ?? true;
        public void Execute(object parameter) => _exec(parameter);
        public event EventHandler CanExecuteChanged { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }
    }
}

