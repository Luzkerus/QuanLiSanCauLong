using QuanLiSanCauLong.LopTruyCapDuLieu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLiSanCauLong.LopDuLieu;

namespace QuanLiSanCauLong.LopNghiepVu
{
    public class ChiTietDatSanBLL
    {
        private ChiTietDatSanDAL dal = new ChiTietDatSanDAL();

        public bool KiemTraTrungLich(int maSan, DateTime ngayDat, TimeSpan gioBD, TimeSpan gioKT)
        {
            return dal.KiemTraTrungLich(maSan, ngayDat, gioBD, gioKT);
        }
        public bool CapNhatTrangThai(string maChiTiet, string trangThaiMoi)
        {
            return dal.CapNhatTrangThai(maChiTiet, trangThaiMoi);
        }
        public List<ChiTietChuaThanhToan> LayDanhSachChuaThanhToan(string sdt)
        {
            return dal.LayDanhSachChuaThanhToan(sdt);
        }
    }
}
