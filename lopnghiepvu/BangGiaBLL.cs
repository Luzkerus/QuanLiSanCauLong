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

        public decimal TinhDonGia(DateTime ngayDat, TimeSpan gioBatDau, TimeSpan gioKetThuc)
        {
            var bangGia = LayBangGiaChung();
            if (bangGia.Rows.Count == 0) return 0;

            bool ngayLe = IsHoliday(ngayDat);

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

            decimal donGia = (decimal)khung["DonGia"]; // đơn giá theo giờ
            decimal phuThu = 0;

            if (ngayLe && khung["PhuThuLePercent"] != DBNull.Value)
            {
                decimal phuThuPercent = (decimal)khung["PhuThuLePercent"];
                phuThu = donGia * phuThuPercent / 100;
            }

            decimal tongGio = (decimal)(gioKetThuc - gioBatDau).TotalHours; // số giờ đặt
            decimal tongTien = donGia  * tongGio;

            return tongTien;
        }
        public decimal TinhPhuThu(DateTime ngayDat, TimeSpan gioBatDau, TimeSpan gioKetThuc)
        {
            var bangGia = LayBangGiaChung();
            if (bangGia.Rows.Count == 0) return 0;
            bool ngayLe = IsHoliday(ngayDat);
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
            decimal donGia = (decimal)khung["DonGia"]; // đơn giá theo giờ
            decimal phuThu = 0;
            if (ngayLe && khung["PhuThuLePercent"] != DBNull.Value)
            {
                decimal phuThuPercent = (decimal)khung["PhuThuLePercent"];
                phuThu = donGia * phuThuPercent / 100;
            }
            decimal tongGio = (decimal)(gioKetThuc - gioBatDau).TotalHours; // số giờ đặt
            decimal tongPhuThu = phuThu * tongGio;
            return tongPhuThu;
        }
        public decimal TinhTongTien(DateTime ngayDat, TimeSpan gioBatDau, TimeSpan gioKetThuc)
        {
            decimal donGia = TinhDonGia(ngayDat, gioBatDau, gioKetThuc);
            decimal phuThu = TinhPhuThu(ngayDat, gioBatDau, gioKetThuc);
            return donGia + phuThu;
        }
        private bool IsHoliday(DateTime date)
        {
            // Thí dụ: thứ 7, CN hoặc ngày lễ
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}
