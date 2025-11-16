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
