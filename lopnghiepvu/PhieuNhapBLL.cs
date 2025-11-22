using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopTruyCapDuLieu;
using System;
using System.Collections.Generic;

namespace QuanLiSanCauLong.LopNghiepVu
{
    public class PhieuNhapBLL
    {
        private readonly PhieuNhapDAL phieuNhapDAL;
        private readonly ChiTietPhieuNhapDAL chiTietPhieuNhapDAL;
        private readonly HangHoaBLL hangHoaBLL;

        public PhieuNhapBLL()
        {
            phieuNhapDAL = new PhieuNhapDAL();
            chiTietPhieuNhapDAL = new ChiTietPhieuNhapDAL();
            hangHoaBLL = new HangHoaBLL();
        }

        public void LuuPhieuNhap(PhieuNhap phieu, List<ChiTietPhieuNhap> chiTiets)
        {
            // 1. Tạo số phiếu tự động
            phieu.SoPhieu = TaoSoPhieu();

            // 2. Lưu phiếu cha
            phieuNhapDAL.LuuPhieu(phieu);

            // 3. Lưu chi tiết và cập nhật/tao hàng hóa
            int stt = 1;
            foreach (var ct in chiTiets)
            {
                // Tạo mã chi tiết = SoPhieu + thứ tự
                ct.MaChiTiet = $"{phieu.SoPhieu}-{stt:000}";
                stt++;

                // Nếu hàng chưa tồn tại → thêm mới
                if (!hangHoaBLL.KiemTraTonTai(ct.MaHang))
                {
                    hangHoaBLL.ThemHangMoi(ct);
                }
                else // Cập nhật tồn kho
                {
                    hangHoaBLL.CapNhatTonKho(ct);
                }

                // Lưu chi tiết phiếu
                chiTietPhieuNhapDAL.ThemChiTiet(phieu.SoPhieu, ct);
            }
        }

        // Hàm tạo số phiếu dạng PN + yyyyMMdd + 0001
        public string TaoSoPhieu()
        {
            string prefix = "PN" + DateTime.Now.ToString("yyyyMMdd");
            string soCuoi = phieuNhapDAL.LaySoPhieuLonNhat(prefix);

            int soMoi = 1;
            if (!string.IsNullOrEmpty(soCuoi))
            {
                int num = int.Parse(soCuoi.Substring(prefix.Length));
                soMoi = num + 1;
            }

            return $"{prefix}{soMoi:0000}";
        }
    }
}
