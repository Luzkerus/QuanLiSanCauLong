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


            hangHoaDAL.ThemHangMoi(ct);
        }

        public void CapNhatTonKho(ChiTietPhieuNhap ct)
        {
            hangHoaDAL.CapNhatTonKho(ct);
        }

        public string TaoMaHang(string dvt, List<string> maHangDaTonTai)
        {
            string prefix;

            switch (dvt)
            {
                case "Chai": prefix = "CH"; break;
                case "Lon": prefix = "LN"; break;
                case "Hộp": prefix = "HP"; break;
                default: prefix = "HH"; break;
            }

            // 1. Lấy mã lớn nhất từ DB
            string lastMaDB = hangHoaDAL.LayMaHangLonNhat(prefix);
            int stt = 1;

            if (!string.IsNullOrEmpty(lastMaDB))
            {
                var parts = lastMaDB.Split('-');
                if (parts.Length > 1 && int.TryParse(parts[1], out int lastNum))
                    stt = lastNum + 1;
            }

            // 2. Kiểm tra mã trong DataGrid (chưa lưu DB)
            foreach (string ma in maHangDaTonTai)
            {
                if (ma.StartsWith(prefix))
                {
                    var parts = ma.Split('-');
                    if (parts.Length > 1 && int.TryParse(parts[1], out int num))
                        if (num >= stt) stt = num + 1;
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

        public int TinhTongSoHangHoa()
        {
            return LayTatCaHangHoa().Count;
        }
        public decimal TinhGiaTriTonKho()
        {
            var allHangHoa = LayTatCaHangHoa();
            decimal tongGiaTri = 0;
            foreach (var hh in allHangHoa)
            {
                tongGiaTri += hh.GiaNhap * hh.TonKho;
            }
            return tongGiaTri;
        }
        
    }

}
