using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLiSanCauLong.LopDuLieu;
using System.Data.SqlClient;

namespace QuanLiSanCauLong.LopTruyCapDuLieu
{
    public class KhachHangDAL
    {
        private readonly string connectionString;
        public KhachHangDAL()
        {
            ConnectStringDAL connect = new ConnectStringDAL();
            connectionString = connect.GetConnectionString();
        }
        public List<KhachHang> LayTatCaKhachHang()
        {
            List<KhachHang> dsKhachHang = new List<KhachHang>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT SDT, SDTPhu, Ten, Email, LuotChoi, TongChiTieu, TuNgay, DiemTichLuy FROM dbo.KhachHang";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    KhachHang kh = new KhachHang
                    {
                        SDT = reader.GetString(0),
                        SDTPhu = reader.IsDBNull(1) ? null : reader.GetString(1),
                        Ten = reader.GetString(2), // <-- sửa lại
                        Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                        LuotChoi = reader.GetInt32(4),
                        TongChiTieu = reader.GetDecimal(5),
                        TuNgay = reader.GetDateTime(6),
                        DiemTichLuy = reader.GetInt32(7)
                    };
                    dsKhachHang.Add(kh);
                }
            }
            return dsKhachHang;
        }
        public void CapNhatKhachHang(KhachHang kh)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @"
UPDATE KhachHang
SET Ten = @ten,
    Email = @email,
    SDTPhu = @sdtPhu
WHERE SDT = @sdt";
                cmd.Parameters.AddWithValue("@ten", kh.Ten);
                cmd.Parameters.AddWithValue("@email", (object)kh.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@sdtPhu", (object)kh.SDTPhu ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@sdt", kh.SDT);
                cmd.ExecuteNonQuery();
            }
        }

        // Kiểm tra SDT mới đã tồn tại chưa (SDT chính hoặc SDT phụ)
        public bool KiemTraTrungSDT(string sdtMoi)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "SELECT 1 FROM KhachHang WHERE SDT = @sdt OR SDTPhu = @sdt";
                cmd.Parameters.AddWithValue("@sdt", sdtMoi);
                return cmd.ExecuteScalar() != null;
            }
        }
    }
}
