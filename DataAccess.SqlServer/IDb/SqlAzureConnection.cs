using Microsoft.Data.SqlClient;
using System;
using System.Data.Common;

namespace DataAccess.SqlAzure.IDb
{
    public class SqlAzureConnection : DbConnection, IDisposable
    {
        public SqlConnection Connection { get; set; }
        public override string DataSource { get { return Connection.DataSource; } }
        public override string ConnectionString { get { return Connection.ConnectionString; } set { Connection.ConnectionString = value; } }
        public override string ServerVersion { get { return Connection.ServerVersion; } }
        public override System.Data.ConnectionState State { get { return Connection.State; } }
        public override string Database { get { return Connection.Database; } }

        public SqlAzureConnection(string connection)
        {
            Connection = new SqlConnection(connection);
        }

        protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
        {
            return Connection.BeginTransaction(isolationLevel);
        }

        public override void ChangeDatabase(string databaseName)
        {
            Connection.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            Connection.Close();
        }

        protected override DbCommand CreateDbCommand()
        {
            return new SqlAzureCommand(Connection.CreateCommand());
        }

        public override void Open()
        {
            RetryAction.RunRetryableAction(null, Connection.Open);
        }

        public static explicit operator SqlConnection(SqlAzureConnection item)
        {
            return item.Connection;
        }
    }
}
