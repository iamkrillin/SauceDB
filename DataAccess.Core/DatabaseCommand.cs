using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

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
        public virtual void SetCommandText(string cmd)
        {
            Command.CommandText = cmd;
        }

        public IDbCommand GetPrepared()
        {
            return Command;
        }

        /// <summary>
        /// Sets the command text for execution
        /// </summary>
        /// <param name="cmd">The command text</param>
        /// <param name="FieldMarker">This marker in the command text will be replaced with the field list</param>
        /// <param name="TableNameMarker">This marker in the command text will be replaced with the table name</param>
        /// <typeparam name="T">The type to pull info from</typeparam>
        public virtual void SetCommandText(string cmd, string FieldMarker, string TableNameMarker)
        {
            Command.CommandText = cmd.Replace(FieldMarker, DataStore.GetSelectList<T>()).Replace(TableNameMarker, DataStore.GetTableName<T>());
        }

        /// <summary>
        /// Sets the parameters on the object based on a parameter object
        /// </summary>
        /// <param name="parameters"></param>
        public virtual void SetParameters(object parameters)
        {
            Command.Parameters.Clear();
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
                    AddCommandParameter(v.FieldName, v.Getter(parameters, null));
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
        public virtual int ExecuteCommand()
        {
            Command.CommandType = CommandType.Text;
            return DataStore.ExecuteCommand(Command);
        }

        /// <summary>
        /// Executes the current command and gets a list of objects
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> ExecuteCommandGetList()
        {
            Command.CommandType = CommandType.Text;
            return DataStore.ExecuteCommandLoadList<T>(Command);
        }

        /// <summary>
        /// Executes the current command and gets the first item
        /// </summary>
        /// <returns></returns>
        public virtual T ExecuteCommandGetObject()
        {
            Command.CommandType = CommandType.Text;
            return DataStore.ExecuteCommandLoadList<T>(Command).FirstOrDefault();
        }

        /// <summary>
        /// Executes the current command as a stored procedure and gets a list of items
        /// </summary>
        /// <returns></returns>
        public virtual List<T> ExecuteAsStoredProcedureGetList()
        {
            Command.CommandType = CommandType.StoredProcedure;
            return DataStore.ExecuteCommandLoadList<T>(Command).ToList();
        }

        /// <summary>
        /// Executes the current command as a stored procedure and get the first item
        /// </summary>
        /// <returns></returns>
        public virtual T ExecuteAsStoredProcedureGetObject()
        {
            Command.CommandType = CommandType.StoredProcedure;
            return DataStore.ExecuteCommandLoadList<T>(Command).FirstOrDefault();
        }

        /// <summary>
        /// Executes a command as stored procedure
        /// </summary>
        /// <param name="command">The command text</param>
        /// <param name="parameters">The parameters</param>
        /// <returns></returns>
        public virtual List<T> ExecuteStoredProcedure(string command, object parameters)
        {
            SetCommandText(command);
            SetParameters(parameters);
            return ExecuteAsStoredProcedureGetList();
        }

        /// <summary>
        /// Executes a command as stored procedure
        /// </summary>
        /// <param name="command">The command text</param>
        /// <param name="parameters">The parameters</param>
        /// <returns></returns>
        public virtual T ExecuteStoredProcedureGetObject(string command, object parameters)
        {
            SetCommandText(command);
            SetParameters(parameters);
            return ExecuteAsStoredProcedureGetObject();
        }

        /// <summary>
        /// Executes a command as stored procedure
        /// </summary>
        /// <param name="command">The command text</param>
        /// <returns></returns>
        public virtual List<T> ExecuteStoredProcedure(string command)
        {
            SetCommandText(command);
            Command.Parameters.Clear();
            return ExecuteAsStoredProcedureGetList();
        }

        /// <summary>
        /// Executes a command as stored procedure
        /// </summary>
        /// <param name="command">The command text</param>
        /// <returns></returns>
        public virtual T ExecuteStoredProcedureGetObject(string command)
        {
            SetCommandText(command);
            Command.Parameters.Clear();
            return ExecuteAsStoredProcedureGetObject();
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="command">The command text</param>
        /// <param name="parameters">The parameters</param>
        /// <returns></returns>
        public virtual IEnumerable<T> ExecuteQuery(string command, object parameters)
        {
            SetCommandText(command);
            SetParameters(parameters);
            return ExecuteCommandGetList();
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="command">The command text</param>
        /// <param name="parameters">The parameters</param>
        /// <returns></returns>
        public virtual T ExecuteQueryGetObject(string command, object parameters)
        {
            SetCommandText(command);
            SetParameters(parameters);
            return ExecuteCommandGetObject();
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="command">The command text</param>
        /// <returns></returns>
        public virtual IEnumerable<T> ExecuteQuery(string command)
        {
            SetCommandText(command);
            Command.Parameters.Clear();
            return ExecuteCommandGetList();
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="command">The command text</param>
        /// <returns></returns>
        public virtual T ExecuteQueryGetObject(string command)
        {
            SetCommandText(command);
            Command.Parameters.Clear();
            return ExecuteCommandGetObject();
        }

        /// <summary>
        /// Executues a DB command and reutrns the rows affected as reported by the datastore
        /// </summary>
        /// <param name="command">The command text</param>
        /// <returns></returns>
        public virtual int ExecuteDBCommand(string command)
        {
            SetCommandText(command);
            return DataStore.ExecuteCommand(this.Command);
        }

        /// <summary>
        /// Executues a DB command and reutrns the rows affected as reported by the datastore
        /// </summary>
        /// /// <param name="command">The command text</param>
        /// <param name="parameters">The parameters</param>
        /// <returns></returns>
        public virtual int ExecuteDBCommand(string command, object parameters)
        {
            SetCommandText(command);
            SetParameters(parameters);
            return DataStore.ExecuteCommand(this.Command);
        }
    }
}
