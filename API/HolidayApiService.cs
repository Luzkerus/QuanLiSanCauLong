using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuanLiSanCauLong.API
{
    internal class HolidayApiService
    {
        private static readonly HttpClient _client = new HttpClient();
        // 🚨 THAY THẾ bằng khóa API của Abstract API của bạn
        private const string ApiKey = "90f792a61a664ccdbf005f922f263ba3";

        // 🔹 Đã cập nhật để dùng Abstract Public Holidays API
        // Endpoint này yêu cầu truy vấn theo năm, tháng, ngày. 
        // Tuy nhiên, Abstract API có thể có endpoint trả về cả năm.
        // Dưới đây là cách tiếp cận phổ biến khi dùng Abstract, lấy tất cả ngày lễ trong năm 2025:
        public static async Task<List<PublicHoliday>> GetHolidaysAsync(int year)
        {
            // Abstract API thường yêu cầu truy vấn theo tháng/ngày, nhưng chúng ta sẽ
            // dùng endpoint đơn giản nhất nếu có thể (lấy tất cả ngày lễ trong năm)
            // LƯU Ý: Endpoint này có thể chỉ hoạt động với gói cao cấp.
            // Nếu không hoạt động, bạn cần lặp qua 12 tháng.
            string url = $"https://holidays.abstractapi.com/v1/?api_key={ApiKey}&country=VN&year={year}";

            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            try
            {
                // Abstract API trả về một List/Array trực tiếp của các đối tượng Holiday
                var abstractHolidays = JsonSerializer.Deserialize<List<AbstractHoliday>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (abstractHolidays == null)
                    return new List<PublicHoliday>();

                // Ánh xạ sang mô hình PublicHoliday của bạn
                var holidays = new List<PublicHoliday>();
                foreach (var h in abstractHolidays)
                {
                    // Lọc chỉ lấy ngày lễ công cộng (Public Holiday) nếu API trả về các loại khác
                    if (h.Type.Equals("public_holiday", StringComparison.OrdinalIgnoreCase))
                    {
                        // Abstract API trả về Date, Month, Day riêng biệt
                        string dateString = $"{h.Year}-{h.Month:D2}-{h.Day:D2}";

                        holidays.Add(new PublicHoliday
                        {
                            Date = dateString,
                            Name = h.Name,
                            Description = h.Name // Abstract thường dùng trường Name làm chính
                        });
                    }
                }

                return holidays;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                return new List<PublicHoliday>();
            }
        }
    }

    // --- CÁC CLASS PHỤ ĐỂ PARSE JSON ---

    // 🔹 Class mới để phù hợp với cấu trúc JSON của Abstract API
    internal class AbstractHoliday
    {
        public string Name { get; set; }
        // Abstract API thường trả về ngày, tháng, năm dưới dạng các trường riêng biệt
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public string Type { get; set; } // Dùng để lọc loại ngày lễ
        // Các thuộc tính khác (như location, weekday, v.v.) bị bỏ qua để code gọn
    }

    // 🔹 Class PublicHoliday của bạn được giữ nguyên
    internal class PublicHoliday
    {
        public string Date { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}