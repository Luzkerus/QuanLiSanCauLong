using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QuanLiSanCauLong.LopDuLieu;

namespace QuanLiSanCauLong.LopTruyCapDuLieu
{
    public class SanDAL
    {
        private readonly string connectionString =
            "Data Source=.;Initial Catalog=QuanLiSanCauLong;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        public List<San> LayTatCaSan()
        {
            List<San> dsSan = new List<San>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT MaSan, TenSan, TrangThai, GiaNgayThuong, GiaCuoiTuan, GiaLeTet, NgayBaoTri FROM dbo.San";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    San san = new San
                    {
                        MaSan = reader.GetInt32(0),
                        TenSan = reader.GetString(1),
                        TrangThai = reader.GetString(2),
                        GiaNgayThuong = reader.GetDecimal(3),
                        GiaCuoiTuan = reader.GetDecimal(4),
                        GiaLeTet = reader.GetDecimal(5),
                        NgayBaoTri = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6)
                    };
                    dsSan.Add(san);
                }
            }

            return dsSan;
        }
        public bool ThemSanMoi(San san)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO dbo.San 
                         (TenSan, TrangThai, GiaNgayThuong, GiaCuoiTuan, GiaLeTet, NgayBaoTri)
                         VALUES (@TenSan, @TrangThai, @GiaNgayThuong, @GiaCuoiTuan, @GiaLeTet, @NgayBaoTri)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenSan", san.TenSan);
                cmd.Parameters.AddWithValue("@TrangThai", san.TrangThai);
                cmd.Parameters.AddWithValue("@GiaNgayThuong", san.GiaNgayThuong);
                cmd.Parameters.AddWithValue("@GiaCuoiTuan", san.GiaCuoiTuan);
                cmd.Parameters.AddWithValue("@GiaLeTet", san.GiaLeTet);
                cmd.Parameters.AddWithValue("@NgayBaoTri",
    san.NgayBaoTri == DateTime.MinValue ? (object)DBNull.Value : san.NgayBaoTri);


                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }
        public bool XoaSan(int maSan)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM San WHERE MaSan = @MaSan";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaSan", maSan);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

    }
}
