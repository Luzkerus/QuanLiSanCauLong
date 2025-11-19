using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopTruyCapDuLieu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiSanCauLong.LopNghiepVu
{
    public class ThanhToanBLL
    {
        private readonly ThanhToanDAL dal = new ThanhToanDAL();

        public bool LuuHoaDon(ThanhToan hd)
        {
            return dal.LuuHoaDon(hd);
        }
        public string TaoSoHoaDonMoi()
        {
            string ngay = DateTime.Now.ToString("yyyyMMdd");   // YYYYMMDD
            string prefix = "HD" + ngay;

            var dal = new ThanhToanDAL();

            // Lấy số hóa đơn mới nhất trong ngày
            string soHDCuoi = dal.LaySoHDMoiNhat(prefix);

            if (soHDCuoi == null)  // chưa có hóa đơn nào trong ngày
            {
                return prefix + "001";
            }

            // Cắt 3 số cuối, chuyển sang int rồi +1
            string sttStr = soHDCuoi.Substring(soHDCuoi.Length - 3);
            int stt = int.Parse(sttStr) + 1;

            return prefix + stt.ToString("D3"); // luôn 3 chữ số
        }
        public List<ThanhToan> LayTatCaHoaDon()
        {
            return dal.LayTatCaHoaDon();
        }
        public int LayTongHoaDonHomNay()
        {
            DateTime today = DateTime.Today;
            return LayTatCaHoaDon().Count(tt => tt.NgayLap.Date == today);
        }

        // Tổng doanh thu hôm nay
        public decimal LayDoanhThuHomNay()
        {
            DateTime today = DateTime.Today;
            return LayTatCaHoaDon()
                .Where(tt => tt.NgayLap.Date == today)
                .Sum(tt => tt.TongTien);
        }

        // Tổng doanh thu tháng hiện tại
        public decimal LayDoanhThuThang()
        {
            DateTime now = DateTime.Now;
            return LayTatCaHoaDon()
                .Where(tt => tt.NgayLap.Year == now.Year && tt.NgayLap.Month == now.Month)
                .Sum(tt => tt.TongTien);
        }
        public int LayTongHoaDonThang()
        {
            DateTime now = DateTime.Now;
            return LayTatCaHoaDon()
                .Count(tt => tt.NgayLap.Year == now.Year && tt.NgayLap.Month == now.Month);
        }
    }
}
   


