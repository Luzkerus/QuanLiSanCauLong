using QuanLiSanCauLong.LopDuLieu;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void CapNhatKhachHang(KhachHang kh, string sdtMoi)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tran;

                    try
                    {
                        // 1. Cập nhật các bảng liên quan nếu SDT thay đổi
                       

                        // 2. Cập nhật KhachHang
                        cmd.CommandText = @"
                        UPDATE KhachHang
                        SET SDT = @sdtMoi,
                            Ten = @ten,
                            Email = @email,
                            SDTPhu = @sdtPhu
                        WHERE SDT = @sdtCu";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@ten", kh.Ten);
                        cmd.Parameters.AddWithValue("@email", (object)kh.Email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@sdtPhu", (object)kh.SDTPhu ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@sdtMoi", sdtMoi);
                        cmd.Parameters.AddWithValue("@sdtCu", kh.SDT);

                        cmd.ExecuteNonQuery();

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }


        // Kiểm tra SDT mới đã tồn tại chưa (SDT chính hoặc SDT phụ)
        public bool KiemTraTrungSDT(string sdtMoi, string sdtCu = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @"
            SELECT 1 
            FROM KhachHang 
            WHERE (SDT = @sdt OR SDTPhu = @sdt)
            AND (@sdtCu IS NULL OR SDT <> @sdtCu)";
                cmd.Parameters.AddWithValue("@sdt", sdtMoi);
                cmd.Parameters.AddWithValue("@sdtCu", (object)sdtCu ?? DBNull.Value);

                return cmd.ExecuteScalar() != null;
            }
        }
        public bool KiemTraTrungSDTChinh(string sdtMoi, string sdtCu = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @"
            SELECT 1
            FROM KhachHang
            WHERE (SDT = @sdtMoi OR SDTPhu = @sdtMoi)
            AND (@sdtCu IS NULL OR SDT <> @sdtCu)";
                cmd.Parameters.AddWithValue("@sdtMoi", sdtMoi);
                cmd.Parameters.AddWithValue("@sdtCu", (object)sdtCu ?? DBNull.Value);

                return cmd.ExecuteScalar() != null;
            }
        }

        public bool KiemTraTrungSDTPhu(string sdtPhuMoi, string sdtCu = null)
        {
            if (string.IsNullOrWhiteSpace(sdtPhuMoi))
                return false; // nếu không nhập thì không trùng

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @"
            SELECT 1
            FROM KhachHang
            WHERE (SDT = @sdtPhuMoi OR SDTPhu = @sdtPhuMoi)
            AND (@sdtCu IS NULL OR SDT <> @sdtCu OR SDTPhu <> @sdtPhuMoi)";
                cmd.Parameters.AddWithValue("@sdtPhuMoi", sdtPhuMoi);
                cmd.Parameters.AddWithValue("@sdtCu", (object)sdtCu ?? DBNull.Value);

                return cmd.ExecuteScalar() != null;
            }
        }


        public DataTable LayKhachHangTheoSDT(string sdt)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT TOP 1 * FROM KhachHang WHERE SDT=@sdt OR SDTPhu=@sdt";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@sdt", sdt);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }
        public bool ThemKhachHang(KhachHang kh)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();

                // Kiểm tra SDT đã tồn tại chưa
                cmd.CommandText = "SELECT 1 FROM KhachHang WHERE SDT=@sdt OR SDTPhu=@sdt";
                cmd.Parameters.AddWithValue("@sdt", kh.SDT);
                if (cmd.ExecuteScalar() != null)
                    return false; // đã tồn tại

                // Thêm khách hàng mới
                cmd.CommandText = @"
            INSERT INTO KhachHang (SDT, Ten, Email, LuotChoi, TongChiTieu, TuNgay)
            VALUES (@sdt, @ten, @email, 0, 0, @tuNgay)";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@sdt", kh.SDT);
                cmd.Parameters.AddWithValue("@ten", kh.Ten);
                cmd.Parameters.AddWithValue("@email", (object)kh.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@tuNgay", DateTime.Now);

                int n = cmd.ExecuteNonQuery();
                return n > 0;
            }
        }

    }
}
