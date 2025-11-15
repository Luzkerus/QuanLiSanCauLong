using System;

public class ChiTietDatSan
{
    public string MaChiTiet { get; set; }     // VD: PD2025111400101
    public string MaPhieu { get; set; }
    public int MaSan { get; set; }
    public string TenSanCached { get; set; }
    public DateTime NgayDat { get; set; }
    public TimeSpan GioBatDau { get; set; }
    public TimeSpan GioKetThuc { get; set; }
    public decimal DonGia { get; set; }
    public decimal PhuThuLe { get; set; }
    public decimal ThanhTien { get; set; }
    public string TrangThai { get; set; }
}
