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
            if (sanDAL.KiemTraTenSanTonTai(san.TenSan))
                throw new Exception($"Tên sân '{san.TenSan}' đã tồn tại.");

            return sanDAL.ThemSanMoi(san);
        }
        public bool XoaSan(int maSan)
        {
            if (maSan <= 0)
                throw new Exception("Mã sân không hợp lệ.");

            return sanDAL.XoaSan(maSan);
        }
        public bool CapNhatSan(San san)
        {
            if (san.MaSan <= 0)
                throw new Exception("Mã sân không hợp lệ.");
            if (string.IsNullOrWhiteSpace(san.TenSan))
                throw new Exception("Tên sân không được để trống.");

            if (sanDAL.KiemTraTenSanTonTaiKhiSua(san.TenSan, san.MaSan))
                throw new Exception($"Tên sân '{san.TenSan}' đã tồn tại.");
            return sanDAL.CapNhatSan(san);
        }


    }
}
