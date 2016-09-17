using DataAccess.Core.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Core
{
    /// <summary>
    /// A Helper class for when you need to deal directly with the database
    /// </summary>
    public class DatabaseCommand<T>
    {
        protected IDbCommand Command;
        public IDataStore DataStore { get; set; }

        public DatabaseCommand(IDataStore dstore)
        {
            DataStore = dstore;
            Command = DataStore.Connection.GetCommand();
        }

        /// <summary>
        /// Sets the command text for execution
        /// </summary>
        /// <param name="cmd">The command text</param>
        protected virtual void SetCommandText(string cmd)
        {
            Command.CommandText = cmd;
        }

        protected IDbCommand GetPrepared()
        {
            return Command;
        }

        /// <summary>
        /// Sets the parameters on the object based on a parameter object
        /// </summary>
        /// <param name="parameters"></param>
        protected virtual void SetParameters(object parameters)
        {
            Command.Parameters.Clear();

            if (parameters != null)
            {
                DatabaseTypeInfo ti = DataStore.TypeInformationParser.GetTypeInfo(parameters.GetType(), false);
                if (ti.IsDynamic)
                {
                    IDictionary<string, object> items = (IDictionary<string, object>)parameters;
                    foreach (string s in items.Keys)
                        AddCommandParameter(s, items[s]);
                }
                else
                {
                    foreach (var v in ti.DataFields)
                        AddCommandParameter(v.FieldName, v.Getter(parameters));
                }
            }
        }

        protected virtual void AddCommandParameter(string key, object value)
        {
            Command.Parameters.Add(DataStore.Connection.GetParameter(key, value));
        }

        /// <summary>
        /// Executes a command and return the number of rows updated
        /// </summary>
        /// <returns></returns>
        protected virtual Task<int> ExecuteCommand()
        {
            Command.CommandType = CommandType.Text;
            return DataStore.ExecuteCommand(Command);
        }

        /// <summary>
        /// Executes the current command and gets a list of objects
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<T>> ExecuteCommandGetList()
        {
            Command.CommandType = CommandType.Text;
            return await DataStore.ExecuteCommandLoadList<T>(Command);
        }

        /// <summary>
        /// Executes the current command as a stored procedure and gets a list of items
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<List<T>> ExecuteAsStoredProcedureGetList()
        {
            Command.CommandType = CommandType.StoredProcedure;
            return (await DataStore.ExecuteCommandLoadList<T>(Command)).ToList();
        }

        /// <summary>
        /// Executes a command as stored procedure
        /// </summary>
        /// <param name="command">The command text</param>
        /// <param name="parameters">The parameters</param>
        /// <returns></returns>
        public virtual Task<List<T>> ExecuteStoredProcedure(string command, object parameters = null)
        {
            SetCommandText(command);
            SetParameters(parameters);
            return ExecuteAsStoredProcedureGetList();
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="command">The command text</param>
        /// <param name="parameters">The parameters</param>
        /// <returns></returns>
        public virtual Task<IEnumerable<T>> ExecuteQuery(string command, object parameters = null)
        {
            SetCommandText(command);
            SetParameters(parameters);
            return ExecuteCommandGetList();
        }

        /// <summary>
        /// Executues a DB command and returns the rows affected as reported by the datastore
        /// </summary>
        /// /// <param name="command">The command text</param>
        /// <param name="parameters">The parameters</param>
        /// <returns></returns>
        public virtual Task<int> ExecuteCommand(string command, object parameters = null)
        {
            SetCommandText(command);
            SetParameters(parameters);
            return DataStore.ExecuteCommand(this.Command);
        }
    }
}
