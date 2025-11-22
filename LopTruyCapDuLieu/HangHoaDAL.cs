using QuanLiSanCauLong.LopDuLieu;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiSanCauLong.LopTruyCapDuLieu
{
    public class HangHoaDAL
    {
        private readonly string connectionString;

        public HangHoaDAL()
        {
            ConnectStringDAL connect = new ConnectStringDAL();
            connectionString = connect.GetConnectionString();
        }

        public bool KiemTraTonTai(string maHang)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM HangHoa WHERE MaHang = @MaHang";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaHang", maHang);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public void ThemHangMoi(ChiTietPhieuNhap ct)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
                INSERT INTO HangHoa(MaHang, TenHang, DVT, TonKho, GiaNhap, GiaBan, LanCuoiNhap)
                VALUES(@MaHang, @TenHang, @DVT, @TonKho, @GiaNhap, @GiaNhap, GETDATE())";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@MaHang", ct.MaHang);
                cmd.Parameters.AddWithValue("@TenHang", ct.TenHang);
                cmd.Parameters.AddWithValue("@DVT", ct.DVT);
                cmd.Parameters.AddWithValue("@TonKho", ct.SoLuong);
                cmd.Parameters.AddWithValue("@GiaNhap", ct.GiaNhap);

                conn.Open();
                cmd.ExecuteNonQuery();
                CapNhatTrangThaiHangHoa(ct.MaHang);
            }
        }

        public void CapNhatTonKho(ChiTietPhieuNhap ct)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
                UPDATE HangHoa SET 
                    TonKho = TonKho + @SL,
                    GiaNhap = @GiaNhap,
                    LanCuoiNhap = GETDATE()
                WHERE MaHang = @MaHang";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@SL", ct.SoLuong);
                cmd.Parameters.AddWithValue("@GiaNhap", ct.GiaNhap);
                cmd.Parameters.AddWithValue("@MaHang", ct.MaHang);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            CapNhatTrangThaiHangHoa(ct.MaHang);
        }
        public string LayMaHangLonNhat(string prefix)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"SELECT TOP 1 MaHang 
                       FROM HangHoa 
                       WHERE MaHang LIKE @Prefix + '%'
                       ORDER BY MaHang DESC";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Prefix", prefix);

                conn.Open();
                var result = cmd.ExecuteScalar();
                return result?.ToString();
            }
        }
        public string LayMaHangByTenVaDVT(string tenHang, string dvt)
        {
            // Kiểm tra đầu vào
            if (string.IsNullOrEmpty(tenHang) || string.IsNullOrEmpty(dvt))
            {
                return null; // Trả về null nếu thông tin tìm kiếm không hợp lệ
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Thêm điều kiện tìm kiếm DVT
                string sql = @"SELECT TOP 1 MaHang 
                        FROM HangHoa 
                        WHERE TenHang = @TenHang AND DVT = @DVT";

                SqlCommand cmd = new SqlCommand(sql, conn);

                // Thêm tham số @DVT
                cmd.Parameters.AddWithValue("@TenHang", tenHang);
                cmd.Parameters.AddWithValue("@DVT", dvt);

                try
                {
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    // Trả về Mã Hàng, hoặc null nếu không tìm thấy
                    return result?.ToString();
                }
                catch (SqlException ex)
                {
                    // Nên log (ghi nhật ký) lỗi để theo dõi vấn đề kết nối/database
                    // Console.WriteLine(ex.Message);
                    return null;
                }
            }

        }
        public void CapNhatTrangThaiHangHoa(string maHang)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Lấy số lượng hiện tại
                string selectSql = "SELECT TonKho FROM HangHoa WHERE MaHang = @MaHang";
                SqlCommand selectCmd = new SqlCommand(selectSql, conn);
                selectCmd.Parameters.AddWithValue("@MaHang", maHang);

                conn.Open();
                object result = selectCmd.ExecuteScalar();
                if (result == null) return; // Không tìm thấy mã hàng

                int tonKho = Convert.ToInt32(result);
                string trangThai;

                if (tonKho == 0)
                    trangThai = "Hết";
                else if (tonKho <= 30)
                    trangThai = "Sắp hết";
                else
                    trangThai = "Bình thường";

                // Cập nhật trạng thái
                string updateSql = "UPDATE HangHoa SET TrangThai = @TrangThai WHERE MaHang = @MaHang";
                SqlCommand updateCmd = new SqlCommand(updateSql, conn);
                updateCmd.Parameters.AddWithValue("@TrangThai", trangThai);
                updateCmd.Parameters.AddWithValue("@MaHang", maHang);

                updateCmd.ExecuteNonQuery();
            }
        }
        public List<HangHoa> LayTatCaHangHoa()
        {
            List<HangHoa> hangHoas = new List<HangHoa>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT MaHang, TenHang, DVT, TonKho, GiaNhap, GiaBan, LanCuoiNhap, TrangThai FROM HangHoa";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    HangHoa hh = new HangHoa
                    {
                        MaHang = reader["MaHang"].ToString(),
                        TenHang = reader["TenHang"].ToString(),
                        DVT = reader["DVT"].ToString(),
                        TonKhio = Convert.ToInt32(reader["TonKho"]),
                        GiaNhap = Convert.ToDecimal(reader["GiaNhap"]),
                        GiaBan = Convert.ToDecimal(reader["GiaBan"]),
                        LanCuoiNhap = Convert.ToDateTime(reader["LanCuoiNhap"]),
                        TrangThai = reader["TrangThai"].ToString()
                    };
                    hangHoas.Add(hh);
                }
            }
            return hangHoas;
        }
    }

}
