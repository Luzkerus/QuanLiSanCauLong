using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using QuanLiSanCauLong.LopDuLieu;

namespace QuanLiSanCauLong.LopTruyCapDuLieu
{
    public class ChiTietHoaDonDAL
    {
        private readonly string connectionString;
        public ChiTietHoaDonDAL()
        {
            ConnectStringDAL connect = new ConnectStringDAL();
            connectionString = connect.GetConnectionString();
        }
        public void ThemChiTietHoaDon(ChiTietHoaDon cthd)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
                INSERT INTO ChiTietHoaDon(SoHDN, MaHang, TenHang, SoLuong, DVT, GiaBan, ThanhTien, MaChiTiet)
                VALUES(@SoHDN, @MaHang, @TenHang, @SoLuong, @DVT, @GiaBan, @ThanhTien, @MaChiTiet)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@SoHDN", cthd.SoHDN);
                cmd.Parameters.AddWithValue("@MaHang", cthd.MaHang);
                cmd.Parameters.AddWithValue("@TenHang", cthd.TenHang);
                cmd.Parameters.AddWithValue("@SoLuong", cthd.SoLuong);
                cmd.Parameters.AddWithValue("@DVT", cthd.DVT);
                cmd.Parameters.AddWithValue("@GiaBan", cthd.GiaBan);
                cmd.Parameters.AddWithValue("@ThanhTien", cthd.ThanhTien);
                cmd.Parameters.AddWithValue("@MaChiTiet", cthd.MaChiTiet);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
