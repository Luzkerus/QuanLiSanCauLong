using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopTrinhBay.ManHinh.DatSan;
using System;
using System.Collections.Generic;

using System.Data.SqlClient;

namespace QuanLiSanCauLong.LopTruyCapDuLieu
{
    public class DatSanDAL
    {
        private readonly string connectionString;


        public DatSanDAL()
        {
            connectionString = ConnectStringDAL.Instance.GetConnectionString();
        }
        public bool LuuDatSan(DatSan datSan, List<ChiTietDatSan> chiTiets)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Insert DatSan
                        var cmdDatSan = new SqlCommand(
                            "INSERT INTO DatSan (MaPhieu, SDT, NgayTao, TongTien) VALUES (@MaPhieu, @SDT, @NgayTao, @TongTien)",
                            conn, tran);
                        cmdDatSan.Parameters.AddWithValue("@MaPhieu", datSan.MaPhieu);
                        cmdDatSan.Parameters.AddWithValue("@SDT", datSan.SDT);
                        cmdDatSan.Parameters.AddWithValue("@NgayTao", datSan.NgayTao);
                        cmdDatSan.Parameters.AddWithValue("@TongTien", datSan.TongTien);
                        cmdDatSan.ExecuteNonQuery();

                        // 2. Insert ChiTietDatSan
                        foreach (var c in chiTiets)
                        {
                            var cmdChiTiet = new SqlCommand(
     @"INSERT INTO ChiTietDatSan 
    (MaChiTiet, MaPhieu, MaSan, TenSanCached, NgayDat, GioBatDau, GioKetThuc, DonGia, PhuThuLe, ThanhTien) 
    VALUES (@MaChiTiet, @MaPhieu, @MaSan, @TenSanCached, @NgayDat, @GioBatDau, @GioKetThuc, @DonGia, @PhuThuLe, @ThanhTien)",
     conn, tran);


                            cmdChiTiet.Parameters.AddWithValue("@MaChiTiet", c.MaChiTiet);
                            cmdChiTiet.Parameters.AddWithValue("@MaPhieu", c.MaPhieu);
                            cmdChiTiet.Parameters.AddWithValue("@MaSan", c.MaSan);
                            cmdChiTiet.Parameters.AddWithValue("@TenSanCached", c.TenSanCached);
                            cmdChiTiet.Parameters.AddWithValue("@NgayDat", c.NgayDat);
                            cmdChiTiet.Parameters.AddWithValue("@GioBatDau", c.GioBatDau);
                            cmdChiTiet.Parameters.AddWithValue("@GioKetThuc", c.GioKetThuc);
                            cmdChiTiet.Parameters.AddWithValue("@DonGia", c.DonGia);
                            cmdChiTiet.Parameters.AddWithValue("@PhuThuLe", c.PhuThuLe);
                            cmdChiTiet.Parameters.AddWithValue("@ThanhTien", c.ThanhTien);
                            cmdChiTiet.ExecuteNonQuery();
                        }

                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        return false;
                    }
                }
            }
        }

        public string LayMaPhieuCuoiTrongNgay(string ngay)
        {
            string maPhieuCuoi = null;

            using (SqlConnection conn = new SqlConnection(connectionString)) // <-- dùng biến instance
            {
                conn.Open();
                string sql = @"
            SELECT TOP 1 MaPhieu
            FROM DatSan
            WHERE CONVERT(VARCHAR(8), NgayTao, 112) = @Ngay
            ORDER BY MaPhieu DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Ngay", ngay);

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        maPhieuCuoi = result.ToString();
                }
            }

            return maPhieuCuoi;
        }
        public List<ChiTietDatSanVM> LayTatCaDatSan()
        {
            var danhSach = new List<ChiTietDatSanVM>();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"
            SELECT c.*, k.Ten AS TenKH, k.Email, d.SDT
            FROM ChiTietDatSan c
            INNER JOIN DatSan d ON c.MaPhieu = d.MaPhieu
            INNER JOIN KhachHang k ON d.SDT = k.SDT
            ORDER BY c.NgayDat ASC, c.GioBatDau ASC";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        danhSach.Add(new ChiTietDatSanVM
                        {
                            MaChiTiet = reader["MaChiTiet"].ToString(),
                            MaPhieu = reader["MaPhieu"].ToString(),
                            MaSan = Convert.ToInt32(reader["MaSan"]),
                            TenSanCached = reader["TenSanCached"].ToString(),
                            NgayDat = Convert.ToDateTime(reader["NgayDat"]),
                            GioBatDau = (TimeSpan)reader["GioBatDau"],
                            GioKetThuc = (TimeSpan)reader["GioKetThuc"],
                            DonGia = Convert.ToDecimal(reader["DonGia"]),
                            PhuThuLe = Convert.ToDecimal(reader["PhuThuLe"]),
                            ThanhTien = Convert.ToDecimal(reader["ThanhTien"]),
                            TenKH = reader["TenKH"].ToString(),
                            SDT = reader["SDT"].ToString(),
                            Email = reader["Email"].ToString(),
                            TrangThai = reader["TrangThai"].ToString(),
                            TrangThaiThanhToan = reader["TrangThaiThanhToan"].ToString()
                        });
                    }
                }
            }

            return danhSach;
        }
        public List<ChiTietDatSanVM> LayTop15DatSan()
        {
            var danhSach = new List<ChiTietDatSanVM>();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"
            SELECT TOP 15 c.*, k.Ten AS TenKH, k.Email, d.SDT
            FROM ChiTietDatSan c
            INNER JOIN DatSan d ON c.MaPhieu = d.MaPhieu
            INNER JOIN KhachHang k ON d.SDT = k.SDT
            ORDER BY c.NgayDat ASC, c.GioBatDau ASC";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        danhSach.Add(new ChiTietDatSanVM
                        {
                            MaChiTiet = reader["MaChiTiet"].ToString(),
                            MaPhieu = reader["MaPhieu"].ToString(),
                            MaSan = Convert.ToInt32(reader["MaSan"]),
                            TenSanCached = reader["TenSanCached"].ToString(),
                            NgayDat = Convert.ToDateTime(reader["NgayDat"]),
                            GioBatDau = (TimeSpan)reader["GioBatDau"],
                            GioKetThuc = (TimeSpan)reader["GioKetThuc"],
                            DonGia = Convert.ToDecimal(reader["DonGia"]),
                            PhuThuLe = Convert.ToDecimal(reader["PhuThuLe"]),
                            ThanhTien = Convert.ToDecimal(reader["ThanhTien"]),
                            TenKH = reader["TenKH"].ToString(),
                            SDT = reader["SDT"].ToString(),
                            Email = reader["Email"].ToString(),
                            TrangThai = reader["TrangThai"].ToString(),
                            TrangThaiThanhToan = reader["TrangThaiThanhToan"].ToString()
                        });
                    }
                }
            }

            return danhSach;
        }

    }
}
