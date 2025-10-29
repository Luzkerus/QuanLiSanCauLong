using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiSanCauLong.LopTruyCapDuLieu
{
    public class ConnectStringDAL
    {
        private readonly string connectionString =
"Data Source=localhost;Initial Catalog=QuanLiSanCauLong;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";
        public string GetConnectionString()
        {
            return connectionString;
        }
    }
}
