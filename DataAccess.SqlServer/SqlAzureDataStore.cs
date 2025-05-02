using DataAccess.SqlServer;

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
