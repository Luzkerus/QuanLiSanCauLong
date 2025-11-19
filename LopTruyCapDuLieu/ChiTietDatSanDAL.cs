using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLiSanCauLong.LopDuLieu;

namespace QuanLiSanCauLong.LopTruyCapDuLieu
{
    public class ChiTietDatSanDAL
    {
        private readonly string connectionString;
        public ChiTietDatSanDAL()
        {
            ConnectStringDAL connect = new ConnectStringDAL();
            connectionString = connect.GetConnectionString();
        }
        public bool KiemTraTrungLich(int maSan, DateTime ngayDat, TimeSpan gioBD, TimeSpan gioKT)
        {
            const string sql = @"
        SELECT COUNT(*) FROM ChiTietDatSan
        WHERE MaSan = @MaSan
          AND NgayDat = @NgayDat
          AND (
                (@GioBD >= GioBatDau AND @GioBD < GioKetThuc) OR
                (@GioKT > GioBatDau AND @GioKT <= GioKetThuc) OR
                (@GioBD <= GioBatDau AND @GioKT >= GioKetThuc)
              )";

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();

                cmd.Parameters.AddWithValue("@MaSan", maSan);
                cmd.Parameters.AddWithValue("@NgayDat", ngayDat);
                cmd.Parameters.AddWithValue("@GioBD", gioBD);
                cmd.Parameters.AddWithValue("@GioKT", gioKT);

                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
        public bool CapNhatTrangThai(string maChiTiet, string trangThaiMoi)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(
                "UPDATE ChiTietDatSan SET TrangThai = @TrangThai WHERE MaChiTiet = @MaChiTiet", conn))
            {
                cmd.Parameters.AddWithValue("@TrangThai", trangThaiMoi);
                cmd.Parameters.AddWithValue("@MaChiTiet", maChiTiet);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<ChiTietChuaThanhToan> LayDanhSachChuaThanhToan(string sdt)
        {
            const string sql = @"
        SELECT ct.MaPhieu, ct.TenSanCached, ct.NgayDat, ct.GioBatDau, ct.GioKetThuc, ct.ThanhTien
        FROM ChiTietDatSan ct
        JOIN DatSan d ON ct.MaPhieu = d.MaPhieu
        JOIN KhachHang kh ON d.SDT = kh.SDT
        WHERE kh.SDT = @SDT
          AND ct.TrangThaiThanhToan = N'Chưa thanh toán' AND ct.TrangThai <> N'Đã hủy';
    ";

            var result = new List<ChiTietChuaThanhToan>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@SDT", sdt);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new ChiTietChuaThanhToan
                        {
                            MaPhieu = reader.GetString(0),
                            TenSanCached = reader.GetString(1),
                            NgayDat = reader.GetDateTime(2),
                            GioBatDau = reader.GetTimeSpan(3),
                            GioKetThuc = reader.GetTimeSpan(4),
                            ThanhTien = reader.GetDecimal(5)
                        });
                    }
                }
            }

            return result;
        }

        public bool CapNhatTrangThaiDanhSach(List<string> danhSachMaChiTiet, string trangThaiMoi)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                foreach (var maChiTiet in danhSachMaChiTiet)
                {
                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE ChiTietDatSan SET TrangThaiThanhToan = @TrangThai WHERE MaPhieu = @MaChiTiet", conn))
                    {
                        cmd.Parameters.AddWithValue("@TrangThai", trangThaiMoi);
                        cmd.Parameters.AddWithValue("@MaChiTiet", maChiTiet);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
        }

    }
}
