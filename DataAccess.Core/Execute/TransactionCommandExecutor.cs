using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Events;
using System.Data.Common;
using System.Threading.Tasks;

namespace DataAccess.Core.Execute
{
    /// <summary>
    /// Executes command within a transaction
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TransactionCommandExecutor" /> class.
    /// </remarks>
    /// <param name="info">The transacation info.</param>
    /// <param name="storeExecutor">The executor used by the data store</param>
    public class TransactionCommandExecutor(TransactionInfo info, IExecuteDatabaseCommand storeExecutor) : object(), IExecuteDatabaseCommand
    {
        public event EventHandler<Events.CommandExecutingEventArgs> CommandExecuting;
        public event EventHandler<Events.CommandExecutingEventArgs> CommandExecuted;

        protected void FireExecutingEvent(DbCommand command, IDataConnection connection, DbConnection conn)
        {
            CommandExecuting?.Invoke(this, new CommandExecutingEventArgs(command, connection, conn));
        }

        protected void FireExecutedEvent(DbCommand command, IDataConnection connection, DbConnection conn)
        {
            CommandExecuted?.Invoke(this, new CommandExecutingEventArgs(command, connection, conn));
        }

        protected async Task<T> ExecuteCommand<T>(DbCommand command, IDataConnection connection, Func<DbConnection, Task<T>> action)
        {
            command.Connection = info.Connection;
            command.Transaction = info.Transaction;

            T toReturn = default;
            foreach (IDbDataParameter parm in command.Parameters)
                connection.DatastoreConverter.CoerceValue(parm);

            InitCommand(command, info.Connection);

            try
            {
                FireExecutingEvent(command, connection, info.Connection);
                toReturn = await action(info.Connection);
                FireExecutedEvent(command, connection, info.Connection);
            }
            catch (Exception e)
            {
                throw new QueryException(e, command);
            }
            finally
            {
                command.Dispose();
            }

            return toReturn;
        }

        public virtual async Task<IQueryData> ExecuteCommandQueryAction(DbCommand command, IDataConnection connection, DbConnection r)
        {
            return await storeExecutor.ExecuteCommandQueryAction(command, connection, r);
        }

        public virtual async Task<IQueryData> ExecuteCommandQuery(DbCommand command, IDataConnection connection)
        {
            return await ExecuteCommand(command, connection, r =>
            {
                return storeExecutor.ExecuteCommandQueryAction(command, connection, r);
            });
        }

        public virtual async Task<int> ExecuteCommand(DbCommand command, IDataConnection connection)
        {
            return await ExecuteCommand(command, connection, async r =>
            {
                return await command.ExecuteNonQueryAsync();
            });
        }

        public virtual void InitCommand(DbCommand command, DbConnection conn)
        {
            storeExecutor.InitCommand(command, conn);
        }
    }
}
