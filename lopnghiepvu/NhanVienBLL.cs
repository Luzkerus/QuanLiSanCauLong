using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopTruyCapDuLieu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace QuanLiSanCauLong.LopNghiepVu
{
    public class NhanVienBLL
    {
        private readonly NhanVienDAL _dal = new NhanVienDAL();



        // ===== Thêm nhân viên =====

        // ===== Sửa nhân viên =====
        public bool SuaNhanVien(NhanVien nv)
        {
            if (string.IsNullOrWhiteSpace(nv.MaNV))
                throw new ArgumentException("Mã nhân viên không hợp lệ");

            // Không cho sửa Username, PasswordHash, NgayVaoLam
            return _dal.SuaNhanVien(nv);
        }

        // ===== Đổi mật khẩu riêng =====
        //public bool DoiMatKhau(string maNV, string newPasswordHash)
        //{
        //    if (string.IsNullOrWhiteSpace(maNV))
        //        throw new ArgumentException("Mã nhân viên không hợp lệ");
        //    if (string.IsNullOrWhiteSpace(newPasswordHash))
        //        throw new ArgumentException("Mật khẩu mới không hợp lệ");

        //    return _dal.DoiMatKhau(maNV, newPasswordHash);
        //}

        // ===== Lấy danh sách nhân viên =====
        public List<NhanVien> LayDanhSachNhanVien()
        {
            return _dal.LayTatCaNhanVien();
        }

        // ===== Tìm nhân viên theo mã =====
        //public NhanVien TimNhanVienTheoMa(string maNV)
        //{
        //    if (string.IsNullOrWhiteSpace(maNV))
        //        throw new ArgumentException("Mã nhân viên không hợp lệ");

        //    return _dal.TimNhanVienTheoMa(maNV);
        //}
        public string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        public bool ThemNhanVien(NhanVien nv)
        {
            // 1) Chọn prefix theo vai trò

            string prefix;

            if (nv.VaiTro == "Quản lý")
                prefix = "QL";
            else if (nv.VaiTro == "Full-time")
                prefix = "FT";
            else if (nv.VaiTro == "Part-time")
                prefix = "PT";
            else
                prefix = "NV";   // mặc định


            // 2) Sinh mã nhân viên
            nv.MaNV = _dal.LayMaNhanVienTiepTheo(prefix);

            // 3) Tạo username từ tên
            nv.Username = TaoUsername(nv.TenNV);

            // 4) Mật khẩu mặc định = 123456
            nv.PasswordHash = HashPassword("123456");

            return _dal.ThemNhanVien(nv);
        }
        public string TaoMaNhanVienTheoVaiTro(string vaiTro)
        {
            string prefix;
            if (vaiTro == "Quản lý")
                prefix = "QL";
            else if (vaiTro == "Full-time")
                prefix = "FT";
            else if (vaiTro == "Part-time")
                prefix = "PT";
            else
                prefix = "NV";   // mặc định
            return _dal.LayMaNhanVienTiepTheo(prefix);
        }
        public string TaoUsername(string tenNV)
        {
            if (string.IsNullOrWhiteSpace(tenNV))
                return "";

            // 1. Chuẩn hóa Unicode
            string text = tenNV.Trim().ToLower();

            // 2. Xử lý đặc biệt Đ & đ
            text = text.Replace("đ", "d");

            // 3. Loại bỏ toàn bộ dấu tiếng Việt
            text = RemoveVietnamese(text);

            // 4. Loại bỏ ký tự không phải chữ a-z, 0-9
            text = System.Text.RegularExpressions.Regex.Replace(text, "[^a-z0-9 ]", "");

            // 5. Tách họ tên → tạo dạng last.firstnames
            string[] parts = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
                return parts[0];

            string last = parts[parts.Length - 1];
            string firsts = string.Join("", parts, 0, parts.Length - 1);

            return firsts+last;
        }

        // ====== Hàm bỏ dấu tiếng Việt ======
        private string RemoveVietnamese(string text)
        {
            string normalized = text.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }

        public bool DoiMatKhau(string maNV, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(maNV))
                throw new ArgumentException("Mã nhân viên không hợp lệ");
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Mật khẩu mới không hợp lệ");
            string newPasswordHash = HashPassword(newPassword);
            return _dal.DoiMatKhau(maNV, newPasswordHash);
        }
        public bool SuaNhanVienVaMaNV(NhanVien nv, string maNVCu)
        {
            if (string.IsNullOrWhiteSpace(maNVCu))
                throw new ArgumentException("Mã nhân viên cũ không hợp lệ");
            // Không cho sửa Username, PasswordHash, NgayVaoLam
            return _dal.SuaNhanVienVaMaNV(nv, maNVCu);
        }
        public int TongSoNhanVien()
        {
            return LayDanhSachNhanVien()
                   .Count(nv => nv.VaiTro != "Admin");
        }
        public int TongSoNVPartTime()
        {
            return LayDanhSachNhanVien().Count(nv => nv.VaiTro == "Part-time");
        }
        public int TongSoNVFullTime()
        {
            return LayDanhSachNhanVien().Count(nv => nv.VaiTro == "Full-time");
        }
        public NhanVien DangNhap(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Tên đăng nhập và mật khẩu không được để trống.");
            }

            // 1. Tìm nhân viên theo Username
            NhanVien nv = _dal.TimNhanVienTheoUsername(username);

            if (nv == null)
            {
                // Không tìm thấy Username
                return null;
            }

            // 2. Mã hóa mật khẩu người dùng nhập vào để so sánh với PasswordHash đã lưu
            string inputHash = HashPassword(password);

            // 3. So sánh Hash
            if (inputHash.Equals(nv.PasswordHash, StringComparison.OrdinalIgnoreCase))
            {
                // Đăng nhập thành công
                return nv;
            }
            else
            {
                // Sai mật khẩu
                return null;
            }
        }

    }
}
