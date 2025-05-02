using DataAccess.SqlServer;
using Microsoft.Data.SqlClient;
using System.Data.Common;

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
