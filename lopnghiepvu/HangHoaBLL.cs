using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopTruyCapDuLieu;

namespace QuanLiSanCauLong.LopNghiepVu
{
    public class HangHoaBLL
    {
        private readonly HangHoaDAL hangHoaDAL;

        public HangHoaBLL()
        {
            hangHoaDAL = new HangHoaDAL();
        }

        public bool KiemTraTonTai(string maHang)
        {
            return hangHoaDAL.KiemTraTonTai(maHang);
        }

        public void ThemHangMoi(ChiTietPhieuNhap ct)
        {
            // Nếu chưa có MaHang → tạo tự động
            if (string.IsNullOrEmpty(ct.MaHang))
            {
                ct.MaHang = TaoMaHang(ct.DVT);
            }

            hangHoaDAL.ThemHangMoi(ct);
        }

        public void CapNhatTonKho(ChiTietPhieuNhap ct)
        {
            hangHoaDAL.CapNhatTonKho(ct);
        }

        public string TaoMaHang(string dvt)
        {
            // Lấy prefix theo ĐVT
            string prefix;
            if (dvt == "Chai")
                prefix = "CH";
            else if (dvt == "Lon")
                prefix = "LN";
            else if (dvt == "Hộp")
                prefix = "HP";
            else
                prefix = "HH";

            // Lấy mã lớn nhất trong DB
            string lastMa = hangHoaDAL.LayMaHangLonNhat(prefix);

            int stt = 1;
            if (!string.IsNullOrEmpty(lastMa))
            {
                // lastMa dạng: CH-005
                string[] parts = lastMa.Split('-');
                if (parts.Length > 1 && int.TryParse(parts[1], out int lastNum))
                {
                    stt = lastNum + 1;
                }
            }

            return $"{prefix}-{stt:000}";
        }
        public string LayMaHangByTenVaDVT(string tenHang, string dvt)
        {
            return hangHoaDAL.LayMaHangByTenVaDVT(tenHang, dvt);
        }
        public List<HangHoa> LayTatCaHangHoa()
        {
            return hangHoaDAL.LayTatCaHangHoa();
        }


    }

}
