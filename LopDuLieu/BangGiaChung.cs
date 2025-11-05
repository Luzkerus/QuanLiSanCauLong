using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiSanCauLong.LopDuLieu
{
    public class BangGiaChung
    {
        public int MaBangGia { get; set; }
        public TimeSpan GioBatDau { get; set; }
        public TimeSpan GioKetThuc { get; set; }
        public decimal DonGia { get; set; }
        public string LoaiNgay { get; set; } // "Thứ 2-Thứ 6" hoặc "Cuối Tuần"
        public decimal? PhuThuLePercent { get; set; }
    }

}
