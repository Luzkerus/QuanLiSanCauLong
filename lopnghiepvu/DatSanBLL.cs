using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopTruyCapDuLieu;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace QuanLiSanCauLong.LopNghiepVu
{
    public class DatSanBLL
    {
        private DatSanDAL dal = new DatSanDAL();

        // Sinh mã phiếu: PD + yyyyMMddHHmmss
        private string SinhMaPhieu()
        {
            string ngayHienTai = DateTime.Now.ToString("yyyyMMdd");

            // Lấy mã phiếu cao nhất trong ngày từ database
            string maPhieuCuoi = dal.LayMaPhieuCuoiTrongNgay(ngayHienTai);

            int soThuTu = 1; // mặc định nếu chưa có đơn nào
            if (!string.IsNullOrEmpty(maPhieuCuoi) && maPhieuCuoi.Length > 11)
            {
                // Cắt phần thứ tự XXX cuối mã phiếu
                string soThuTuStr = maPhieuCuoi.Substring(maPhieuCuoi.Length - 3);
                if (int.TryParse(soThuTuStr, out int stt))
                {
                    soThuTu = stt + 1;
                }
            }

            // Trả về mã phiếu mới dạng PDyyyymmddXXX
            return $"PD{ngayHienTai}{soThuTu:D3}";
        }


        public bool TaoDon(string sdt, List<ChiTietDatSan> chiTiets)
        {
            if (chiTiets == null || chiTiets.Count == 0)
                return false;

            // Tạo DatSan
            var datSan = new DatSan
            {
                MaPhieu = SinhMaPhieu(),
                SDT = sdt,
                NgayTao = DateTime.Now
            };

            // Gán MaPhieu và MaChiTiet cho chi tiết
            for (int i = 0; i < chiTiets.Count; i++)
            {
                chiTiets[i].MaPhieu = datSan.MaPhieu;
                chiTiets[i].MaChiTiet = datSan.MaPhieu + (i + 1).ToString("D2");
            }

            // Tính tổng tiền
            datSan.TongTien = 0;
            foreach (var c in chiTiets)
                datSan.TongTien += c.ThanhTien;

            // Lưu vào database
            return dal.LuuDatSan(datSan, chiTiets);
        }
        public List<ChiTietDatSanVM> LayTatCaDatSan()
        {
            return dal.LayTatCaDatSan();
        }
        public List<ChiTietDatSanVM> LayDatSanGanDay()
        {
            return dal.LayTop15DatSan();
        }
        public int DemTongSoDatSanHomNay()
        {
            DateTime today = DateTime.Today;
            return LayTatCaDatSan()
                   .Count(x => x.NgayDat.Date == today);
        }
        public double TinhTyLeLapDay()
        {
            DateTime today = DateTime.Today;
            var datSansHomNay = LayTatCaDatSan()
                                .Where(x => x.NgayDat.Date == today)
                                .ToList();
            if (datSansHomNay.Count == 0)
                return 0.0;
            int soGioDat = 0;
            foreach (var ds in datSansHomNay)
            {
                soGioDat += (int)(ds.GioKetThuc - ds.GioBatDau).TotalHours;
            }
            // Giả sử mỗi sân có thể được đặt tối đa 10 giờ mỗi ngày
            int tongSoGioCoTheDat = 18 * datSansHomNay.Select(x => x.MaSan).Distinct().Count();
            return (double)soGioDat / tongSoGioCoTheDat * 100.0;
        }
        public double TinhBienDongSoLuotDatSan()
        {
            DateTime today = DateTime.Today;
            DateTime yesterday = today.AddDays(-1);
            int soLuotDatHomNay = LayTatCaDatSan()
                                 .Count(x => x.NgayDat.Date == today);
            int soLuotDatHomQua = LayTatCaDatSan()
                                  .Count(x => x.NgayDat.Date == yesterday);
            if (soLuotDatHomQua == 0)
                return soLuotDatHomNay > 0 ? 100.0 : 0.0;
            return ((double)(soLuotDatHomNay - soLuotDatHomQua) / soLuotDatHomQua) * 100.0;
        }
        public double TinhBienDongTyLeLapDay()
        {
            DateTime today = DateTime.Today;
            DateTime yesterday = today.AddDays(-1);

            // Tỷ lệ lấp đầy hôm nay
            double tyLeHomNay = TinhTyLeLapDay();

            // Tỷ lệ lấp đầy hôm qua
            var datSansHomQua = LayTatCaDatSan()
                                .Where(x => x.NgayDat.Date == yesterday)
                                .ToList();
            double tyLeHomQua = 0.0;
            if (datSansHomQua.Count > 0)
            {
                int soGioDat = 0;
                foreach (var ds in datSansHomQua)
                {
                    soGioDat += (int)(ds.GioKetThuc - ds.GioBatDau).TotalHours;
                }
                int tongSoGioCoTheDat = 18 * datSansHomQua.Select(x => x.MaSan).Distinct().Count();
                tyLeHomQua = (double)soGioDat / tongSoGioCoTheDat * 100.0;
            }

            // Nếu hôm qua không có dữ liệu
            if (tyLeHomQua == 0)
                return tyLeHomNay > 0 ? 100.0 : 0.0;

            // Tính biến động %
            return ((tyLeHomNay - tyLeHomQua) / tyLeHomQua) * 100.0;
        }
        public int TinhSoLuotDatSanTuNgayDenNgay(DateTime fromDate, DateTime toDate)
        {
            return LayTatCaDatSan()
                   .Count(x => x.NgayDat.Date >= fromDate.Date && x.NgayDat.Date <= toDate.Date);
        }
        public List<TimeSlotVM> LayBaoCaoTheoKhungGio(DateTime fromDate, DateTime toDate)
        {
            var datSans = LayTatCaDatSan()
                          .Where(x => x.NgayDat.Date >= fromDate.Date && x.NgayDat.Date <= toDate.Date)
                          .ToList();

            // Lấy khung giờ từ bảng giá
            var bangGiaBLL = new BangGiaBLL();
            var bangGia = bangGiaBLL.LayBangGiaChung();

            var result = new List<TimeSlotVM>();

            foreach (DataRow row in bangGia.Rows)
            {
                var bd = (TimeSpan)row["GioBatDau"];
                var kt = (TimeSpan)row["GioKetThuc"];

                // Gom nhóm theo khung giờ
                var bookings = datSans.Count(ds => ds.GioBatDau >= bd && ds.GioKetThuc <= kt);
                var doanhThu = datSans.Where(ds => ds.GioBatDau >= bd && ds.GioKetThuc <= kt)
                                      .Sum(ds => ds.ThanhTien);

                // Giả sử công suất tối đa mỗi ca là 30 lượt
                int utilPercent = (int)((double)bookings / 30 * 100);

                result.Add(new TimeSlotVM
                {
                    TimeRange = $"Ca {bd:hh\\:mm} - {kt:hh\\:mm}",
                    BookingsDisplay = $"Lượt đặt: {bookings}",
                    UtilPercent = utilPercent,
                    StatusText = utilPercent >= 50 ? "Ca đông" : "Ca vắng",
                    IsPeak = utilPercent >= 50
                });
            }

            // Sắp xếp: ca đông trước, ca vắng sau
            return result.OrderByDescending(x => x.IsPeak)
                         .ThenByDescending(x => x.UtilPercent)
                         .ToList();
        }
        public double TinhSoGioTuNgayDenNgay(DateTime fromDate, DateTime toDate)
        {
            var datSans = LayTatCaDatSan()
                          .Where(x => x.NgayDat.Date >= fromDate.Date && x.NgayDat.Date <= toDate.Date)
                          .ToList();
            double tongGio = 0;
            foreach (var ds in datSans)
            {
                tongGio += (double)(ds.GioKetThuc - ds.GioBatDau).TotalHours;
            }
            return tongGio;
        }
        public decimal TinhGiaTrungBinhMotGio(DateTime fromDate, DateTime toDate)
        {
            var datSans = LayTatCaDatSan()
                          .Where(x => x.NgayDat.Date >= fromDate.Date && x.NgayDat.Date <= toDate.Date)
                          .ToList();
            decimal tongTien = 0;
            double tongGio = 0;
            foreach (var ds in datSans)
            {
                tongTien += ds.ThanhTien;
                tongGio += (double)(ds.GioKetThuc - ds.GioBatDau).TotalHours;
            }
            if (tongGio == 0)
                return 0;
            return tongTien / (decimal)tongGio;
        }

    }
}
