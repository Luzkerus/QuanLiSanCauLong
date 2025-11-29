namespace QuanLiSanCauLong.LopDuLieu
{
    public class CauHinhHeThong
    {
        // Thuộc tính Cấu hình Đặt sân & Cảnh báo
        public int CanhBaoNoShow { get; set; }        // Cảnh báo No-show (phút)
        public int SoSanToiDa { get; set; }           // Số sân tối đa / lần đặt
        public int SoSlotToiDa { get; set; }          // Số slot tối đa / lần đặt

        // Thuộc tính Cấu hình Bảo mật & Kho
        public int TimeoutPhien { get; set; }         // Timeout phiên đăng nhập (phút)
        public int NguongTonKhoThap { get; set; }     // Ngưỡng tồn kho thấp
    }
}