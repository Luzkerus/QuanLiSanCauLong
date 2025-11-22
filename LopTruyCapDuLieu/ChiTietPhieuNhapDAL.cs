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
            ConnectStringDAL connect = new ConnectStringDAL();
            connectionString = connect.GetConnectionString();
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


    }
}
