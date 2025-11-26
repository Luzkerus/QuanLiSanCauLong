using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiSanCauLong.LopDuLieu
{
    public class TimeSlotVM
    {
        public string TimeRange { get; set; }          // "Ca 18:00 - 20:00"
        public string BookingsDisplay { get; set; }   // "Lượt đặt: 23"
        public int UtilPercent { get; set; }          // % công suất
        public string StatusText { get; set; }        // "Ca đông" hoặc "Ca vắng"
        public bool IsPeak { get; set; }              // true = đông, false = vắng
    }

}
