using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLiSanCauLong.LopDuLieu;

namespace QuanLiSanCauLong.LopTruyCapDuLieu
{
    public class PhieuNhapDAL
    {
        private readonly string connectionString;

        public PhieuNhapDAL()
        {
            ConnectStringDAL connect = new ConnectStringDAL();
            connectionString = connect.GetConnectionString();
        }

        public void LuuPhieu(PhieuNhap phieu)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
                INSERT INTO PhieuNhap (SoPhieu,NhaCungCap, NgayNhap, GhiChu, TongTien)
                VALUES (@SoPhieu,@NhaCungCap, @NgayNhap, @GhiChu, @TongTien)";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@SoPhieu", phieu.SoPhieu);
                cmd.Parameters.AddWithValue("@NhaCungCap", phieu.NhaCungCap ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NgayNhap", phieu.NgayNhap);
                cmd.Parameters.AddWithValue("@GhiChu", phieu.GhiChu ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TongTien", phieu.TongTien);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public string LaySoPhieuLonNhat(string prefix)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"SELECT TOP 1 SoPhieu
                       FROM PhieuNhap
                       WHERE SoPhieu LIKE @Prefix + '%'
                       ORDER BY SoPhieu DESC";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Prefix", prefix);

                conn.Open();
                var result = cmd.ExecuteScalar();

                return result?.ToString();
            }
        }
        public List<PhieuNhap> LayTatCaPhieuNhap()
        {
            List<PhieuNhap> phieuNhaps = new List<PhieuNhap>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT SoPhieu, NhaCungCap, NgayNhap, GhiChu, TongTien FROM PhieuNhap";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PhieuNhap phieu = new PhieuNhap
                    {
                        SoPhieu = reader["SoPhieu"].ToString(),
                        NhaCungCap = reader["NhaCungCap"].ToString(),
                        NgayNhap = Convert.ToDateTime(reader["NgayNhap"]),
                        GhiChu = reader["GhiChu"].ToString(),
                        TongTien = Convert.ToDecimal(reader["TongTien"])
                    };
                    phieuNhaps.Add(phieu);
                }
            }
            return phieuNhaps;
        }


    }

}
