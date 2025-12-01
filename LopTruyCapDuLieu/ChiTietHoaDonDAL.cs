using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using QuanLiSanCauLong.LopDuLieu;

namespace QuanLiSanCauLong.LopTruyCapDuLieu
{
    public class ChiTietHoaDonDAL
    {
        private readonly string connectionString;
        public ChiTietHoaDonDAL()
        {
            connectionString = ConnectStringDAL.Instance.GetConnectionString();
        }
        public void ThemChiTietHoaDon(ChiTietHoaDon cthd)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
                INSERT INTO ChiTietHoaDon(SoHDN, MaHang, TenHang, SoLuong, DVT, GiaBan, ThanhTien, MaChiTiet)
                VALUES(@SoHDN, @MaHang, @TenHang, @SoLuong, @DVT, @GiaBan, @ThanhTien, @MaChiTiet)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@SoHDN", cthd.SoHDN);
                cmd.Parameters.AddWithValue("@MaHang", cthd.MaHang);
                cmd.Parameters.AddWithValue("@TenHang", cthd.TenHang);
                cmd.Parameters.AddWithValue("@SoLuong", cthd.SoLuong);
                cmd.Parameters.AddWithValue("@DVT", cthd.DVT);
                cmd.Parameters.AddWithValue("@GiaBan", cthd.GiaBan);
                cmd.Parameters.AddWithValue("@ThanhTien", cthd.ThanhTien);
                cmd.Parameters.AddWithValue("@MaChiTiet", cthd.MaChiTiet);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public List<ChiTietHoaDon> LayChiTietHoaDonTheoSoHDN(string soHDN)
        {
            List<ChiTietHoaDon> chiTietHoaDons = new List<ChiTietHoaDon>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT MaChiTiet, MaHang, TenHang, SoLuong, DVT, GiaBan, ThanhTien, SoHDN FROM ChiTietHoaDon WHERE SoHDN = @SoHDN";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@SoHDN", soHDN);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ChiTietHoaDon cthd = new ChiTietHoaDon
                    {
                        MaChiTiet = reader.GetString(0),
                        MaHang = reader.GetString(1),
                        TenHang = reader.GetString(2),
                        SoLuong = reader.GetInt32(3),
                        DVT = reader.GetString(4),
                        GiaBan = reader.GetDecimal(5),
                        ThanhTien = reader.GetDecimal(6),
                        SoHDN = reader.GetString(7)
                    };
                    chiTietHoaDons.Add(cthd);
                }
            }
            return chiTietHoaDons;
        }
        public List<ChiTietHoaDon> LayTatCaChiTietHoaDon() { 
            List<ChiTietHoaDon> chiTietHoaDons = new List<ChiTietHoaDon>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT MaChiTiet, MaHang, TenHang, SoLuong, DVT, GiaBan, ThanhTien, SoHDN FROM ChiTietHoaDon";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ChiTietHoaDon cthd = new ChiTietHoaDon
                    {
                        MaChiTiet = reader.GetString(0),
                        MaHang = reader.GetString(1),
                        TenHang = reader.GetString(2),
                        SoLuong = reader.GetInt32(3),
                        DVT = reader.GetString(4),
                        GiaBan = reader.GetDecimal(5),
                        ThanhTien = reader.GetDecimal(6),
                        SoHDN = reader.GetString(7)
                    };
                    chiTietHoaDons.Add(cthd);
                }
            }
            return chiTietHoaDons;
        }
    }
}
