using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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



    }
}
