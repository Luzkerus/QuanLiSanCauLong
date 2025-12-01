using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLiSanCauLong.LopDuLieu;

namespace QuanLiSanCauLong.LopTruyCapDuLieu
{
    public class ChiTietPhieuNhapDAL
    {
        private readonly string connectionString;
        public ChiTietPhieuNhapDAL()
        {
            connectionString = ConnectStringDAL.Instance.GetConnectionString();
        }
        public void ThemChiTiet(string soPhieu, ChiTietPhieuNhap ct)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
            INSERT INTO ChiTietPhieuNhap
            (MaChiTiet, MaHang, TenHang, DVT, SoLuong, GiaNhap, ChietKhau, ChietKhauTien,
             VAT, SoLo, HSD, ThanhTien, SoPhieu)
            VALUES (@MaChiTiet, @MaHang, @TenHang, @DVT, @SoLuong, @GiaNhap, @ChietKhau,
                    @ChietKhauTien, @VAT, @SoLo, @HSD, @ThanhTien, @SoPhieu)";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@MaChiTiet", ct.MaChiTiet);
                cmd.Parameters.AddWithValue("@MaHang", ct.MaHang);
                cmd.Parameters.AddWithValue("@TenHang", ct.TenHang);
                cmd.Parameters.AddWithValue("@DVT", ct.DVT);
                cmd.Parameters.AddWithValue("@SoLuong", ct.SoLuong);
                cmd.Parameters.AddWithValue("@GiaNhap", ct.GiaNhap);
                cmd.Parameters.AddWithValue("@ChietKhau", ct.ChietKhau);
                cmd.Parameters.AddWithValue("@ChietKhauTien", ct.ChietKhauTien);
                cmd.Parameters.AddWithValue("@VAT", ct.VAT);
                cmd.Parameters.AddWithValue("@SoLo", ct.SoLo ?? "");
                cmd.Parameters.AddWithValue("@HSD", ct.HSD);
                cmd.Parameters.AddWithValue("@ThanhTien", ct.ThanhTien);
                cmd.Parameters.AddWithValue("@SoPhieu", soPhieu);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public string LayMaChiTietLonNhat(string soPhieu)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"SELECT TOP 1 MaChiTiet
                       FROM ChiTietPhieuNhap
                       WHERE SoPhieu = @SoPhieu
                       ORDER BY MaChiTiet DESC";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@SoPhieu", soPhieu);

                conn.Open();
                var result = cmd.ExecuteScalar();

                return result?.ToString();
            }
        }
        public List<ChiTietPhieuNhap> LayTatCaChiTietPhieuNhap()
        {
            List<ChiTietPhieuNhap> chiTiets = new List<ChiTietPhieuNhap>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM ChiTietPhieuNhap";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ChiTietPhieuNhap ct = new ChiTietPhieuNhap
                    {
                        MaChiTiet = reader["MaChiTiet"].ToString(),
                        MaHang = reader["MaHang"].ToString(),
                        TenHang = reader["TenHang"].ToString(),
                        DVT = reader["DVT"].ToString(),
                        SoLuong = Convert.ToInt32(reader["SoLuong"]),
                        GiaNhap = Convert.ToDecimal(reader["GiaNhap"]),
                        ChietKhau = Convert.ToDecimal(reader["ChietKhau"]),
                        //ChietKhauTien = Convert.ToDecimal(reader["ChietKhauTien"]),
                        VAT = Convert.ToDecimal(reader["VAT"]),
                        SoLo = reader["SoLo"].ToString(),
                        HSD = Convert.ToDateTime(reader["HSD"]),
                        //ThanhTien = Convert.ToDecimal(reader["ThanhTien"]),
                        SoPhieu = reader["SoPhieu"].ToString()
                    };
                    chiTiets.Add(ct);
                }
            }
            return chiTiets;
        }
        public List<ChiTietPhieuNhap> LayChiTietSoPhieuTheoNgay(DateTime fromDate, DateTime toDate) { 
            List<ChiTietPhieuNhap> chiTiets = new List<ChiTietPhieuNhap>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
                SELECT ctpn.*
                FROM ChiTietPhieuNhap ctpn
                JOIN PhieuNhap pn ON ctpn.SoPhieu = pn.SoPhieu
                WHERE pn.NgayNhap BETWEEN @FromDate AND @ToDate";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ChiTietPhieuNhap ct = new ChiTietPhieuNhap
                    {
                        MaChiTiet = reader["MaChiTiet"].ToString(),
                        MaHang = reader["MaHang"].ToString(),
                        TenHang = reader["TenHang"].ToString(),
                        DVT = reader["DVT"].ToString(),
                        SoLuong = Convert.ToInt32(reader["SoLuong"]),
                        GiaNhap = Convert.ToDecimal(reader["GiaNhap"]),
                        ChietKhau = Convert.ToDecimal(reader["ChietKhau"]),
                        //ChietKhauTien = Convert.ToDecimal(reader["ChietKhauTien"]),
                        VAT = Convert.ToDecimal(reader["VAT"]),
                        SoLo = reader["SoLo"].ToString(),
                        HSD = Convert.ToDateTime(reader["HSD"]),
                        //ThanhTien = Convert.ToDecimal(reader["ThanhTien"]),
                        SoPhieu = reader["SoPhieu"].ToString()
                    };
                    chiTiets.Add(ct);
                }
            }

            return chiTiets;
        }
        public List<ChiTietPhieuNhap> LayChiTietTheoSoPhieu(string soPhieu)
        {
            List<ChiTietPhieuNhap> chiTiets = new List<ChiTietPhieuNhap>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM ChiTietPhieuNhap WHERE SoPhieu = @SoPhieu";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@SoPhieu", soPhieu);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ChiTietPhieuNhap ct = new ChiTietPhieuNhap
                    {
                        MaChiTiet = reader["MaChiTiet"].ToString(),
                        MaHang = reader["MaHang"].ToString(),
                        TenHang = reader["TenHang"].ToString(),
                        DVT = reader["DVT"].ToString(),
                        SoLuong = Convert.ToInt32(reader["SoLuong"]),
                        GiaNhap = Convert.ToDecimal(reader["GiaNhap"]),
                        ChietKhau = Convert.ToDecimal(reader["ChietKhau"]),
                        //ChietKhauTien = Convert.ToDecimal(reader["ChietKhauTien"]),
                        VAT = Convert.ToDecimal(reader["VAT"]),
                        SoLo = reader["SoLo"].ToString(),
                        HSD = Convert.ToDateTime(reader["HSD"]),
                        //ThanhTien = Convert.ToDecimal(reader["ThanhTien"]),
                        SoPhieu = reader["SoPhieu"].ToString()
                    };
                    chiTiets.Add(ct);
                }
            }
            return chiTiets;
        }

    }
}
