// Tạo một folder mới (ví dụ: LopNghiepVu) và đặt file SessionManager.cs
using QuanLiSanCauLong.LopDuLieu;

namespace QuanLiSanCauLong.LopNghiepVu
{
    // Lớp tĩnh để quản lý phiên làm việc của người dùng
    public static class SessionManager
    {
        // Thuộc tính lưu trữ thông tin nhân viên hiện tại
        public static NhanVien CurrentUser { get; private set; }

        // Kiểm tra xem đã có người dùng đăng nhập chưa
        public static bool IsLoggedIn => CurrentUser != null;

        /// <summary>
        /// Thiết lập thông tin người dùng sau khi đăng nhập thành công
        /// </summary>
        public static void Login(NhanVien user)
        {
            // Gán đối tượng nhân viên đã được xác thực
            CurrentUser = user;
        }

        /// <summary>
        /// Xóa thông tin người dùng khi đăng xuất
        /// </summary>
        public static void Logout()
        {
            CurrentUser = null;
        }

        private const string ROLE_ADMIN = "Admin";
        private const string ROLE_QUAN_LY = "Quản lý";
        private const string ROLE_NHAN_VIEN = "Nhân viên";

        /// <summary>
        /// Kiểm tra người dùng hiện tại có vai trò Admin.
        /// </summary>
        public static bool IsAdmin => IsLoggedIn && CurrentUser.VaiTro == ROLE_ADMIN;

        /// <summary>
        /// Kiểm tra người dùng hiện tại có vai trò Quản lý.
        /// </summary>
        public static bool IsQuanLy => IsLoggedIn && CurrentUser.VaiTro == ROLE_QUAN_LY;

        /// <summary>
        /// Kiểm tra người dùng hiện tại có quyền truy cập vào màn hình Quản lý Sân (QLSan).
        /// (Chỉ Quản lý và Admin)
        /// </summary>
        public static bool CanAccessQuanLySan => IsAdmin || IsQuanLy;

        /// <summary>
        /// Kiểm tra người dùng hiện tại có quyền truy cập vào màn hình Báo Cáo.
        /// (Admin và Nhân viên)
        /// </summary>
        public static bool CanAccessBaoCao => IsAdmin || IsQuanLy;
    }
}