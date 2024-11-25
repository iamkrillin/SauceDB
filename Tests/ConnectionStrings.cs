using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public static class ConnectionStrings
    {
        public const string SQL_SERVER = "Data Source=172.16.0.17;Initial Catalog=Sauce;User Id=AppLogin;Password=AppLogin;Encrypt=false";
        public const string MYSQL = "server=172.16.0.32;user id=root;password=changeme!;persist security info=True;database=test";
        public const string POSTGRE = "Server=172.16.0.32;Port=5432;Database=sauce;User Id=postgres;Password=changeme!;";
    }
}
