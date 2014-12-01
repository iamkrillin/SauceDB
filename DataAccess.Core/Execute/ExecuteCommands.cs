using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;
using System.Data;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Events;

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
        /// Initializes a new instance of the <see cref="ExecuteCommands" /> class.
        /// </summary>
        public ExecuteCommands()
        {

        }

        /// <summary>
        /// Executes a command and returns the result
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="connection">The connection to use</param>
        /// <returns></returns>
        public virtual IQueryData ExecuteCommandQuery(IDbCommand command, IDataConnection connection)
        {
            return ExecuteCommand<IQueryData>(command, connection, r =>
                {
                    return ExecuteCommandQueryAction(command, connection, r);
                });
        }

        public virtual IQueryData ExecuteCommandQueryAction(IDbCommand command, IDataConnection connection, IDbConnection r)
        {
            QueryData toReturn = new QueryData(r, command);
            return toReturn;
        }

        private static void CloseConnection(IDbConnection r)
        {
            if (r != null)
            {
                r.Close();
                r.Dispose();
            }
        }

        /// <summary>
        /// Fires an  event.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="connection">The connection.</param>
        protected void FireExecutingEvent(IDbCommand command, IDataConnection connection, IDbConnection conn)
        {
            if (CommandExecuting != null) CommandExecuting(this, new CommandExecutingEventArgs(command, connection, conn));
        }

        protected void FireExecutedEvent(IDbCommand command, IDataConnection connection, IDbConnection conn)
        {
            if (CommandExecuted != null) CommandExecuted(this, new CommandExecutingEventArgs(command, connection, conn));
        }

        /// <summary>
        /// Executes a command on the data store
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="connection">The connection to use</param>
        public virtual int ExecuteCommand(IDbCommand command, IDataConnection connection)
        {
            return ExecuteCommand<int>(command, connection, r =>
                {
                    var result = command.ExecuteNonQuery();
                    CloseConnection(r);

                    return result;
                });
        }

        protected virtual T ExecuteCommand<T>(IDbCommand command, IDataConnection connection, Func<IDbConnection, T> action)
        {
            T toReturn = default(T);
            foreach (IDbDataParameter parm in command.Parameters)
                connection.DatastoreConverter.CoerceValue(parm);

            IDbConnection conn = OpenConnection(connection);
            InitCommand(command, conn);

            try
            {
                FireExecutingEvent(command, connection, conn);
                toReturn = action(conn);
                FireExecutedEvent(command, connection, conn);
            }
            catch (Exception e)
            {
                if (conn.State == ConnectionState.Open)
                {
                    command.Dispose();
                    conn.Close();
                }
                throw new QueryException(e, command);
            }
            return toReturn;
        }

        public virtual IDbConnection OpenConnection(IDataConnection connection)
        {
            IDbConnection conn = connection.GetConnection();
            if (conn.State != ConnectionState.Open)
                conn.Open();
            return conn;
        }

        public virtual void InitCommand(IDbCommand command, IDbConnection conn)
        {
            command.Connection = conn;
            command.CommandTimeout = 10000;
        }
    }
}
