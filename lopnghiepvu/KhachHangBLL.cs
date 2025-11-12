using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopTruyCapDuLieu;

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
        public void CapNhatKhachHang(KhachHang kh)
        {
            khachHangDAL.CapNhatKhachHang(kh);
        }

        // Kiểm tra SDT mới có bị trùng
        public bool KiemTraTrungSDT(string sdtMoi)
        {
            return khachHangDAL.KiemTraTrungSDT(sdtMoi);
        }
    }
}
