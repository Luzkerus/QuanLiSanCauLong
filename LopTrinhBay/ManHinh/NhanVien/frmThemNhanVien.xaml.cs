using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace QuanLiSanCauLong.LopTrinhBay.ManHinh.NhanVien
{
    public partial class frmThemNhanVien : Window, INotifyPropertyChanged
    {
        // ===== INotifyPropertyChanged =====
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // ===== Static counter giả lập tăng mã theo loại NV =====
        // Sau này bạn có thể thay bằng query DB để lấy số lớn nhất
        private static readonly Dictionary<string, int> _counterByPrefix = new Dictionary<string, int>();

        // ===== Các property bind với XAML =====
        private string _hoTen;
        public string HoTen
        {
            get => _hoTen;
            set { if (_hoTen != value) { _hoTen = value; OnPropertyChanged(); } }
        }

        private string _soDienThoai;
        public string SoDienThoai
        {
            get => _soDienThoai;
            set { if (_soDienThoai != value) { _soDienThoai = value; OnPropertyChanged(); } }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set { if (_email != value) { _email = value; OnPropertyChanged(); } }
        }

        private string _note;
        public string Note
        {
            get => _note;
            set { if (_note != value) { _note = value; OnPropertyChanged(); } }
        }

        private string _vaiTro;
        /// <summary>
        /// Loại nhân viên: "Quản lý" / "Full-time" / "Part-time"
        /// </summary>
        public string VaiTro
        {
            get => _vaiTro;
            set
            {
                if (_vaiTro != value)
                {
                    _vaiTro = value;
                    OnPropertyChanged();
                    GenerateMaNhanVien();   // mỗi lần đổi loại NV thì tự sinh mã
                }
            }
        }

        private DateTime? _ngayVaoLam = DateTime.Today;
        public DateTime? NgayVaoLam
        {
            get => _ngayVaoLam;
            set { if (_ngayVaoLam != value) { _ngayVaoLam = value; OnPropertyChanged(); } }
        }

        private string _trangThai = "Đang làm";
        public string TrangThai
        {
            get => _trangThai;
            set { if (_trangThai != value) { _trangThai = value; OnPropertyChanged(); } }
        }

        private string _maNhanVien;
        public string MaNhanVien
        {
            get => _maNhanVien;
            set { if (_maNhanVien != value) { _maNhanVien = value; OnPropertyChanged(); } }
        }

        // ===== Constructor =====
        public frmThemNhanVien()
        {
            InitializeComponent();

            // dùng luôn chính window làm DataContext
            DataContext = this;

            // Giá trị mặc định
            NgayVaoLam = DateTime.Today;
            TrangThai = "Đang làm";
            // Nếu muốn default loại là Full-time:
            // VaiTro = "Full-time";  // sẽ tự sinh mã luôn
        }

        // ===== Sinh mã nhân viên theo loại =====
        private void GenerateMaNhanVien()
        {
            if (string.IsNullOrWhiteSpace(VaiTro))
                return;

            // Map loại nhân viên -> prefix (dùng switch thường để hợp C# 7.3)
            string prefix;
            switch (VaiTro)
            {
                case "Quản lý":
                    prefix = "QL";
                    break;
                case "Full-time":
                    prefix = "FT";
                    break;
                case "Part-time":
                    prefix = "PT";
                    break;
                default:
                    prefix = "NV";
                    break;
            }

            // Lấy số hiện tại trong dictionary
            if (!_counterByPrefix.TryGetValue(prefix, out int current))
            {
                current = 0;
            }

            current++;
            _counterByPrefix[prefix] = current;

            // Format: QL0001, FT0005, PT0010...
            MaNhanVien = string.Format("{0}{1:0000}", prefix, current);
        }


        // ===== Event UI =====
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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // TODO: validate & lưu nhân viên vào DB
            // Ví dụ: kiểm tra HoTen, SoDienThoai không được trống

            // Nếu chưa có mã (trường hợp người dùng chưa chọn loại nhân viên),
            // có thể tự sinh mã ở đây lần cuối:
            if (string.IsNullOrWhiteSpace(MaNhanVien))
            {
                GenerateMaNhanVien();
            }

            // TODO: gọi repository / service để Insert NhanVien:
            // var nv = new NhanVienEntity {
            //     MaNhanVien = this.MaNhanVien,
            //     HoTen = this.HoTen,
            //     SoDienThoai = this.SoDienThoai,
            //     Email = this.Email,
            //     Note = this.Note,
            //     VaiTro = this.VaiTro,
            //     NgayVaoLam = this.NgayVaoLam,
            //     TrangThai = this.TrangThai
            // };
            // _nhanVienService.Insert(nv);

            DialogResult = true;
            Close();
        }
    }
}
