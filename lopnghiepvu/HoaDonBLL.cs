using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopTruyCapDuLieu;

namespace QuanLiSanCauLong.LopNghiepVu
{
    public class HoaDonBLL
    {
        private readonly HoaDonDAL hoaDonDAL;
        private readonly ChiTietHoaDonDAL chiTietHoaDonDAL;
        public HoaDonBLL()
        {
            hoaDonDAL = new HoaDonDAL();
            chiTietHoaDonDAL = new ChiTietHoaDonDAL();
        }

        public string TaoSoHDN()
        {
            string prefix = "HDN";
            string today = DateTime.Now.ToString("yyyyMMdd");

            // Lấy danh sách các hóa đơn hôm nay để đếm
            var hoaDonsHomNay = hoaDonDAL.LayTatCaHoaDon()
                                        .Where(hd => hd.Ngay.Date == DateTime.Now.Date)
                                        .ToList();

            int stt = hoaDonsHomNay.Count + 1; // stt trong ngày
            string soHDN = $"{prefix}{today}{stt:D3}"; // D3 = 3 chữ số, có 0 ở đầu nếu <100

            return soHDN;
        }

        public void ThemHoaDon(HoaDon hd, List<ChiTietHoaDon> chiTietHoaDons)
        {
            // Tạo số hóa đơn nếu chưa có
            if (string.IsNullOrEmpty(hd.SoHDN))
            {
                hd.SoHDN = TaoSoHDN();
            }

            // Gán MaChiTiet cho từng chi tiết
            int index = 1;
            foreach (var cthd in chiTietHoaDons)
            {
                cthd.SoHDN = hd.SoHDN;
                cthd.MaChiTiet = $"{hd.SoHDN}{index:D2}"; // D2 = 2 chữ số
                index++;
            }

            // Lưu dữ liệu
            hoaDonDAL.ThemHoaDon(hd);
            foreach (var cthd in chiTietHoaDons)
            {
                chiTietHoaDonDAL.ThemChiTietHoaDon(cthd);
            }
        }
        public void CapNhatTonKhoSauKhiBan(List<ChiTietHoaDon> chiTietHoaDons)
        {
            foreach (var cthd in chiTietHoaDons)
            {
                var hangHoaDAL = new HangHoaDAL();
                hangHoaDAL.CapNhatTonKhoSauKhiBan(cthd.MaHang, cthd.SoLuong);
            }
        }
    }
}
