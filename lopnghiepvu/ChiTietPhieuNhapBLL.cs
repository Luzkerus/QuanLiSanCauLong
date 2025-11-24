using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopTruyCapDuLieu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiSanCauLong.LopNghiepVu
{
    public class ChiTietPhieuNhapBLL
    {
        ChiTietPhieuNhapDAL chiTietPhieuNhapDAL = new ChiTietPhieuNhapDAL();
        public List<ChiTietPhieuNhap> LayTatCaChiTietPhieuNhap()
        {
            return chiTietPhieuNhapDAL.LayTatCaChiTietPhieuNhap();
        }
        public List<ChiTietPhieuNhap> LayChiTietSoPhieuTheoNgay(DateTime fromDate, DateTime toDate)
        {
            return chiTietPhieuNhapDAL.LayChiTietSoPhieuTheoNgay(fromDate, toDate);
        }
        public List<ChiTietPhieuNhap> LayChiTietTheoSoPhieu(string soPhieu)
        {
            return chiTietPhieuNhapDAL.LayChiTietTheoSoPhieu(soPhieu);
        }
    }
}
