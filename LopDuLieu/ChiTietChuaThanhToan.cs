using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiSanCauLong.LopDuLieu
{
    public class ChiTietChuaThanhToan
    {
        public string MaPhieu { get; set; }
        public string TenSanCached { get; set; }
        public DateTime NgayDat { get; set; }
        public TimeSpan GioBatDau { get; set; }
        public TimeSpan GioKetThuc { get; set; }
        public decimal ThanhTien { get; set; }
        public bool IsChecked { get; set; } = false;
    }

}
