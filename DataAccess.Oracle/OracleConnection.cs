using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using DataAccess.Core;
using System.Collections;
using DataAccess.Core.Linq;
using DataAccess.Core.Linq.Mapping;
using System.Reflection;
using System.Data.OracleClient;

namespace DataAccess.Oracle
{
    /// <summary>
    /// A connection for sql server
    /// </summary>
    public class OracleConnection : IDataConnection
    {
        private static string _GetTableColumns;
        private static string _GetTables;
        private static string _GetViews;

        static OracleConnection()
        {
            _GetTableColumns = "select column_id, column_name, table_name, data_type from all_tab_columns where table_name = '{0}'";
            _GetTables = "select table_name as \"Table\" from user_tables";
            _GetViews = "select view_name as \"Table\" from user_views";
        }

        private string _connectionString;
        private ICommandGenerator _commandGenerator;
        private ITypeConverter _tConverter;        

        /// <summary>
        /// The type converter for this data store
        /// </summary>
        /// <value></value>
        public ITypeConverter TypeConverter { get { return _tConverter; } }

        /// <summary>
        /// The command generator for this data store
        /// </summary>
        /// <value></value>
        public ICommandGenerator CommandGenerator { get { return _commandGenerator; } }

        /// <summary>
        /// the data stores escape character (left side)
        /// </summary>
        /// <value></value>
        public string LeftEscapeCharacter { get { return "\""; } }

        /// <summary>
        /// the data stores escape character (right side)
        /// </summary>
        /// <value></value>
        public string RightEscapeCharacter { get { return "\""; } }

