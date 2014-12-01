using DataAccess.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.SqlAzure
{
    public class SqlAzureDataStore : SqlServerDataStore
    {
        public SqlAzureDataStore(string connectionString)
            : base(new SqlAzureConnection(connectionString))
        {
        }
    }
}
