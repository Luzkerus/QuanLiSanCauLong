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
        private const string ApiKey = "Cl7KudnNkmgAPa53oazN5hIS59Hi2BXo"; // 🔸 Thay bằng key của bạn

        public static async Task<List<PublicHoliday>> GetHolidaysAsync(int year)
        {
            string url = $"https://calendarific.com/api/v2/holidays?api_key={ApiKey}&country=VN&year={year}";

            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            var root = JsonSerializer.Deserialize<CalendarificResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (root?.Response?.Holidays == null)
                return new List<PublicHoliday>();

            var holidays = new List<PublicHoliday>();
            foreach (var holidayList in root.Response.Holidays.Values)
            {
                foreach (var h in holidayList)
                {
                    holidays.Add(new PublicHoliday
                    {
                        Date = h.Date?.Iso,
                        Name = h.Name,
                        Description = h.Description
                    });
                }
            }

            return holidays;
        }
    }

    // 🔹 Các class phụ để parse JSON
    internal class CalendarificResponse
    {
        public CalendarificResponseData Response { get; set; }
    }

    internal class CalendarificResponseData
    {
        public Dictionary<string, List<CalendarificHoliday>> Holidays { get; set; }
    }

    internal class CalendarificHoliday
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public CalendarificDate Date { get; set; }
    }

    internal class CalendarificDate
    {
        public string Iso { get; set; }
    }

    internal class PublicHoliday
    {
        public string Date { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