        /// <summary>
        /// The default schema for this data store
        /// </summary>
        /// <value></value>
        public string DefaultSchema { get { return ""; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerConnection"/> class with a connection string appropriate for sql server 2k8
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="User">The user.</param>
        /// <param name="Password">The password.</param>
        public OracleConnection(string Server, string User, string Password)
            : this(string.Format("Data Source={0};User Id={1};Password={2};Integrated Security=no;", Server, User, Password))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerConnection"/> class.
        /// </summary>
        /// <param name="ConnectionString">The connection string.</param>
        public OracleConnection(string ConnectionString)
        {
            _commandGenerator = new OracleCommandGenerator();
            _connectionString = ConnectionString;
            _tConverter = new OracleTypeConverter();
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetConnection()
        {
            return new System.Data.OracleClient.OracleConnection(_connectionString);
        }

        /// <summary>
        /// Gets a data command for this connection type
        /// </summary>
        /// <returns></returns>
        public IDbCommand GetCommand()
        {
            return new System.Data.OracleClient.OracleCommand();
        }

        /// <summary>
        /// Gets a data parameter for this connection type
        /// </summary>
        /// <returns></returns>
        public IDbDataParameter GetParameter()
        {
            return new System.Data.OracleClient.OracleParameter();
        }

        /// <summary>
        /// Gets a data parameter for this connection type
        /// </summary>
        /// <param name="name">The parameters name</param>
        /// <param name="value">The value of the parameter</param>
        /// <returns></returns>
        public IDbDataParameter GetParameter(string name, object value)
        {
            return new System.Data.OracleClient.OracleParameter(name, value);
        }

        /// <summary>
        /// Gets a data store configured for sql server
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Catalog">The catalog.</param>
        /// <param name="User">The user.</param>
        /// <param name="Password">The password.</param>
        /// <returns></returns>
        public static IDataStore GetDataStore(string Server, string User, string Password)
        {
            return new OracleDataStore(Server, User, Password);
        }

        /// <summary>
        /// Gets a data store configured for sql server
        /// </summary>
        /// <param name="connection">The connection string</param>
        /// <returns></returns>
        public static IDataStore GetDataStore(string connection)
        {
            return new OracleDataStore(connection);
        }

        /// <summary>
        /// Will execute a command to 'truncate' a table
        /// </summary>
        /// <typeparam name="T">The type to truncate</typeparam>
        /// <param name="dstore">The data store to use when executing</param>
        public void TruncateTable<T>(IDataStore dstore)
        {
            TruncateTable(typeof(T), dstore);
        }

        /// <summary>
        /// Will execute a command to 'truncate' a table
        /// </summary>
        /// <param name="t">The type to truncate</param>
        /// <param name="dstore">The data store to use when executing</param>
        public void TruncateTable(Type t, IDataStore dstore)
        {
            OracleCommand command = new OracleCommand();
            command.CommandText = string.Format("TRUNCATE TABLE {0}", dstore.GetTableName(t));
            dstore.ExecuteCommand(command);
        }

        private void AddColumnsToTable(DataTable dt, TypeInfo ti, DBObject table)
        {
            foreach (Column c in table.Columns)
            {
                DataFieldInfo fi = ti.DataFields.Where(R => R.FieldName.Equals(c.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                Type target = fi.PropertyType;
                if (target.IsGenericType && target.GetGenericTypeDefinition() == typeof(Nullable<>))
                    target = target.GetGenericArguments()[0];

                dt.Columns.Add(new DataColumn(fi.FieldName, target) { AllowDBNull = true });
            }
        }

        public IQueryProvider GetQueryProvider(IDataStore dStore)
        {
            throw new NotSupportedException();
            //return new TSqlQueryProvider(dStore);
        }

        public IDeleteFormatter GetDeleteFormatter(IDataStore dstore)
        {
            throw new NotSupportedException();
            //return new DeleteTSqlFormatter(dstore);
        }

        public List<DBObject> GetSchemaTables(IDataStore dstore)
        {
            List<DBObject> toReturn = new List<DBObject>();
            OracleCommand getColumns = new OracleCommand();
            OracleCommand getTables = new OracleCommand();
            getTables.CommandText = _GetTables;

            QueryData tables = dstore.ExecuteComamands.ExecuteCommandQuery(getTables, dstore.Connection);
            for (int i = 0; i < tables.RowCount; i++)
            {
                DBObject t = new DBObject();
                t.Name = tables.GetDataForRowField(0, i).ToString();
                t.Columns = new List<Column>();

                getColumns.CommandText = string.Format(_GetTableColumns, t.Name);
                QueryData columns = dstore.ExecuteComamands.ExecuteCommandQuery(getColumns, dstore.Connection);

                for (int j = 0; j < columns.RowCount; j++)
                {
                    t.Columns.Add(new Column()
                    {
                        Name = columns.GetDataForRowField("column_name", j).ToString(),
                        DataType = columns.GetDataForRowField("data_type", j).ToString(),
                        IsPrimaryKey = false
                    });
                }
                toReturn.Add(t);
            }

            return toReturn;
        }

        public List<DBObject> GetSchemaViews(IDataStore dstore)
        {
            List<DBObject> toReturn = new List<DBObject>();
            OracleCommand getColumns = new OracleCommand();
            OracleCommand getTables = new OracleCommand();
            getTables.CommandText = _GetViews;

            QueryData tables = dstore.ExecuteComamands.ExecuteCommandQuery(getTables, dstore.Connection);
            for (int i = 0; i < tables.RowCount; i++)
            {
                DBObject t = new DBObject();
                t.Name = tables.GetDataForRowField(0, i).ToString();
                t.Columns = new List<Column>();

                getColumns.CommandText = string.Format(_GetTableColumns, t.Name);
                QueryData columns = dstore.ExecuteComamands.ExecuteCommandQuery(getColumns, dstore.Connection);

                for (int j = 0; j < columns.RowCount; j++)
                {
                    t.Columns.Add(new Column()
                    {
                        Name = columns.GetDataForRowField("column_name", j).ToString(),
                        DataType = columns.GetDataForRowField("data_type", j).ToString(),
                        IsPrimaryKey = false
                    });
                }
                toReturn.Add(t);
            }

            return toReturn;
        }


        public void DoBulkInsert(IList items, IDataStore dstore)
        {
            throw new NotImplementedException();
        }
    }
}
