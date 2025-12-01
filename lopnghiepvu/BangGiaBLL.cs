using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopTruyCapDuLieu;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiSanCauLong.LopNghiepVu
{
    public class BangGiaBLL
    {
        private readonly BangGiaDAL dal = new BangGiaDAL();

        public DataTable LayBangGiaChung()
        {
            return dal.LayBangGiaChung();
        }
        public bool ThemBangGiaMau()
        {
            return dal.ThemBangGiaMau();
        }
        public bool XoaBangGia(int maBangGia)
        {
            return dal.XoaBangGia(maBangGia);
        }
        public bool SuaBangGia(BangGiaChung bangGia)
        {
            return dal.SuaBangGia(bangGia);
        }
        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday ||
                   date.DayOfWeek == DayOfWeek.Sunday;
        }

        public decimal TinhDonGia(DateTime ngayDat, TimeSpan gioBatDau, TimeSpan gioKetThuc)
        {
            var bangGia = LayBangGiaChung();
            if (bangGia.Rows.Count == 0) return 0;

            bool ngayLe = IsHoliday(ngayDat);
            bool cuoiTuan = IsWeekend(ngayDat);

            DataRow khung = null;

            foreach (DataRow row in bangGia.Rows)
            {
                var bd = (TimeSpan)row["GioBatDau"];
                var kt = (TimeSpan)row["GioKetThuc"];

                // Chỉ chọn khung giờ phù hợp
                if (gioBatDau >= bd && gioKetThuc <= kt)
                {
                    // Nếu là ngày lễ → bỏ qua loại ngày
                    if (ngayLe)
                    {
                        khung = row;
                        break;
                    }

                    // Không phải ngày lễ → so loại ngày
                    string loaiNgay = row["LoaiNgay"].ToString();

                    if (!cuoiTuan && loaiNgay == "Thứ 2-Thứ 6")
                    {
                        khung = row;
                        break;
                    }

                    if (cuoiTuan && loaiNgay == "Cuối Tuần")
                    {
                        khung = row;
                        break;
                    }
                }
            }

            if (khung == null) return 0;

            decimal donGia = (decimal)khung["DonGia"];
            decimal tongGio = (decimal)(gioKetThuc - gioBatDau).TotalHours;

            return donGia * tongGio;
        }

        public decimal TinhPhuThu(DateTime ngayDat, TimeSpan gioBatDau, TimeSpan gioKetThuc)
        {
            var bangGia = LayBangGiaChung();
            if (bangGia.Rows.Count == 0) return 0;

            bool ngayLe = IsHoliday(ngayDat);
            if (!ngayLe) return 0; // chỉ tính cho ngày lễ

            DataRow khung = null;

            foreach (DataRow row in bangGia.Rows)
            {
                var bd = (TimeSpan)row["GioBatDau"];
                var kt = (TimeSpan)row["GioKetThuc"];

                if (gioBatDau >= bd && gioKetThuc <= kt)
                {
                    khung = row;
                    break;
                }
            }

            if (khung == null) return 0;

            decimal donGia = (decimal)khung["DonGia"];

            if (khung["PhuThuLePercent"] == DBNull.Value)
                return 0;

            decimal phuThuPercent = (decimal)khung["PhuThuLePercent"];
            decimal phuThuMoiGio = donGia * phuThuPercent / 100;

            decimal tongGio = (decimal)(gioKetThuc - gioBatDau).TotalHours;

            return phuThuMoiGio * tongGio;
        }

        public decimal TinhTongTien(DateTime ngayDat, TimeSpan gioBatDau, TimeSpan gioKetThuc)
        {
            decimal donGia = TinhDonGia(ngayDat, gioBatDau, gioKetThuc);
            decimal phuThu = TinhPhuThu(ngayDat, gioBatDau, gioKetThuc);
            return donGia + phuThu;
        }
        private bool IsHoliday(DateTime date)
        {
            // Danh sách ngày lễ cố định
                var holidays = new List<DateTime>
        {
            new DateTime(date.Year, 1, 1),   // Tết Dương Lịch
            new DateTime(date.Year, 4, 30),  // 30/4
            new DateTime(date.Year, 5, 1),   // Quốc tế lao động
            new DateTime(date.Year, 9, 2)    // Quốc khánh
        };

            return holidays.Any(d => d.Date == date.Date);
        }
        private static List<DateTime> GetFixedHolidays(int year)
        {
            var holidays = new List<DateTime>
            {
                // Tết Dương lịch (1 ngày)
                new DateTime(year, 1, 1), 

                // Giải phóng miền Nam (1 ngày)
                new DateTime(year, 4, 30), 

                // Quốc tế Lao động (1 ngày)
                new DateTime(year, 5, 1), 
                
                // Quốc khánh (2 ngày: 2/9 và ngày liền kề theo quy định) - Chỉ lấy 2/9
                new DateTime(year, 9, 2) 

                // ⚠️ Nếu muốn thêm Giỗ Tổ Hùng Vương (10/3 Âm lịch) và Tết Nguyên Đán,
                // bạn CẦN sử dụng một thư viện Lịch Âm để chuyển đổi.
                // Ví dụ: new DateTime(year, 4, 21) // Giả sử Giỗ Tổ năm 2025
            };

            // Nếu bạn không dùng API, bạn có thể thêm các ngày nghỉ bù cuối tuần vào đây nếu cần.
            return holidays;
        }

        public async Task<bool> IsHolidayAsync(DateTime date)
        {
            try
            {
                // 🔹 ƯU TIÊN 1: Gọi API
                var holidays = await QuanLiSanCauLong.API.HolidayApiService.GetHolidaysAsync(date.Year);

                // 🔹 So sánh ngày hiện tại với ngày lễ từ API
                return holidays.Any(h =>
                {
                    if (DateTime.TryParse(h.Date, out var holidayDate))
                        return holidayDate.Date == date.Date;
                    return false;
                });
            }
            catch (Exception ex)
            {
                // 🔹 FALLBACK: Nếu API lỗi, sử dụng danh sách cố định đã tạo
                Console.WriteLine($"API Error: {ex.Message}. Falling back to fixed list.");

                // Lấy danh sách ngày lễ cố định của năm đó
                var fixedHolidays = GetFixedHolidays(date.Year);

                // Kiểm tra xem ngày có trùng với bất kỳ ngày nào trong danh sách cố định không
                return fixedHolidays.Any(h => h.Date == date.Date);
            }
        }

    }
}
