using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopTruyCapDuLieu;
using System;

namespace QuanLiSanCauLong.LopNghiepVu
{
    public class CauHinhHeThongBLL
    {
        // Khởi tạo đối tượng DAL để tương tác với cơ sở dữ liệu
        private readonly CauHinhHeThongDAL _dal = new CauHinhHeThongDAL();

        /// <summary>
        /// Lấy toàn bộ cấu hình hệ thống hiện tại.
        /// </summary>
        /// <returns>Đối tượng CauHinhHeThong.</returns>
        public CauHinhHeThong LayCauHinhHeThong()
        {
            try
            {
                return _dal.LayCauHinh();
            }
            catch (Exception ex)
            {
                // Ghi log lỗi và trả về cấu hình mặc định hoặc ném lỗi tùy vào yêu cầu
                // Trong trường hợp này, ta trả về đối tượng trống để tránh crash UI
                Console.WriteLine($"Lỗi BLL khi tải cấu hình: {ex.Message}");
                return new CauHinhHeThong();
            }
        }

        /// <summary>
        /// Lưu cấu hình hệ thống mới vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="cauHinh">Đối tượng cấu hình cần lưu.</param>
        /// <returns>True nếu lưu thành công.</returns>
        public bool LuuCauHinhHeThong(CauHinhHeThong cauHinh)
        {
            // --- 1. Xác thực nghiệp vụ ---

            // Kiểm tra các giá trị phải lớn hơn hoặc bằng 0
            if (cauHinh.CanhBaoNoShow < 0 || cauHinh.SoSanToiDa <= 0 ||
                cauHinh.SoSlotToiDa <= 0 || cauHinh.TimeoutPhien <= 0 ||
                cauHinh.NguongTonKhoThap < 0)
            {
                throw new ArgumentException("Các tham số cấu hình phải là số dương và hợp lệ.");
            }

            // Kiểm tra các ràng buộc logic khác (nếu có)
            if (cauHinh.SoSanToiDa > 10) // Ví dụ: giới hạn số sân tối đa là 10
            {
                throw new ArgumentException("Số sân tối đa cho phép đặt không được vượt quá 10.");
            }

            // --- 2. Gọi DAL để lưu dữ liệu ---
            try
            {
                return _dal.LuuCauHinh(cauHinh);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi và ném ra lỗi để UI xử lý
                Console.WriteLine($"Lỗi BLL khi lưu cấu hình: {ex.Message}");
                throw new Exception("Lỗi hệ thống khi lưu cấu hình. Vui lòng kiểm tra kết nối CSDL.", ex);
            }
        }
    }
}