using DataAccess.SqlServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        public override System.Data.IDbConnection GetConnection()
        {
            return new IDb.SqlAzureConnection(_connectionString);
        }

        public override System.Data.IDbCommand GetCommand()
        {
            SqlCommand cmd = new SqlCommand();
            return new IDb.SqlAzureCommand(cmd);
        }
    }
}
