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
    }
}