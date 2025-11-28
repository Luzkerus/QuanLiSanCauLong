using System;

namespace QuanLiSanCauLong.LopDuLieu
{
    public class NhanVien
    {
        public string MaNV { get; set; }            // Mã nhân viên
        public string TenNV { get; set; }           // Tên nhân viên
        public string SDT { get; set; }             // Số điện thoại
        public string Email { get; set; }           // Email
        public string Username { get; set; }        // Tài khoản đăng nhập
        public string PasswordHash { get; set; }    // Mật khẩu đã mã hoá
        public string VaiTro { get; set; }          // Vai trò (Quản lý, Full-time, Part-time)
        public DateTime NgayVaoLam { get; set; }    // Ngày vào làm
        public string TrangThai { get; set; }       // Trạng thái (Đang làm, Tạm ngưng, Đã nghỉ)
        public string GhiChu { get; set; }          // Ghi chú
    }
}
