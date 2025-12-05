**PHẦN MỀM QUẢN LÝ SÂN CẦU LÔNG CỦ CHI**
*Ứng dụng WPF – C# – SQL Server*

## **1. Giới thiệu chung**
Phần mềm *Quản Lý Sân Cầu Lông Củ Chi* được xây dựng để hỗ trợ tự động hóa các nghiệp vụ vận hành sân cầu lông như đặt sân, quản lý sân, quản lý nhân viên, kho – POS, thanh toán và báo cáo.
Hệ thống hướng đến tính dễ sử dụng, độ chính xác cao và khả năng mở rộng trong tương lai.
Dự án được thiết kế theo mô hình **3 lớp (3-Layer Architecture)**:
* **Presentation Layer (WPF)**
* **Business Logic Layer (BLL)**
* **Data Access Layer (DAL)**

## **2. Công nghệ sử dụng**

| Thành phần        | Công nghệ            |
| ----------------- | -------------------- |
| Giao diện         | WPF (.NET Framework) |
| Ngôn ngữ          | C#                   |
| Cơ sở dữ liệu     | SQL Server           |
| Kiến trúc         | 3-layer              |
| Báo cáo & Hóa đơn | PD/ Excel  |
| Quản lý mã nguồn  | Git / GitHub         |

## **3. Các chức năng chính**
### **3.1. Quản lý sân**
* Theo dõi trạng thái sân theo thời gian thực
* Cấu hình giá theo khung giờ và loại ngày
* Quản lý lịch bảo trì sân
### **3.2. Đặt sân – Check-in – Check-out**
* Tạo/sửa/hủy đơn đặt sân
* Tự động kiểm tra trùng giờ
* Tính tiền theo thời gian thực
* Auto No-show: hủy sau 15 phút không check-in
### **3.3. POS nước uống – Thuê vợt**
* Bán nước uống, trừ tồn kho
* Thuê – trả vợt, phụ phí
* Gộp chung với hóa đơn sân
### **3.4. Quản lý nhân viên**
* Thêm/sửa/xóa nhân viên
* Phân quyền theo vai trò
* Phân ca làm việc
### **3.5. Báo cáo**
* Doanh thu theo ngày/tuần/tháng
* Tỷ lệ lấp đầy sân
* Báo cáo kho

## **4. Cấu trúc thư mục**

```
QuanLiSanCauLong/
│── LopTrinhBay/                # UI WPF
│── LopNghiepVu/                # Business (BLL)
│── LopTruyCapDuLieu/           # Data Access (DAL)
│── LopDuLieu/                  # DTO
│── TaiNguyen/                  # Hình ảnh, logo...
│── API/                        # Tích hợp (nếu có)
│── app.config                  # Chuỗi kết nối SQL
└── README.md                   # Tài liệu mô tả
```

# **5. Hướng dẫn cài đặt **
# **Hướng dẫn cấu hình kết nối SQL Server lần đầu**
Khi mở phần mềm lần đầu, hệ thống sẽ yêu cầu người dùng thiết lập chuỗi kết nối SQL Server.
Các bước thực hiện:
### **Bước 1 – Nhập thông tin server**
* **Tên server**:
  * Nếu dùng SQL mặc định:
    ```
    .\SQLEXPRESS
    ```
  * Nếu dùng SQL local:
    ```
    localhost
    ```
### **Bước 2 – Nhập tên database**
```
QuanLiSanCauLong
```
### **Bước 3 – Chọn phương thức đăng nhập**
✔ Nếu sử dụng Windows Authentication → tick vào *“Không cần nhập User/Password”*
✘ Nếu dùng SQL Authentication → nhập:
* User SQL
* Password
### **Bước 4 – Kiểm tra kết nối**
* Nhấn **Test kết nối**
* Nếu thành công → thông báo *“Kết nối thành công”*

# **6. Thông tin đăng nhập mặc định**
| Vai trò   | Tài khoản | Mật khẩu |
| --------- | --------- | -------- |
| **Admin** | `Admin`   | `123456` |

Tài khoản Admin dùng để:
* Quản trị hệ thống
* Cấu hình giá sân
* Quản lý nhân viên
* Xem báo cáo

# **7. Nhóm thực hiện**
* **Nguyễn Tường Vy – 52300273**
* **Đỗ Huỳnh Thiên Tài – 52300251**

