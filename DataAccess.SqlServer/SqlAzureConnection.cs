using DataAccess.SqlServer;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.SqlAzure
{
    public class SqlAzureConnection : SqlServerConnection
    {
        public SqlAzureConnection(string connectionString)
            : base(connectionString)
        {

        }

        public override DbConnection GetConnection()
        {
            return new IDb.SqlAzureConnection(_connectionString);
        }

        public override DbCommand GetCommand()
        {
            SqlCommand cmd = new SqlCommand();
            return new IDb.SqlAzureCommand(cmd);
        }
    }
}
