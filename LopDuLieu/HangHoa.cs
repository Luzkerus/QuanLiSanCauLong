using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiSanCauLong.LopDuLieu
{
    public class HangHoa
    {
        public string MaHang { get; set; }
        public string TenHang { get; set; }

        public string DVT { get; set; }
        public int TonKho { get; set; }
        public decimal GiaNhap { get; set; }
        public decimal GiaBan { get; set; } 
        public DateTime LanCuoiNhap { get; set; }
        public string TrangThai { get; set; }
    }
}
