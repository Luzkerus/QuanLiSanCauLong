using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace QuanLiSanCauLong
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            bool isNew;
            _mutex = new Mutex(true, "QuanLiSanCauLong_SingleInstance", out isNew);
            if (!isNew)
            {
                MessageBox.Show("Ứng dụng đang chạy.", "Thông báo");
                Shutdown();
                return;
            }
            base.OnStartup(e);
        }
    }

}
