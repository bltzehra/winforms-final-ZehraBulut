using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace KlinikOtomasyon
{
    class SqlBaglantisi
    {
        public SqlConnection baglanti()
        {
           
            SqlConnection baglan = new SqlConnection(@"Data Source=ZEHRA;Initial Catalog=KullaniciOtomasyonDB;Integrated Security=True");
            baglan.Open();
            return baglan;
        }
    }
}