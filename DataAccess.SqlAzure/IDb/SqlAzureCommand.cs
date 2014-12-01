using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccess.SqlAzure.IDb
{
    public class SqlAzureCommand : DbCommand
    {
        private SqlCommand _cmd;

        public override bool DesignTimeVisible { get { return false; } set { } }
        public override string CommandText { get { return _cmd.CommandText; } set { _cmd.CommandText = value; } }
        public override int CommandTimeout { get { return _cmd.CommandTimeout; } set { _cmd.CommandTimeout = value; } }
        public override System.Data.CommandType CommandType { get { return _cmd.CommandType; } set { _cmd.CommandType = value; } }
        protected override DbConnection DbConnection { get { return _cmd.Connection; } set { _cmd.Connection = ((SqlAzureConnection)value)._conn; } }
        protected override DbTransaction DbTransaction { get { return _cmd.Transaction; } set { _cmd.Transaction = (SqlTransaction)value; } }
        protected override DbParameterCollection DbParameterCollection { get { return _cmd.Parameters; } }
        public override System.Data.UpdateRowSource UpdatedRowSource { get { return _cmd.UpdatedRowSource; } set { _cmd.UpdatedRowSource = value; } }
        
        public SqlAzureCommand(SqlCommand cmd)
        {
            _cmd = cmd;
        }

        public override void Cancel()
        {
            _cmd.Cancel();
        }
        
        protected override DbParameter CreateDbParameter()
        {
            return _cmd.CreateParameter();
        }

        public override void Prepare()
        {
            _cmd.Prepare();
        }

        protected override DbDataReader ExecuteDbDataReader(System.Data.CommandBehavior behavior)
        {
            return RetryAction.RunRetryableAction<DbDataReader>(this.Connection, _cmd.ExecuteReader);
        }

        public override int ExecuteNonQuery()
        {
            return RetryAction.RunRetryableAction<int>(this.Connection, _cmd.ExecuteNonQuery);
        }

        public override object ExecuteScalar()
        {
            return RetryAction.RunRetryableAction<object>(this.Connection, _cmd.ExecuteScalar);
        }
    }
}
