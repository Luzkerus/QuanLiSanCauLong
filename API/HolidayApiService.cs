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

        public static async Task<List<PublicHoliday>> GetHolidaysAsync(int year)
        {
            string url = $"https://date.nager.at/api/v3/PublicHolidays/{year}/VN";

            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            var holidays = JsonSerializer.Deserialize<List<PublicHoliday>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return holidays ?? new List<PublicHoliday>();
        }
        public static async Task<bool> IsHolidayAsync(DateTime date)
        {
            // Lấy danh sách ngày lễ của năm đó
            var holidays = await GetHolidaysAsync(date.Year);

            // So sánh ngày
            foreach (var h in holidays)
            {
                if (DateTime.TryParse(h.Date, out var holidayDate))
                {
                    if (holidayDate.Date == date.Date)
                        return true;
                }
            }

            return false;
        }

    }

    internal class PublicHoliday
    {
        public string Date { get; set; }
        public string LocalName { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
    }
}
