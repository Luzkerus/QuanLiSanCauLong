using QuanLiSanCauLong.LopDuLieu;
using QuanLiSanCauLong.LopTruyCapDuLieu;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiSanCauLong.LopNghiepVu
{
    public class BangGiaBLL
    {
        private readonly BangGiaDAL dal = new BangGiaDAL();

        public DataTable LayBangGiaChung()
        {
            return dal.LayBangGiaChung();
        }
        public bool ThemBangGiaMau()
        {
            return dal.ThemBangGiaMau();
        }
        public bool XoaBangGia(int maBangGia)
        {
            return dal.XoaBangGia(maBangGia);
        }
        public bool SuaBangGia(BangGiaChung bangGia)
        {
            return dal.SuaBangGia(bangGia);
        }

    }
}
