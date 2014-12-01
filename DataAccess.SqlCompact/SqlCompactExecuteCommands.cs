using DataAccess.Core;
using DataAccess.Core.Events;
using DataAccess.Core.Execute;
using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DataAccess.SqlCompact
{
    public class SqlCompactExecuteCommands : IExecuteDatabaseCommand
    {
         private Dictionary<string, Dictionary<string, int>> _mappings = new Dictionary<string, Dictionary<string, int>>();

        /// <summary>
        /// This event will fire just before a command is executed
        /// </summary>
        public event EventHandler<CommandExecutingEventArgs> CommandExecuting;

        public event EventHandler<CommandExecutingEventArgs> CommandExecuted;


        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteCommands" /> class.
        /// </summary>
        public SqlCompactExecuteCommands()
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
            return ExecuteCommand<IQueryData>(command, connection, () =>
                {
                    return ExecuteCommandQueryAction(command, connection, null);
                });
        }

        public IQueryData ExecuteCommandQueryAction(IDbCommand command, IDataConnection connection, IDbConnection r)
        {
            SqlCompactQueryData toReturn = new SqlCompactQueryData();
            using (IDataReader reader = command.ExecuteReader())
                MapReturnData(command, toReturn, reader);

            toReturn.QuerySuccessful = true;
            return toReturn;
        }

        /// <summary>
        /// Maps return data to the query data
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="toReturn">To return.</param>
        /// <param name="reader">The reader.</param>
        protected void MapReturnData(IDbCommand command, SqlCompactQueryData toReturn, IDataReader reader)
        {
            if (reader.Read())
            {
                if (!_mappings.ContainsKey(command.CommandText))
                {
                    _mappings[command.CommandText] = new Dictionary<string, int>();
                    DataTable schema = reader.GetSchemaTable();
                    if (schema != null)
                    {
                        for (int i = 0; i < schema.Rows.Count; i++)
                        {
                            toReturn.AddFieldMapping(schema.Rows[i]["ColumnName"].ToString(), i);
                        }
                    }

                    _mappings[command.CommandText] = toReturn.GetMappings();
                }

                toReturn.SetFieldMappings(_mappings[command.CommandText]);
                AddRecord(toReturn, reader);

                while (reader.Read())
                    AddRecord(toReturn, reader);
            }
        }

        /// <summary>
        /// Adds a record to the result set
        /// </summary>
        protected static void AddRecord(SqlCompactQueryData toReturn, IDataReader reader)
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
            return ExecuteCommand<int>(command, connection, () =>
                {
                    return command.ExecuteNonQuery();
                });
        }

        protected virtual T ExecuteCommand<T>(IDbCommand command, IDataConnection connection, Func<T> action)
        {
            T toReturn = default(T);
            foreach (IDbDataParameter parm in command.Parameters)
                connection.DatastoreConverter.CoerceValue(parm);

            using (IDbConnection conn = connection.GetConnection())
            {
                if(conn.State != ConnectionState.Open)
                    conn.Open();
                
                command.Connection = conn;

                try
                {
                    FireExecutingEvent(command, connection, conn);
                    toReturn = action();
                    FireExecutedEvent(command, connection, conn);
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


        public void InitCommand(IDbCommand command, IDbConnection conn)
        {
            command.Connection = conn;
        }
    }
}
