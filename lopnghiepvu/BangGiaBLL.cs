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
    }
}
