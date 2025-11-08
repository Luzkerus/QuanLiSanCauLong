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
    public class BangGiaDAL
    {
        private readonly string connectionString;
        public BangGiaDAL()
        {
            ConnectStringDAL connect = new ConnectStringDAL();
            connectionString = connect.GetConnectionString();
        }
        public DataTable LayBangGiaChung()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM BangGiaChung ORDER BY LoaiNgay DESC, GioBatDau ASC";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
        public bool ThemBangGiaMau()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // 🟨 Nếu bảng của bạn KHÔNG cho phép NULL giờ,
                // thì dùng dòng này thay thế:
                string query = @"
                     INSERT INTO BangGiaChung (GioBatDau, GioKetThuc, DonGia, LoaiNgay, PhuThuLePercent)
                     VALUES ('00:00', '00:00', 120000, N'Thứ 2-Thứ 6', 3);
                 ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }
        public bool XoaBangGia(int maBangGia)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM BangGiaChung WHERE MaBangGia = @MaBangGia";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaBangGia", maBangGia);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        public bool SuaBangGia(BangGiaChung bangGia)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            UPDATE BangGiaChung
            SET GioBatDau = @GioBatDau,
                GioKetThuc = @GioKetThuc,
                DonGia = @DonGia,
                LoaiNgay = @LoaiNgay,
                PhuThuLePercent = @PhuThuLePercent
            WHERE MaBangGia = @MaBangGia";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaBangGia", bangGia.MaBangGia);
                    cmd.Parameters.AddWithValue("@GioBatDau", bangGia.GioBatDau);
                    cmd.Parameters.AddWithValue("@GioKetThuc", bangGia.GioKetThuc);
                    cmd.Parameters.AddWithValue("@DonGia", bangGia.DonGia);
                    cmd.Parameters.AddWithValue("@LoaiNgay", bangGia.LoaiNgay);
                    cmd.Parameters.AddWithValue("@PhuThuLePercent", bangGia.PhuThuLePercent ?? 0);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
