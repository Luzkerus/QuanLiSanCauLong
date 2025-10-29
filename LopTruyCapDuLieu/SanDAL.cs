using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QuanLiSanCauLong.LopDuLieu;

namespace QuanLiSanCauLong.LopTruyCapDuLieu
{
    public class SanDAL
    {
        private readonly string connectionString =
"Data Source=localhost;Initial Catalog=QuanLiSanCauLong;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        public List<San> LayTatCaSan()
        {
            CapNhatTrangThaiTuDong();
            List<San> dsSan = new List<San>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT MaSan, TenSan, TrangThai, NgayBaoTri FROM dbo.San";
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
                        NgayBaoTri = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3)

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
                         (TenSan, TrangThai, NgayBaoTri)
                         VALUES (@TenSan, @TrangThai, @NgayBaoTri)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenSan", san.TenSan);
                cmd.Parameters.AddWithValue("@TrangThai", san.TrangThai);
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

        public bool CapNhatSan(San san)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE dbo.San
                         SET TenSan = @TenSan,
                             TrangThai = @TrangThai,
                             NgayBaoTri = @NgayBaoTri
                         WHERE MaSan = @MaSan";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaSan", san.MaSan);
                cmd.Parameters.AddWithValue("@TenSan", san.TenSan);
                cmd.Parameters.AddWithValue("@TrangThai", san.TrangThai);
                cmd.Parameters.AddWithValue("@NgayBaoTri",
                    san.NgayBaoTri == null ? (object)DBNull.Value : san.NgayBaoTri);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public void CapNhatTrangThaiTuDong()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            UPDATE dbo.San
            SET TrangThai = N'Bảo trì'
            WHERE CAST(NgayBaoTri AS date) = CAST(GETDATE() AS date);

            UPDATE dbo.San
            SET TrangThai = N'Đang hoạt động'
            WHERE NgayBaoTri IS NOT NULL AND CAST(NgayBaoTri AS date) < CAST(GETDATE() AS date);
        ";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


    }
}
