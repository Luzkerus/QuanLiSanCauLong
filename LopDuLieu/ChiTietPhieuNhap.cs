using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiSanCauLong.LopDuLieu
{
    public class ChiTietPhieuNhap
    {
        public string MaChiTiet { get; set; }   
        public string MaHang { get; set; }
        public string TenHang { get; set; }
        
        public string DVT { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaNhap { get; set; }
        public decimal ChietKhau { get; set; }
        public decimal ChietKhauTien
        {
            get
            {
                return SoLuong * GiaNhap * (ChietKhau / 100);
            }
        }
        public decimal VAT { get; set; } = 5;
        public string SoLo { get; set; }
        public DateTime HSD { get; set; }

        public decimal ThanhTien
        {
            get
            {
                decimal tienTruocThue = SoLuong * GiaNhap * (1 - ChietKhau / 100);
                decimal tienVAT = tienTruocThue * (VAT / 100);
                return tienTruocThue + tienVAT;
            }
        }
        public string SoPhieu { get; set; }
    }
}
