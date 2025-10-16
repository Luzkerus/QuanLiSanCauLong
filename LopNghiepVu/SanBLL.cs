using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopTruyCapDuLieu;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public bool ThemSanMoi(San san)
        {
            // Có thể thêm kiểm tra nghiệp vụ ở đây, ví dụ:
            if (string.IsNullOrWhiteSpace(san.TenSan))
                throw new Exception("Tên sân không được để trống.");
            if (san.GiaNgayThuong <= 0 || san.GiaCuoiTuan <= 0 || san.GiaLeTet <= 0)
                throw new Exception("Giá sân phải lớn hơn 0.");

            return sanDAL.ThemSanMoi(san);
        }

    }
}
