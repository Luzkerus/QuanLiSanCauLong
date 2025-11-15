using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopTruyCapDuLieu;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiSanCauLong.LopNghiepVu
{
    public class KhachHangBLL
    {
        KhachHangDAL khachHangDAL = new KhachHangDAL();
        public List<KhachHang> LayTatCaKhachHang()
        {
            return khachHangDAL.LayTatCaKhachHang();
        }
        public int LayTongKhachHang()
        {
            return LayTatCaKhachHang().Count;
        }

        // 👉 2. Tổng số hội viên (LuotChoi > 10)
        public int LayTongHoiVien()
        {
            return LayTatCaKhachHang().Count(kh => kh.LuotChoi > 10);
        }

        // 👉 3. Tổng chi tiêu của tất cả khách hàng
        public decimal LayTongDiemTichLuy()
        {
            return LayTatCaKhachHang().Sum(kh => kh.DiemTichLuy);
        }

        // 👉 4. Tổng khách hàng mới trong tháng hiện tại
        public int LayTongKhachHangMoiTrongThang()
        {
            List<KhachHang> ds = LayTatCaKhachHang();
            DateTime now = DateTime.Now;
            return ds.Count(kh => kh.TuNgay.Month == now.Month && kh.TuNgay.Year == now.Year);
        }
        public void CapNhatKhachHang(KhachHang kh, string sdtmoi)
        {
            khachHangDAL.CapNhatKhachHang(kh, sdtmoi);
        }

        // Kiểm tra SDT mới có bị trùng
        public bool KiemTraTrungSDT(string sdtMoi)
        {
            return khachHangDAL.KiemTraTrungSDT(sdtMoi);
        }
        public bool KiemTraTrungSDTPhu(string sdtPhuMoi, string sdtCu)
        {
            return khachHangDAL.KiemTraTrungSDTPhu(sdtPhuMoi, sdtCu);
        }
        public bool KiemTraTrungSDTChinh(string sdtPhuMoi, string sdtCu)
        {
            return khachHangDAL.KiemTraTrungSDTChinh(sdtPhuMoi, sdtCu);
        }
        public KhachHang LayKhachHangTheoSDT(string sdt)
        {
            DataTable dt = khachHangDAL.LayKhachHangTheoSDT(sdt);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new KhachHang
                {
                    SDT = row["SDT"].ToString(),
                    SDTPhu = row["SDTPhu"].ToString(),
                    Ten = row["Ten"].ToString(),
                    Email = row["Email"].ToString()
                    // map thêm các field khác
                };
            }
            return null;
        }
        public bool ThemKhachHangMoi(KhachHang kh)
        {
            return khachHangDAL.ThemKhachHang(kh);
        }
    }
}
