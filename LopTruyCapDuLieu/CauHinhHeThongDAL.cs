using QuanLiSanCauLong.LopDuLieu;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System;

namespace QuanLiSanCauLong.LopTruyCapDuLieu
{
    // Giả định: Bạn phải có lớp ConnectStringDAL để lấy chuỗi kết nối
    // hoặc đã định nghĩa connectionString ở một nơi khác.
    public class CauHinhHeThongDAL
    {
        private readonly string connectionString;

        public CauHinhHeThongDAL()
        {
            // Lấy chuỗi kết nối (Cần có ConnectStringDAL hoặc thay bằng chuỗi cứng)
            ConnectStringDAL connect = new ConnectStringDAL();
            connectionString = connect.GetConnectionString();
        }

        // --- LayCauHinh (Dùng SqlDataAdapter) ---
        /// <summary>
        /// Lấy toàn bộ cấu hình hệ thống từ Database.
        /// </summary>
        public CauHinhHeThong LayCauHinh()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT TenThamSo, GiaTriThamSo FROM CauHinhHeThong";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();

                try
                {
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    // Xử lý hoặc ghi log lỗi kết nối/DB
                    Console.WriteLine("Lỗi khi tải cấu hình: " + ex.Message);
                    return new CauHinhHeThong(); // Trả về đối tượng rỗng/mặc định khi lỗi
                }

                // Chuyển DataTable thành Dictionary<string, string>
                var configDict = dt.AsEnumerable()
                                   .ToDictionary(row => row.Field<string>("TenThamSo"),
                                                 row => row.Field<string>("GiaTriThamSo"));

                return MapDictionaryToDTO(configDict);
            }
        }

        // --- LuuCauHinh (Dùng SqlCommand.ExecuteNonQuery) ---
        /// <summary>
        /// Lưu cấu hình hệ thống vào Database (Sử dụng UPDATE).
        /// </summary>
        public bool LuuCauHinh(CauHinhHeThong cauHinh)
        {
            var configDict = MapDTOToDictionary(cauHinh);
            bool allSuccess = true;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open(); // Mở kết nối 1 lần cho nhiều lệnh UPDATE

                foreach (var kvp in configDict)
                {
                    string sql = "UPDATE CauHinhHeThong SET GiaTriThamSo = @Value WHERE TenThamSo = @Key";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        // Khởi tạo tham số
                        cmd.Parameters.AddWithValue("@Value", kvp.Value);
                        cmd.Parameters.AddWithValue("@Key", kvp.Key);

                        try
                        {
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected == 0)
                            {
                                // Nếu rowsAffected là 0, có thể là do giá trị không đổi
                                // hoặc tham số không tồn tại. Ta vẫn coi là thành công.
                            }
                        }
                        catch (Exception ex)
                        {
                            // Ghi log lỗi cho tham số này
                            Console.WriteLine($"Lỗi khi lưu tham số {kvp.Key}: {ex.Message}");
                            allSuccess = false; // Đánh dấu là có lỗi xảy ra
                            // Không return ngay, cố gắng lưu các tham số còn lại
                        }
                    }
                }
            } // Kết nối tự động đóng ở đây

            return allSuccess;
        }

        // --- Hàm hỗ trợ Ánh xạ DTO <-> Dictionary (Không đổi) ---

        private CauHinhHeThong MapDictionaryToDTO(Dictionary<string, string> dict)
        {
            int GetInt(string key) => dict.ContainsKey(key) && int.TryParse(dict[key], out int val) ? val : 0;

            return new CauHinhHeThong
            {
                CanhBaoNoShow = GetInt("CanhBaoNoShow"),
                SoSanToiDa = GetInt("SoSanToiDa"),
                SoSlotToiDa = GetInt("SoSlotToiDa"),
                TimeoutPhien = GetInt("TimeoutPhien"),
                NguongTonKhoThap = GetInt("NguongTonKhoThap"),
            };
        }

        private Dictionary<string, string> MapDTOToDictionary(CauHinhHeThong dto)
        {
            return new Dictionary<string, string>
            {
                { "CanhBaoNoShow", dto.CanhBaoNoShow.ToString() },
                { "SoSanToiDa", dto.SoSanToiDa.ToString() },
                { "SoSlotToiDa", dto.SoSlotToiDa.ToString() },
                { "TimeoutPhien", dto.TimeoutPhien.ToString() },
                { "NguongTonKhoThap", dto.NguongTonKhoThap.ToString() },
            };
        }
    }
}