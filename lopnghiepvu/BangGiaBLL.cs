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

    }
}
