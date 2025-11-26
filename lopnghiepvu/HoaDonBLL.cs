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
        public List<HoaDon> LayTatCaHoaDon()
        {
            return hoaDonDAL.LayTatCaHoaDon();
        }
        public List<ChiTietHoaDon> LayChiTietHoaDonTheoSoHDN(string soHDN)
        {
            return chiTietHoaDonDAL.LayChiTietHoaDonTheoSoHDN(soHDN);
        }
        public List<ChiTietHoaDon> LayChiTietHoaDonTheoSoHDN(List<string> soHDNlist)
        {
            // Ví dụ: lọc từ database
            return chiTietHoaDonDAL.LayTatCaChiTietHoaDon()
                     .Where(ct => soHDNlist.Contains(ct.SoHDN))
                     .ToList();
        }

        public List<HoaDon> LayHoaDonTheoNgay(DateTime fromDate, DateTime toDate)
        {
            var allHoaDons = hoaDonDAL.LayTatCaHoaDon();
            return allHoaDons
                    .Where(hd => hd.Ngay.Date >= fromDate.Date && hd.Ngay.Date <= toDate.Date)
                    .ToList();
        }
        public decimal TinhTongDoanhThu(DateTime fromDate, DateTime toDate)
        {
            var hoaDons = LayHoaDonTheoNgay(fromDate, toDate);
            return hoaDons.Sum(hd => hd.TongTien);
        }
        public decimal TinhDoanhThuTuNgayDenNgay(DateTime fromDate, DateTime toDate)
        {
            var hoaDons = LayHoaDonTheoNgay(fromDate, toDate);
            return hoaDons.Sum(hd => hd.TongTien);
        }
        public List<ChiTietHoaDon> LayChiTietHoaDonTheoNgay(DateTime fromDate, DateTime toDate)
        {
            var hoaDons = LayHoaDonTheoNgay(fromDate, toDate);
            var soHDNList = hoaDons.Select(hd => hd.SoHDN).ToList();
            return LayChiTietHoaDonTheoSoHDN(soHDNList);
        }
        public int TongSoLuongBanRa(DateTime fromDate, DateTime toDate)
        {
            var chiTiets = LayChiTietHoaDonTheoNgay(fromDate, toDate);
            return chiTiets.Sum(ct => ct.SoLuong);
        }
        public decimal GiaTriTrungBinhBanRa(DateTime fromDate, DateTime toDate)
        {
            var chiTiets = LayChiTietHoaDonTheoNgay(fromDate, toDate);
            int tongSoLuong = chiTiets.Sum(ct => ct.SoLuong);
            if (tongSoLuong == 0)
                return 0;
            decimal tongGiaTri = chiTiets.Sum(ct => ct.ThanhTien);
            return tongGiaTri / tongSoLuong;
        }
        public string LayMatHangBanChayNhat(DateTime fromDate, DateTime toDate)
        {
            var chiTiets = LayChiTietHoaDonTheoNgay(fromDate, toDate);
            var nhomHangBanChay = chiTiets
                .GroupBy(ct => ct.MaHang)
                .Select(g => new
                {
                    MaHang = g.Key,
                    TongSoLuong = g.Sum(ct => ct.SoLuong)
                })
                .OrderByDescending(x => x.TongSoLuong)
                .FirstOrDefault();
            if (nhomHangBanChay != null)
            {
                var hangHoaDAL = new HangHoaDAL();
                var hangHoas = hangHoaDAL.LayHangHoaTheoMa(nhomHangBanChay.MaHang);
                var hangHoa = hangHoas.FirstOrDefault(); // lấy phần tử đầu tiên

                return hangHoa != null ? hangHoa.TenHang : "Không xác định";
            }
            return "Không có dữ liệu";
        }
    }
}
