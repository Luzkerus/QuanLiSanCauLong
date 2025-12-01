using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopTruyCapDuLieu;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuanLiSanCauLong.LopNghiepVu
{
    public class PhieuNhapBLL
    {
        private readonly PhieuNhapDAL phieuNhapDAL;
        private readonly ChiTietPhieuNhapDAL chiTietPhieuNhapDAL;
        private readonly HangHoaBLL hangHoaBLL;

        public PhieuNhapBLL()
        {
            phieuNhapDAL = new PhieuNhapDAL();
            chiTietPhieuNhapDAL = new ChiTietPhieuNhapDAL();
            hangHoaBLL = new HangHoaBLL();
        }

        public void LuuPhieuNhap(PhieuNhap phieu, List<ChiTietPhieuNhap> chiTiets)
        {
            // 1. Tạo số phiếu tự động
            phieu.SoPhieu = TaoSoPhieu();

            // 2. Lưu phiếu cha
            phieuNhapDAL.LuuPhieu(phieu);

            // 3. Lưu chi tiết và cập nhật/tao hàng hóa
            int stt = 1;
            foreach (var ct in chiTiets)
            {
                // Tạo mã chi tiết = SoPhieu + thứ tự
                ct.MaChiTiet = $"{phieu.SoPhieu}-{stt:000}";
                stt++;

                // Nếu hàng chưa tồn tại → thêm mới
                if (!hangHoaBLL.KiemTraTonTai(ct.MaHang))
                {
                    hangHoaBLL.ThemHangMoi(ct);
                }
                else // Cập nhật tồn kho
                {
                    hangHoaBLL.CapNhatTonKho(ct);
                }

                // Lưu chi tiết phiếu
                chiTietPhieuNhapDAL.ThemChiTiet(phieu.SoPhieu, ct);
            }
        }

        // Hàm tạo số phiếu dạng PN + yyyyMMdd + 0001
        public string TaoSoPhieu()
        {
            string prefix = "PN" + DateTime.Now.ToString("yyyyMMdd");
            string soCuoi = phieuNhapDAL.LaySoPhieuLonNhat(prefix);

            int soMoi = 1;
            if (!string.IsNullOrEmpty(soCuoi))
            {
                int num = int.Parse(soCuoi.Substring(prefix.Length));
                soMoi = num + 1;
            }

            return $"{prefix}{soMoi:0000}";
        }
        public List<PhieuNhap> LayTatCaPhieuNhap()
        {
            return phieuNhapDAL.LayTatCaPhieuNhap();
        }
        public List<PhieuNhap> LayPhieuNhapTheoNgay(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            var tatCaPhieu = phieuNhapDAL.LayTatCaPhieuNhap();
            var ketQua = new List<PhieuNhap>();
            foreach (var phieu in tatCaPhieu)
            {
                if (phieu.NgayNhap.Date >= ngayBatDau.Date && phieu.NgayNhap.Date <= ngayKetThuc.Date)
                {
                    ketQua.Add(phieu);
                }
            }
            return ketQua;
        }
        public int TatCaSoPhieuNhap()
        {
            var tatCaPhieu = phieuNhapDAL.LayTatCaPhieuNhap();
            return tatCaPhieu.Count;
        }
        public decimal TongTienTatCaPhieuNhap()
        {
            var tatCaPhieu = phieuNhapDAL.LayTatCaPhieuNhap();
            decimal tongTien = 0;
            foreach (var phieu in tatCaPhieu)
            {
                tongTien += phieu.TongTien;
            }
            return tongTien;
        }
        public decimal TongTienPhieuNhapHomNay()
        {
            var tatCaPhieu = phieuNhapDAL.LayTatCaPhieuNhap();
            decimal sum = 0;
            foreach (var phieu in tatCaPhieu)
            {
                if (phieu.NgayNhap.Date == DateTime.Now.Date)
                {
                    sum+=phieu.TongTien;
                }
            }
            return sum;
        }
        // Trong PhieuNhapBLL.cs
        // Trong PhieuNhapBLL.cs
        public List<string> LayDanhSachNhaCungCapGoiY(string nhapLieu)
        {
            // 1. Lấy tất cả các phiếu nhập
            // Nếu LayTatCaPhieuNhap() lấy dữ liệu trực tiếp từ DAL, 
            // bạn cần đảm bảo rằng nó không gây chậm nếu có quá nhiều dữ liệu.
            // Tối ưu nhất là tạo một hàm mới chỉ lấy DISTINCT NhaCungCap từ DAL.
            var tatCaPhieu = LayTatCaPhieuNhap(); // Giả sử hàm này đã được định nghĩa trong PhieuNhapBLL

            // 2. Trích xuất danh sách tên Nhà Cung Cấp duy nhất (DISTINCT)
            var tatCaNhaCungCap = tatCaPhieu
                .Select(p => p.NhaCungCap) // Chọn trường NhaCungCap
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct() // Chỉ lấy các tên duy nhất
                .ToList();

            // 3. Lọc theo ký tự đang nhập (không phân biệt hoa thường)
            var goiYList = tatCaNhaCungCap
                .Where(ncc => ncc.ToLower().Contains(nhapLieu.ToLower()))
                .OrderByDescending(ncc => ncc.ToLower().StartsWith(nhapLieu.ToLower())) // Ưu tiên các tên bắt đầu bằng ký tự nhập
                .ThenBy(ncc => ncc) // Sắp xếp theo tên
                .Take(3) // Chỉ lấy tối đa 3 kết quả
                .ToList();

            return goiYList;
        }
    }
}
