using QuanLiSanCauLong.LopDuLieu;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiSanCauLong.LopTruyCapDuLieu
{
    public class ThanhToanDAL
    {
        private string connectionString;
        public ThanhToanDAL()
        {
            connectionString = ConnectStringDAL.Instance.GetConnectionString();
        }
        public bool LuuHoaDon(ThanhToan hoaDon)
        {
            string query = @"
        INSERT INTO ThanhToan
        (SoHD, SDT, TenKH, NgayLap, TongTienSan, TongTienThueVot, TongTien, PhuongThuc)
        VALUES
        (@SoHD, @SDT, @TenKH, @NgayLap, @TongTienSan, @TongTienThueVot, @TongTien, @PhuongThuc)
    ";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SoHD", hoaDon.SoHD);
                cmd.Parameters.AddWithValue("@SDT", hoaDon.SDT);
                cmd.Parameters.AddWithValue("@TenKH", hoaDon.TenKH);
                cmd.Parameters.AddWithValue("@NgayLap", hoaDon.NgayLap);
                cmd.Parameters.AddWithValue("@TongTienSan", hoaDon.TongTienSan);
                cmd.Parameters.AddWithValue("@TongTienThueVot", hoaDon.TongTienThueVot);
                cmd.Parameters.AddWithValue("@TongTien", hoaDon.TongTien);
                cmd.Parameters.AddWithValue("@PhuongThuc", hoaDon.PhuongThuc);

                conn.Open();

                int rows = cmd.ExecuteNonQuery();

                return rows > 0;    // true = thành công
            }
        }
        public string LaySoHDMoiNhat(string prefix)
        {
            string sql = @"
        SELECT TOP 1 SoHD 
        FROM ThanhToan 
        WHERE SoHD LIKE @prefix + '%'
        ORDER BY SoHD DESC
    ";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@prefix", prefix);

                conn.Open();
                var result = cmd.ExecuteScalar();

                return result == null ? null : result.ToString();
            }
        }
        public List<ThanhToan> LayTatCaHoaDon()
        {
            List<ThanhToan> danhSach = new List<ThanhToan>();
            string query = "SELECT * FROM ThanhToan";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ThanhToan hd = new ThanhToan
                        {
                            SoHD = reader["SoHD"].ToString(),
                            SDT = reader["SDT"].ToString(),
                            TenKH = reader["TenKH"].ToString(),
                            NgayLap = Convert.ToDateTime(reader["NgayLap"]),
                            TongTienSan = Convert.ToDecimal(reader["TongTienSan"]),
                            TongTienThueVot = Convert.ToDecimal(reader["TongTienThueVot"]),
                            TongTien = Convert.ToDecimal(reader["TongTien"]),
                            PhuongThuc = reader["PhuongThuc"].ToString()
                        };
                        danhSach.Add(hd);
                    }
                }
            }
            return danhSach;
        }

    }
}
