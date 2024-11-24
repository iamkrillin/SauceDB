using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Events;
using DataAccess.Core;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Data.Common;

namespace DataAccess.SQLite
{
    /// <summary>
    /// A generic command executor
    /// </summary>
    public class SqliteExecuteCommands : IExecuteDatabaseCommand
    {
        /// <summary>
        /// This event will fire just before a command is executed
        /// </summary>
        public event EventHandler<CommandExecutingEventArgs> CommandExecuting;

        public event EventHandler<CommandExecutingEventArgs> CommandExecuted;


        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteCommands" /> class.
        /// </summary>
        public SqliteExecuteCommands()
        {

        }

        /// <summary>
        /// Executes a command and returns the result
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="connection">The connection to use</param>
        /// <returns></returns>
        public virtual async Task<IQueryData> ExecuteCommandQuery(DbCommand command, IDataConnection connection)
        {
            return await ExecuteCommand<IQueryData>(command, connection, async () =>
            {
                return await ExecuteCommandQueryAction(command, connection, null);
            });
        }

        public async Task<IQueryData> ExecuteCommandQueryAction(DbCommand command, IDataConnection connection, DbConnection r)
        {
            SqLiteQueryData toReturn = new SqLiteQueryData();
            FireExecutingEvent(command, connection);

            using (IDataReader reader = await command.ExecuteReaderAsync())
                MapReturnData(command, toReturn, reader);

            FireExecutedEvent(command, connection);
            toReturn.QuerySuccessful = true;
            return toReturn;
        }

        /// <summary>
        /// Maps return data to the query data
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="toReturn">To return.</param>
        /// <param name="reader">The reader.</param>
        protected void MapReturnData(DbCommand command, SqLiteQueryData toReturn, IDataReader reader)
        {
            if (reader.Read())
            {
                DataTable schema = reader.GetSchemaTable();
                if (schema != null)
                {
                    for (int i = 0; i < schema.Rows.Count; i++)
                    {
                        toReturn.AddFieldMapping(schema.Rows[i]["ColumnName"].ToString(), i);
                    }
                }

                AddRecord(toReturn, reader);

                while (reader.Read())
                    AddRecord(toReturn, reader);
            }
        }

        /// <summary>
        /// Adds a record to the result set
        /// </summary>
        protected static void AddRecord(SqLiteQueryData toReturn, IDataReader reader)
        {
            object[] values = new object[reader.FieldCount];
            reader.GetValues(values);
            toReturn.AddRowData(values);
        }

        /// <summary>
        /// Fires an  event.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="connection">The connection.</param>
        protected void FireExecutingEvent(DbCommand command, IDataConnection connection)
        {
            CommandExecuting?.Invoke(this, new CommandExecutingEventArgs(command, connection));
        }

        protected void FireExecutedEvent(DbCommand command, IDataConnection connection)
        {
            CommandExecuted?.Invoke(this, new CommandExecutingEventArgs(command, connection));
        }

        /// <summary>
        /// Executes a command on the data store
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="connection">The connection to use</param>
        public virtual async Task<int> ExecuteCommand(DbCommand command, IDataConnection connection)
        {
            return await ExecuteCommand<int>(command, connection, async () =>
            {
                return await command.ExecuteNonQueryAsync();
            });
        }

        protected virtual async Task<T> ExecuteCommand<T>(DbCommand command, IDataConnection connection, Func<Task<T>> action)
        {
            T toReturn = default;
            foreach (IDbDataParameter parm in command.Parameters)
                connection.DatastoreConverter.CoerceValue(parm);

            using (DbConnection conn = connection.GetConnection())
            {
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                command.Connection = conn;
                command.CommandTimeout = 10000;

                try
                {
                    toReturn = await action();
                }
                catch (Exception e)
                {
                    throw new QueryException(e, command);
                }
                finally
                {
                    command.Dispose();
                    conn.Close();
                }
            }

            return toReturn;
        }

        public void InitCommand(DbCommand command, DbConnection conn)
        {
        }
    }
}
