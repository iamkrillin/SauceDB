using DataAccess.SqlServer;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.SqlAzure.IDb
{
    public class SqlAzureConnection : DbConnection, IDisposable
    {
        internal SqlConnection _conn;
        public override string DataSource { get { return _conn.DataSource; } }
        public override string ConnectionString { get { return _conn.ConnectionString; } set { _conn.ConnectionString = value; } }
        public override string ServerVersion { get { return _conn.ServerVersion; } }
        public override System.Data.ConnectionState State { get { return _conn.State; } }
        public override string Database { get { return _conn.Database; } }

        public SqlAzureConnection(string connection)
        {
            _conn = new SqlConnection(connection);
        }

        protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
        {
            return _conn.BeginTransaction(isolationLevel);
        }

        public override void ChangeDatabase(string databaseName)
        {
            _conn.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            _conn.Close();
        }

        protected override DbCommand CreateDbCommand()
        {
            return new SqlAzureCommand(_conn.CreateCommand());
        }

        public override void Open()
        {
            RetryAction.RunRetryableAction(null, _conn.Open);
        }
    }
}
