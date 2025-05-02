using DataAccess.Core;
using DataAccess.Core.Data;
using DataAccess.Core.Extensions;
using DataAccess.Core.Interfaces;
using DataAccess.SqlServer.Linq;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DataAccess.SqlServer
{
    /// <summary>
    /// A connection for sql server
    /// </summary>
    public class SqlServerConnection : IDataConnection
    {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        protected static string _GetTableColumns;
        protected static string _GetTables;
        protected static string _GetViews;
        protected static string _GetViewsColumns;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        static SqlServerConnection()
        {
            Assembly asmb = Assembly.GetExecutingAssembly();
            _GetTableColumns = asmb.LoadResource("DataAccess.SqlServer.Sql.GetTableColumns.sql");
            _GetTables = asmb.LoadResource("DataAccess.SqlServer.Sql.GetTables.sql");
            _GetViews = asmb.LoadResource("DataAccess.SqlServer.Sql.GetViews.sql");
            _GetViewsColumns = asmb.LoadResource("DataAccess.SqlServer.Sql.GetViewColumns.sql");
        }

        protected string _connectionString;
        protected ICommandGenerator _commandGenerator;
        protected IConvertToCLR _tConverter;
        protected IConvertToDatastore _dConverter;

        /// <summary>
        /// Converts data on the way out that is Datastore -&gt; CLR
        /// </summary>
        public IConvertToCLR CLRConverter => _tConverter;

        /// <summary>
        /// Coverts data on the way in that is, CLR -&gt; Datastore
        /// </summary>
        public IConvertToDatastore DatastoreConverter => _dConverter;

        /// <summary>
        /// The command generator for this data store
        /// </summary>
        /// <value></value>
        public ICommandGenerator CommandGenerator => _commandGenerator;

        /// <summary>
        /// the data stores escape character (left side)
        /// </summary>
        /// <value></value>
        public string LeftEscapeCharacter => "[";

        /// <summary>
        /// the data stores escape character (right side)
        /// </summary>
        /// <value></value>
        public string RightEscapeCharacter => "]";

        /// <summary>
        /// The default schema for this data store
        /// </summary>
        /// <value></value>
        public string DefaultSchema => "dbo";

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerConnection"/> class with a connection string appropriate for sql server 2k8
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Catalog">The catalog.</param>
        /// <param name="User">The user.</param>
        /// <param name="Password">The password.</param>
        public SqlServerConnection(string Server, string Catalog, string User, string Password)
            : this($"Data Source={Server};Initial Catalog={Catalog};User Id={User};Password={Password};Encrypt=false")
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerConnection"/> class.
        /// </summary>
        /// <param name="ConnectionString">The connection string.</param>
        public SqlServerConnection(string ConnectionString)
        {
            _commandGenerator = new SqlServerCommandGenerator(this);
            _connectionString = ConnectionString;
            _tConverter = new SqlServerTypeConverter();
            _dConverter = new SqlServerDBConverter();
        }

        /// <summary>
        /// will use windows auth
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="Catalog"></param>
        public SqlServerConnection(string Server, string Catalog)
            : this($"Data Source={Server};Initial Catalog={Catalog};Integrated Security=true;Encrypt=false")
        {

        }

        /// <summary>
        /// Will use windows auth
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="Catalog"></param>
        /// <returns></returns>
        public static IDataStore GetDataStore(string Server, string Catalog)
        {
            return new SqlServerDataStore(Server, Catalog);
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        public virtual DbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        /// <summary>
        /// Gets a data command for this connection type
        /// </summary>
        /// <returns></returns>
        public virtual DbCommand GetCommand()
        {
            return new SqlCommand();
        }

        /// <summary>
        /// Gets a data parameter for this connection type
        /// </summary>
        /// <returns></returns>
        public IDbDataParameter GetParameter()
        {
            return new SqlParameter();
        }

        /// <summary>
        /// Gets a data parameter for this connection type
        /// </summary>
        /// <param name="name">The parameters name</param>
        /// <param name="value">The value of the parameter</param>
        /// <returns></returns>
        public IDbDataParameter GetParameter(string name, object value)
        {
            SqlParameter parm = new SqlParameter();

            if (value == null)
            {
                parm.ParameterName = name;
                parm.Value = DBNull.Value;
            }
            else
            {
                parm.Value = value;
                parm.ParameterName = name;

                if (value is DataTable)
                    parm.SqlDbType = SqlDbType.Structured;
            }

            return parm;
        }

        /// <summary>
        /// Gets a data store configured for sql server
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Catalog">The catalog.</param>
        /// <param name="User">The user.</param>
        /// <param name="Password">The password.</param>
        /// <returns></returns>
        public static IDataStore GetDataStore(string Server, string Catalog, string User, string Password)
        {
            return new SqlServerDataStore(Server, Catalog, User, Password);
        }

        /// <summary>
        /// Gets a data store configured for sql server
        /// </summary>
        /// <param name="connection">The connection string</param>
        /// <returns></returns>
        public static IDataStore GetDataStore(string connection)
        {
            return new SqlServerDataStore(connection);
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
            SqlCommand command = new SqlCommand();
            command.CommandText = $"TRUNCATE TABLE {dstore.GetTableName(t)}";
            dstore.ExecuteCommand(command);
        }

        /// <summary>
        /// Inserts a list of items using SqlBulkCopy
        /// </summary>
        /// <param name="items">The items to insert</param>
        /// <param name="dstore">The data store to use when executing</param>
        public async Task DoBulkInsert(IList items, IDataStore dstore)
        {
            ArrayList toUse = [];
            toUse.AddRange(items);
            await DoBulkCopy(toUse, dstore);
        }

        private async Task DoBulkCopy(ArrayList items, IDataStore dstore)
        {
            if (items.Count > 0)
            {
                Type t = items[0].GetType();
                DatabaseTypeInfo ti = await dstore.TypeParser.GetTypeInfo(t);
                DBObject table = dstore.SchemaValidator.TableValidator.GetObjects().Where(R => R.Schema.Equals(ti.UnEscapedSchema, StringComparison.InvariantCultureIgnoreCase) && R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                if (table != null)
                {
                    while (items.Count > 0)
                    {
                        ArrayList list = [];
                        while (items.Count > 0)
                        {
                            list.Add(items[0]);
                            items.RemoveAt(0);

                            if (list.Count > 20000)
                            {
                                await ProcessBulkInsert(dstore, list, t, ti, table);
                                list.Clear();
                            }
                        }

                        if (list.Count > 0)
                            await ProcessBulkInsert(dstore, list, t, ti, table);
                    }
                }
                else
                {
                    throw new DataStoreException("Target Table not found");
                }
            }
        }

        private async Task ProcessBulkInsert(IDataStore dstore, IList list, Type t, DatabaseTypeInfo ti, DBObject table)
        {
            DataTable dt = new DataTable();
            AddColumnsToTable(dt, ti, table);

            foreach (object record in list)
            {
                DataRow dr = dt.NewRow();
                foreach (DataFieldInfo dfi in ti.DataFields)
                {
                    object item = dfi.Getter(record);
                    if (item == null)
                        dr[dfi.FieldName] = DBNull.Value;
                    else
                        dr[dfi.FieldName] = item;
                }

                dt.Rows.Add(dr);
            }

            await WriteToServer(dt, await this.CommandGenerator.ResolveTableName(dstore.TypeParser, t));
        }

        private async Task WriteToServer(DataTable dt, string table)
        {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_connectionString, SqlBulkCopyOptions.KeepNulls))
            {
                bulkCopy.BulkCopyTimeout = Int32.MaxValue;
                bulkCopy.DestinationTableName = table;
                await bulkCopy.WriteToServerAsync(dt);
            }
        }

        private static void AddColumnsToTable(DataTable dt, DatabaseTypeInfo ti, DBObject table)
        {
            foreach (Column c in table.Columns)
            {
                DataFieldInfo fi = ti.DataFields.Where(R => R.FieldName.Equals(c.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                if (fi != null)
                {
                    Type target = fi.PropertyType;
                    if (target.IsGenericType && target.GetGenericTypeDefinition() == typeof(Nullable<>))
                        target = target.GetGenericArguments()[0];

                    dt.Columns.Add(new DataColumn(fi.FieldName, target) { AllowDBNull = true });
                }
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
        public async IAsyncEnumerable<DBObject> GetSchemaTables(IDataStore dstore)
        {
            DbCommand tblCmd = GetCommand();
            tblCmd.CommandText = _GetTables;

            DbCommand clmCmd = GetCommand();
            clmCmd.CommandText = _GetTableColumns;

            using IQueryData objects = await dstore.ExecuteCommands.ExecuteCommandQuery(tblCmd, dstore.Connection);
            using IQueryData columns = await dstore.ExecuteCommands.ExecuteCommandQuery(clmCmd, dstore.Connection);
            List<IQueryRow> rows = columns.GetQueryEnumerator().ToList();
            foreach (IQueryRow o in objects)
            {
                yield return await Helpers.LoadObjectInfo(dstore, o, rows);
            }
        }

        /// <summary>
        /// Gets the views for a datastore
        /// </summary>
        /// <param name="dstore">the datastore to fetch from</param>
        /// <returns></returns>
        public async IAsyncEnumerable<DBObject> GetSchemaViews(IDataStore dstore)
        {
            DbCommand tblCmd = GetCommand();
            tblCmd.CommandText = _GetViews;

            DbCommand clmCmd = GetCommand();
            clmCmd.CommandText = _GetViewsColumns;

            using IQueryData objects = await dstore.ExecuteCommands.ExecuteCommandQuery(tblCmd, dstore.Connection);
            using IQueryData columns = await dstore.ExecuteCommands.ExecuteCommandQuery(clmCmd, dstore.Connection);
            List<IQueryRow> rows = columns.GetQueryEnumerator().ToList();

            foreach (IQueryRow o in objects)
                yield return await Helpers.LoadObjectInfo(dstore, o, rows);
        }
    }
}
