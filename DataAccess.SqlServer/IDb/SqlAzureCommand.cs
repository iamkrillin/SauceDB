using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace DataAccess.SqlAzure.IDb
{
    public class SqlAzureCommand : DbCommand
    {
        public SqlCommand Command { get; set; }
        public override bool DesignTimeVisible { get { return false; } set { } }
        public override string CommandText { get { return Command.CommandText; } set { Command.CommandText = value; } }
        public override int CommandTimeout { get { return Command.CommandTimeout; } set { Command.CommandTimeout = value; } }
        public override System.Data.CommandType CommandType { get { return Command.CommandType; } set { Command.CommandType = value; } }
        protected override DbConnection DbConnection { get { return Command.Connection; } set { Command.Connection = ((SqlAzureConnection)value).Connection; } }
        protected override DbTransaction DbTransaction { get { return Command.Transaction; } set { Command.Transaction = (SqlTransaction)value; } }
        protected override DbParameterCollection DbParameterCollection { get { return Command.Parameters; } }
        public override System.Data.UpdateRowSource UpdatedRowSource { get { return Command.UpdatedRowSource; } set { Command.UpdatedRowSource = value; } }

        public SqlAzureCommand(SqlCommand cmd)
        {
            Command = cmd;
        }

        public override void Cancel()
        {
            Command.Cancel();
        }

        protected override DbParameter CreateDbParameter()
        {
            return Command.CreateParameter();
        }

        public override void Prepare()
        {
            Command.Prepare();
        }

        protected override DbDataReader ExecuteDbDataReader(System.Data.CommandBehavior behavior)
        {
            return RetryAction.RunRetryableAction<DbDataReader>(this.Connection, Command.ExecuteReader);
        }

        public override int ExecuteNonQuery()
        {
            return RetryAction.RunRetryableAction<int>(this.Connection, Command.ExecuteNonQuery);
        }

        public override object ExecuteScalar()
        {
            return RetryAction.RunRetryableAction<object>(this.Connection, Command.ExecuteScalar);
        }

        public static explicit operator SqlCommand(SqlAzureCommand item)
        {
            return item.Command;
        }
    }
}
