using System;

public class KhachHang
{
    public string SDT { get; set; }
    public String SDTPhu { get; set; }
    public string Ten { get; set; }
    public string Email { get; set; }
    public int LuotChoi { get; set; }
    public decimal TongChiTieu { get; set; }
    public DateTime TuNgay { get; set; }
    public int DiemTichLuy { get; set; }

    public string LoaiKhach => LuotChoi > 10 ? "Hội viên" : "Vãng lai";
}
