using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;
using System.Data;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Events;
using System.Data.Common;
using System.Threading.Tasks;

namespace DataAccess.Core.Execute
{
    /// <summary>
    /// A generic command executor
    /// </summary>
    public class ExecuteCommands : IExecuteDatabaseCommand
    {
        /// <summary>
        /// This event will fire just before a command is executed
        /// </summary>
        public event EventHandler<CommandExecutingEventArgs> CommandExecuting;
        public event EventHandler<CommandExecutingEventArgs> CommandExecuted;

        /// <summary>
        /// Executes a command and returns the result
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="connection">The connection to use</param>
        /// <returns></returns>
        public virtual async Task<IQueryData> ExecuteCommandQuery(DbCommand command, IDataConnection connection)
        {
            return await ExecuteCommand(command, connection, async r =>
            {
                return await ExecuteCommandQueryAction(command, connection, r);
            });
        }

        public virtual async Task<IQueryData> ExecuteCommandQueryAction(DbCommand command, IDataConnection connection, DbConnection r)
        {
            QueryData toReturn = new QueryData(r, command);
            return toReturn;
        }

        private static async Task CloseConnection(DbConnection r)
        {
            if (r != null)
            {
                await r.CloseAsync();
                await r.DisposeAsync();
            }
        }

        /// <summary>
        /// Fires an  event.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="connection">The connection.</param>
        protected void FireExecutingEvent(DbCommand command, IDataConnection connection, DbConnection conn)
        {
            CommandExecuting?.Invoke(this, new CommandExecutingEventArgs(command, connection, conn));
        }

        protected void FireExecutedEvent(DbCommand command, IDataConnection connection, DbConnection conn)
        {
            CommandExecuted?.Invoke(this, new CommandExecutingEventArgs(command, connection, conn));
        }

        /// <summary>
        /// Executes a command on the data store
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="connection">The connection to use</param>
        public virtual async Task<int> ExecuteCommand(DbCommand command, IDataConnection connection)
        {
            return await ExecuteCommand(command, connection, async r =>
            {
                var result = await command.ExecuteNonQueryAsync();
                await CloseConnection(r);

                return result;
            });
        }

        protected virtual async Task<T> ExecuteCommand<T>(DbCommand command, IDataConnection connection, Func<DbConnection, Task<T>> action)
        {
            T toReturn = default;
            foreach (IDbDataParameter parm in command.Parameters)
                connection.DatastoreConverter.CoerceValue(parm);

            DbConnection conn = await OpenConnection(connection);
            InitCommand(command, conn);

            try
            {
                FireExecutingEvent(command, connection, conn);
                toReturn = await action(conn);
                FireExecutedEvent(command, connection, conn);
            }
            catch (Exception e)
            {
                if (conn.State == ConnectionState.Open)
                {
                    await command.DisposeAsync();
                    await conn.CloseAsync();
                }

                throw new QueryException(e, command);
            }

            return toReturn;
        }

        public virtual async Task<DbConnection> OpenConnection(IDataConnection connection)
        {
            DbConnection conn = connection.GetConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            return conn;
        }

        public virtual void InitCommand(DbCommand command, DbConnection conn)
        {
            command.Connection = conn;
            command.CommandTimeout = 10000;
        }
    }
}
