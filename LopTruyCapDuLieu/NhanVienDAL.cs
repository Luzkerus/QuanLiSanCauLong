using QuanLiSanCauLong.LopDuLieu;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiSanCauLong.LopTruyCapDuLieu
{
    public class NhanVienDAL
    {
        private readonly string connectionString;
        public NhanVienDAL()
        {
            // Chuỗi kết nối đến cơ sở dữ liệu SQL Server
            connectionString = ConnectStringDAL.Instance.GetConnectionString();
        }
        public bool ThemNhanVien(NhanVien nv)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"INSERT INTO NhanVien
                               (MaNV, TenNV, SDT, Email, Username, PasswordHash, VaiTro, NgayVaoLam, TrangThai, GhiChu)
                               VALUES (@MaNV, @TenNV, @SDT, @Email, @Username, @PasswordHash, @VaiTro, @NgayVaoLam, @TrangThai, @GhiChu)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaNV", nv.MaNV);
                cmd.Parameters.AddWithValue("@TenNV", nv.TenNV);
                cmd.Parameters.AddWithValue("@SDT", nv.SDT ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", nv.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Username", nv.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", nv.PasswordHash);
                cmd.Parameters.AddWithValue("@VaiTro", nv.VaiTro ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NgayVaoLam", nv.NgayVaoLam);
                cmd.Parameters.AddWithValue("@TrangThai", nv.TrangThai ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GhiChu", nv.GhiChu ?? (object)DBNull.Value);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ===== Sửa nhân viên =====
        public bool SuaNhanVien(NhanVien nv)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // KHÔNG cho sửa Username, PasswordHash, NgayVaoLam
                string sql = @"UPDATE NhanVien
                               SET TenNV = @TenNV,
                                   SDT = @SDT,
                                   Email = @Email,
                                   VaiTro = @VaiTro,
                                   TrangThai = @TrangThai,
                                   GhiChu = @GhiChu
                               WHERE MaNV = @MaNV";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaNV", nv.MaNV);
                cmd.Parameters.AddWithValue("@TenNV", nv.TenNV);
                cmd.Parameters.AddWithValue("@SDT", nv.SDT ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", nv.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@VaiTro", nv.VaiTro ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TrangThai", nv.TrangThai ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GhiChu", nv.GhiChu ?? (object)DBNull.Value);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public List<NhanVien> LayTatCaNhanVien()
        {
            List<NhanVien> danhSachNV = new List<NhanVien>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM NhanVien";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    NhanVien nv = new NhanVien
                    {
                        MaNV = reader["MaNV"].ToString(),
                        TenNV = reader["TenNV"].ToString(),
                        SDT = reader["SDT"] != DBNull.Value ? reader["SDT"].ToString() : null,
                        Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : null,
                        Username = reader["Username"].ToString(),
                        PasswordHash = reader["PasswordHash"].ToString(),
                        VaiTro = reader["VaiTro"] != DBNull.Value ? reader["VaiTro"].ToString() : null,
                        NgayVaoLam = Convert.ToDateTime(reader["NgayVaoLam"]),
                        TrangThai = reader["TrangThai"] != DBNull.Value ? reader["TrangThai"].ToString() : null,
                        GhiChu = reader["GhiChu"] != DBNull.Value ? reader["GhiChu"].ToString() : null
                    };
                    danhSachNV.Add(nv);
                }
            }
            return danhSachNV;
        }
        public string LayMaNhanVienTiepTheo(string prefix)
        {
            string sql = "SELECT MaNV FROM NhanVien WHERE MaNV LIKE @p + '%'";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@p", prefix);

                List<int> existingNumbers = new List<int>();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string ma = reader.GetString(0); // ví dụ: QL002
                        if (ma.Length > prefix.Length)
                        {
                            if (int.TryParse(ma.Substring(prefix.Length), out int num))
                            {
                                existingNumbers.Add(num);
                            }
                        }
                    }
                }

                // Nếu chưa có mã nào thì bắt đầu từ 1
                if (existingNumbers.Count == 0)
                    return prefix + "001";

                // Tìm số nhỏ nhất chưa tồn tại
                int next = 1;
                while (existingNumbers.Contains(next))
                {
                    next++;
                }

                return prefix + next.ToString("000");
            }
        }

        public bool DoiMatKhau(string maNV, string newPasswordHash)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"UPDATE NhanVien
                               SET PasswordHash = @PasswordHash
                               WHERE MaNV = @MaNV";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaNV", maNV);
                cmd.Parameters.AddWithValue("@PasswordHash", newPasswordHash);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public bool SuaNhanVienVaMaNV(NhanVien nv, string maNVCu)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"UPDATE NhanVien
                               SET MaNV = @MaNV,
                                   TenNV = @TenNV,
                                   SDT = @SDT,
                                   Email = @Email,
                                   VaiTro = @VaiTro,
                                   TrangThai = @TrangThai,
                                   GhiChu = @GhiChu
                               WHERE MaNV = @MaNVCu";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaNV", nv.MaNV);
                cmd.Parameters.AddWithValue("@MaNVCu", maNVCu);
                cmd.Parameters.AddWithValue("@TenNV", nv.TenNV);
                cmd.Parameters.AddWithValue("@SDT", nv.SDT ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", nv.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@VaiTro", nv.VaiTro ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TrangThai", nv.TrangThai ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GhiChu", nv.GhiChu ?? (object)DBNull.Value);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public NhanVien TimNhanVienTheoUsername(string username)
        {
            NhanVien nv = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM NhanVien WHERE Username = @Username";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    nv = new NhanVien
                    {
                        MaNV = reader["MaNV"].ToString(),
                        TenNV = reader["TenNV"].ToString(),
                        SDT = reader["SDT"] != DBNull.Value ? reader["SDT"].ToString() : null,
                        Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : null,
                        Username = reader["Username"].ToString(),
                        PasswordHash = reader["PasswordHash"].ToString(),
                        VaiTro = reader["VaiTro"] != DBNull.Value ? reader["VaiTro"].ToString() : null,
                        NgayVaoLam = Convert.ToDateTime(reader["NgayVaoLam"]),
                        TrangThai = reader["TrangThai"] != DBNull.Value ? reader["TrangThai"].ToString() : null,
                        GhiChu = reader["GhiChu"] != DBNull.Value ? reader["GhiChu"].ToString() : null
                    };
                }
            }
            return nv;
        }
    }
}
