using System.Collections.Generic;
using System.Linq;
using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopTruyCapDuLieu;

namespace QuanLiSanCauLong.LopNghiepVu
{
    public class SanBLL
    {
        private SanDAL sanDAL = new SanDAL();

        public List<San> LayTatCaSan()
        {
            return sanDAL.LayTatCaSan();
        }

        public int DemTongSoSan(List<San> danhSach) => danhSach.Count;
        public int DemSanBaoTri(List<San> danhSach) => danhSach.Count(s => s.TrangThai == "Bảo trì");
        public int DemSanKhongBaoTri(List<San> danhSach) => danhSach.Count(s => s.TrangThai != "Bảo trì");
    }
}
