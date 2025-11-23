using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using QuanLiSanCauLong.LopDuLieu;

namespace QuanLiSanCauLong.LopTruyCapDuLieu
{
    public class HoaDonDAL
    {
        private readonly string connectionString;
        public HoaDonDAL()
        {
            ConnectStringDAL connect = new ConnectStringDAL();
            connectionString = connect.GetConnectionString();
        }
        public void ThemHoaDon(HoaDon hd)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
                INSERT INTO HoaDon(SoHDN, Ngay, TongTien, PhuongThuc)
                VALUES(@SoHDN, @Ngay, @TongTien, @PhuongThuc)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@SoHDN", hd.SoHDN);
                cmd.Parameters.AddWithValue("@Ngay", hd.Ngay);
                cmd.Parameters.AddWithValue("@TongTien", hd.TongTien);
                cmd.Parameters.AddWithValue("@PhuongThuc", hd.PhuongThuc);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public List<HoaDon> LayTatCaHoaDon()
        {
            List<HoaDon> hoaDons = new List<HoaDon>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT SoHDN, Ngay, TongTien, PhuongThuc FROM HoaDon";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    HoaDon hd = new HoaDon
                    {
                        SoHDN = reader.GetString(0),
                        Ngay = reader.GetDateTime(1),
                        TongTien = reader.GetDecimal(2),
                        PhuongThuc = reader.GetString(3)
                    };
                    hoaDons.Add(hd);
                }
            }
            return hoaDons;
        }
    }
}
