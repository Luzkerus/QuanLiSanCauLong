using System;

public class ChiTietDatSanVM
{
    public string MaChiTiet { get; set; }
    public string MaPhieu { get; set; }
    public int MaSan { get; set; }
    public string TenSanCached { get; set; }
    public DateTime NgayDat { get; set; }
    public TimeSpan GioBatDau { get; set; }
    public TimeSpan GioKetThuc { get; set; }
    public decimal DonGia { get; set; }
    public decimal PhuThuLe { get; set; }
    public decimal ThanhTien { get; set; }

    // Thông tin khách hàng
    public string TenKH { get; set; }
    public string SDT { get; set; }
    public string Email { get; set; }

    public string TrangThai { get; set; } = "Chưa bắt đầu"; // hoặc dữ liệu thực tế
}
