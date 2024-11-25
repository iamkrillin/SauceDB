using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using System.Data;
using DataAccess.Core;
using Npgsql;
using DataAccess.Postgre.Linq;
using DataAccess.Core.ObjectValidators;
using System.Collections;
using DataAccess.Core.Data;
using System.Reflection;
using DataAccess.Core.Conversion;
using DataAccess.Core.Data.Results;
using DataAccess.Core.Extensions;
using System.Data.Common;
using System.Threading.Tasks;

namespace DataAccess.PostgreSQL
{
    /// <summary>
    /// A connection to postgre
    /// </summary>
    public class PostgreSQLServerConnection : IDataConnection
    {
        private static string _getColumnsCommand;
        private static string _getTablesCommand;
        private static string _getViewCommand;
        private static string _getViewColumnsCommand;

        static PostgreSQLServerConnection()
        {
            Assembly asmb = Assembly.GetExecutingAssembly();
            _getColumnsCommand = asmb.LoadResource("DataAccess.PostgreSQL.Sql.GetTableColumns.sql");
            _getTablesCommand = asmb.LoadResource("DataAccess.PostgreSQL.Sql.GetTables.sql");
            _getViewCommand = asmb.LoadResource("DataAccess.PostgreSQL.Sql.GetViews.sql");
            _getViewColumnsCommand = asmb.LoadResource("DataAccess.PostgreSQL.Sql.GetViewColumns.sql");
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
        /// Initializes a new instance of the <see cref="PostgreSQLServerConnection"/> class.
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Catalog">The catalog.</param>
        /// <param name="User">The user.</param>
        /// <param name="Password">The password.</param>
        public PostgreSQLServerConnection(string Server, string Catalog, string User, string Password)
            : this(string.Format("Server={0};Port=5432;Database={1};User Id={2};Password={3}", Server, Catalog, User, Password))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSQLServerConnection"/> class.
        /// </summary>
        /// <param name="ConnectionString">The connection string.</param>
        public PostgreSQLServerConnection(string ConnectionString)
        {
            _commandGenerator = new PostgreCommandGenerator(this);
            _connectionString = ConnectionString;
            _tConverter = new StandardCLRConverter();
            _dConverter = new PostgreDBConverter();
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        public DbConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        /// <summary>
        /// Gets a data command for this connection type
        /// </summary>
        /// <returns></returns>
        public DbCommand GetCommand()
        {
            return new NpgsqlCommand();
        }

        /// <summary>
        /// Gets a data parameter for this connection type
        /// </summary>
        /// <returns></returns>
        public IDbDataParameter GetParameter()
        {
            return new NpgsqlParameter();
        }

        /// <summary>
        /// Gets a data parameter for this connection type
        /// </summary>
        /// <param name="name">The parameters name</param>
        /// <param name="value">The value of the parameter</param>
        /// <returns></returns>
        public IDbDataParameter GetParameter(string name, object value)
        {
            if (value != null)
            {
                //postgre is the only one that doesnt do enum.getunderlyingtype() when adding a value to a cmd.  So I do it here so it more consistent with the other guys
                Type type = value.GetType();
                object converted = value;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    converted = CLRConverter.ConvertToType(value, type.GetGenericArguments()[0]);
                    if (converted != null)
                    {
                        type = converted.GetType();
                        converted = typeof(Nullable<>).MakeGenericType(new Type[] { type }).GetConstructor(new Type[] { type }).Invoke(new object[] { converted });
                    }
                }

                if (type.IsEnum)
                {
                    converted = Convert.ChangeType(converted, Enum.GetUnderlyingType(type));
                }

                return new NpgsqlParameter(name, converted);
            }
            else
                return new NpgsqlParameter(name, value);
        }

        /// <summary>
        /// Gets the linq query provider.
        /// </summary>
        /// <param name="dStore">The data store.</param>
        /// <returns></returns>
        public IQueryProvider GetQueryProvider(IDataStore dStore)
        {
            return new PostgreQueryProvider(dStore);
        }

        /// <summary>
        /// Gets a data store configured for postgre
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Catalog">The catalog.</param>
        /// <param name="User">The user.</param>
        /// <param name="Password">The password.</param>
        /// <returns></returns>
        public static IDataStore GetDataStore(string Server, string Catalog, string User, string Password)
        {
            return new PostgreDataStore(Server, Catalog, User, Password);
        }

        /// <summary>
        /// Gets a data store configured for postgre
        /// </summary>
        /// <param name="connection">The connection string</param>
        /// <returns></returns>
        public static IDataStore GetDataStore(string connection)
        {
            return new PostgreDataStore(connection);
        }

        /// <summary>
        /// Performs a bulk insert of data to the datastore (if supported)
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dstore"></param>
        public async Task DoBulkInsert(IList items, IDataStore dstore)
        {
            while (items.Count > 0)
                await dstore.InsertObjects(items.GetSmallerList(100));
        }

        /// <summary>
        /// Returns a delete formatter
        /// </summary>
        /// <param name="dstore"></param>
        /// <returns></returns>
        public IDeleteFormatter GetDeleteFormatter(IDataStore dstore)
        {
            return new DeletePostgreFormatter(dstore);
        }

        /// <summary>
        /// Returns a list of tables from the data store
        /// </summary>
        /// <param name="dstore"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<DBObject> GetSchemaTables(IDataStore dstore)
        {
            DbCommand tblCmd = GetCommand();
            tblCmd.CommandText = _getTablesCommand;

            DbCommand clmCmd = GetCommand();
            clmCmd.CommandText = _getColumnsCommand;

            using (IQueryData tables = await dstore.ExecuteCommands.ExecuteCommandQuery(tblCmd, dstore.Connection))
            using (IQueryData columns = await dstore.ExecuteCommands.ExecuteCommandQuery(clmCmd, dstore.Connection))
            {
                List<IQueryRow> rows = columns.GetQueryEnumerator().ToList();
                foreach (IQueryRow table in tables)
                {
                    yield return await Helpers.LoadObjectInfo(dstore, table, rows);
                }
            }
        }

        /// <summary>
        /// Returns a list of views from the datastore
        /// </summary>
        /// <param name="dstore"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<DBObject> GetSchemaViews(IDataStore dstore)
        {
            DbCommand tblCmd = GetCommand();
            tblCmd.CommandText = _getViewCommand;

            DbCommand clmCmd = GetCommand();
            clmCmd.CommandText = _getViewColumnsCommand;

            using (IQueryData objects = await dstore.ExecuteCommands.ExecuteCommandQuery(tblCmd, dstore.Connection))
            using (IQueryData columns = await dstore.ExecuteCommands.ExecuteCommandQuery(clmCmd, dstore.Connection))
            {
                List<IQueryRow> rows = columns.GetQueryEnumerator().ToList();
                foreach (IQueryRow o in objects)
                    yield return await Helpers.LoadObjectInfo(dstore, o, rows);
            }
        }
    }
}
