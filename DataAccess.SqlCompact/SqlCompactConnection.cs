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
using DataAccess.Core.Data.Results;
using DataAccess.Core.Extensions;
using System.Data.SqlServerCe;
using DataAccess.SqlCompact.Linq;

namespace DataAccess.SqlCompact
{
    /// <summary>
    /// A connection for sql server
    /// </summary>
    public class SqlCompactConnection : IDataConnection
    {
        private static string _GetTableColumns;
        private static string _GetTables;
        private static string _GetViews;
        private static string _GetViewsColumns;

        static SqlCompactConnection()
        {
            Assembly asmb = Assembly.GetExecutingAssembly();
            _GetTableColumns = asmb.LoadResource("DataAccess.SqlCompact.Sql.GetTableColumns.sql");
            _GetTables = asmb.LoadResource("DataAccess.SqlCompact.Sql.GetTables.sql");
            _GetViews = asmb.LoadResource("DataAccess.SqlCompact.Sql.GetViews.sql");
            _GetViewsColumns = asmb.LoadResource("DataAccess.SqlCompact.Sql.GetViewColumns.sql");
        }

        private string _connectionString;
        private ICommandGenerator _commandGenerator;
        private IConvertToCLR _tConverter;
        private IConvertToDatastore _dConverter;

        /// <summary>
        /// Converts data on the way out that is Datastore -&gt; CLR
        /// </summary>
        public IConvertToCLR CLRConverter { get { return _tConverter; } }

        /// <summary>
        /// Coverts data on the way in that is, CLR -&gt; Datastore
        /// </summary>
        public IConvertToDatastore DatastoreConverter { get { return _dConverter; } }


        /// <summary>
        /// The command generator for this data store
        /// </summary>
        /// <value></value>
        public ICommandGenerator CommandGenerator { get { return _commandGenerator; } }

        /// <summary>
        /// the data stores escape character (left side)
        /// </summary>
        /// <value></value>
        public string LeftEscapeCharacter { get { return "["; } }

        /// <summary>
        /// the data stores escape character (right side)
        /// </summary>
        /// <value></value>
        public string RightEscapeCharacter { get { return "]"; } }

        /// <summary>
        /// The default schema for this data store
        /// </summary>
        /// <value></value>
        public string DefaultSchema { get { return ""; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerConnection"/> class with a connection string appropriate for sql server 2k8
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Catalog">The catalog.</param>
        /// <param name="User">The user.</param>
        /// <param name="Password">The password.</param>
        public SqlCompactConnection(string file)
        {
            _commandGenerator = new SqlCompactCommandGenerator();
            _connectionString = string.Format("Data Source={0};Persist Security Info=False;", file);
            _tConverter = new SqlCompactTypeConverter();
            _dConverter = new SqlCompactDBConverter();
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetConnection()
        {
            return new SqlCeConnection(_connectionString);
        }

        /// <summary>
        /// Gets a data command for this connection type
        /// </summary>
        /// <returns></returns>
        public IDbCommand GetCommand()
        {
            return new SqlCeCommand();
        }

        /// <summary>
        /// Gets a data parameter for this connection type
        /// </summary>
        /// <returns></returns>
        public IDbDataParameter GetParameter()
        {
            return new SqlCeParameter();
        }

        /// <summary>
        /// Gets a data parameter for this connection type
        /// </summary>
        /// <param name="name">The parameters name</param>
        /// <param name="value">The value of the parameter</param>
        /// <returns></returns>
        public IDbDataParameter GetParameter(string name, object value)
        {
            value = _dConverter.CoerceValue(value);
            return new SqlCeParameter(name, value);
        }

        /// <summary>
        /// Gets a data store configured for sql server
        /// </summary>
        /// <param name="connection">The connection string</param>
        /// <returns></returns>
        public static IDataStore GetDataStore(string connection)
        {
            return new SqlCompactDataStore(connection);
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
            SqlCeCommand command = new SqlCeCommand();
            command.CommandText = string.Format("TRUNCATE TABLE {0}", dstore.GetTableName(t));
            dstore.ExecuteCommand(command);
        }

        /// <summary>
        /// Inserts a list of items using SqlBulkCopy
        /// </summary>
        /// <param name="items">The items to insert</param>
        /// <param name="dstore">The data store to use when executing</param>
        public void DoBulkInsert(IList items, IDataStore dstore)
        {
            while (items.Count > 0)
                dstore.InsertObjects(items.GetSmallerList(100));
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

        /// <summary>
        /// Gets a query provider
        /// </summary>
        /// <param name="dStore">the datastore to attach it to</param>
        /// <returns></returns>
        public IQueryProvider GetQueryProvider(IDataStore dStore)
        {
            return new TSqlQueryProvider(dStore);
        }

        /// <summary>
        /// Gets a delete formatter
        /// </summary>
        /// <param name="dstore">the datastore to attach it to</param>
        /// <returns></returns>
        public IDeleteFormatter GetDeleteFormatter(IDataStore dstore)
        {
            return new DeleteTSqlFormatter(dstore);
        }

        /// <summary>
        /// Gets the tables for a datastore
        /// </summary>
        /// <param name="dstore">the datastore to fetch from</param>
        /// <returns></returns>
        public IEnumerable<DBObject> GetSchemaTables(IDataStore dstore)
        {
            IDbCommand tblCmd = GetCommand();
            tblCmd.CommandText = _GetTables;

            IDbCommand clmCmd = GetCommand();
            clmCmd.CommandText = _GetTableColumns;

            using (IQueryData objects = dstore.ExecuteCommands.ExecuteCommandQuery(tblCmd, dstore.Connection))
            using (IQueryData columns = dstore.ExecuteCommands.ExecuteCommandQuery(clmCmd, dstore.Connection))
            {
                List<IQueryRow> rows = columns.GetQueryEnumerator().ToList();
                foreach (IQueryRow o in objects)
                {
                    yield return Helpers.LoadObjectInfo(dstore, o, rows);
                }
            }
        }

        /// <summary>
        /// Gets the views for a datastore
        /// </summary>
        /// <param name="dstore">the datastore to fetch from</param>
        /// <returns></returns>
        public IEnumerable<DBObject> GetSchemaViews(IDataStore dstore)
        {
            IDbCommand tblCmd = GetCommand();
            tblCmd.CommandText = _GetViews;

            IDbCommand clmCmd = GetCommand();
            clmCmd.CommandText = _GetViewsColumns;

            using (IQueryData objects = dstore.ExecuteCommands.ExecuteCommandQuery(tblCmd, dstore.Connection))
            using (IQueryData columns = dstore.ExecuteCommands.ExecuteCommandQuery(clmCmd, dstore.Connection))
            {
                List<IQueryRow> rows = columns.GetQueryEnumerator().ToList();

                foreach (IQueryRow o in objects)
                    yield return Helpers.LoadObjectInfo(dstore, o, rows);
            }
        }
    }
}
