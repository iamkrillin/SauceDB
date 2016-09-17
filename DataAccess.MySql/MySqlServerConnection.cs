using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using DataAccess.Core;
using DataAccess.MySql.Linq;
using System.Collections;
using DataAccess.Core.Data;
using DataAccess.Core.Conversion;
using DataAccess.Core.Data.Results;
using System.Threading.Tasks;

namespace DataAccess.MySql
{
    /// <summary>
    /// A connection for mysql
    /// </summary>
    public class MySqlServerConnection : IDataConnection
    {
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
        public string LeftEscapeCharacter { get { return "`"; } }

        /// <summary>
        /// the data stores escape character (right side)
        /// </summary>
        /// <value></value>
        public string RightEscapeCharacter { get { return "`"; } }

        /// <summary>
        /// The default schema for this data store
        /// </summary>
        /// <value></value>
        public string DefaultSchema { get { return ""; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlServerConnection"/> class
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Catalog">The catalog.</param>
        /// <param name="User">The user.</param>
        /// <param name="Password">The password.</param>
        public MySqlServerConnection(string Server, string Catalog, string User, string Password)
            : this(string.Format("server={0};user id={1};password={2};persist security info=True;database={3}", Server, User, Password, Catalog))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlServerConnection"/> class.
        /// </summary>
        /// <param name="ConnectionString">The connection string.</param>
        public MySqlServerConnection(string ConnectionString)
        {
            _commandGenerator = new MySqlCommandGenerator(StorageEngine.InnoDB);
            _connectionString = ConnectionString;
            _tConverter = new StandardCLRConverter();
            _dConverter = new MySqlServerDBConverter();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlServerConnection"/> class.
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Catalog">The catalog.</param>
        /// <param name="User">The user.</param>
        /// <param name="Password">The password.</param>
        /// <param name="engine">The storage engine to use when creating tables.</param>
        public MySqlServerConnection(string Server, string Catalog, string User, string Password, StorageEngine engine)
            : this(string.Format("server={0};user id={1};password={2};persist security info=True;database={3}", Server, User, Password, Catalog), engine)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlServerConnection"/> class.
        /// </summary>
        /// <param name="ConnectionString">The connection string.</param>
        /// <param name="engine">The storage engine to use when creating tables</param>
        public MySqlServerConnection(string ConnectionString, StorageEngine engine)
        {
            _commandGenerator = new MySqlCommandGenerator(engine);
            _connectionString = ConnectionString;
            _tConverter = new StandardCLRConverter();
            _dConverter = new MySqlServerDBConverter();
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        /// <summary>
        /// Gets a data command for this connection type
        /// </summary>
        /// <returns></returns>
        public IDbCommand GetCommand()
        {
            return new MySqlCommand();
        }

        /// <summary>
        /// Gets a data parameter for this connection type
        /// </summary>
        /// <returns></returns>
        public IDbDataParameter GetParameter()
        {
            return new MySqlParameter();
        }

        /// <summary>
        /// Gets a data parameter for this connection type
        /// </summary>
        /// <param name="name">The parameters name</param>
        /// <param name="value">The value of the parameter</param>
        /// <returns></returns>
        public IDbDataParameter GetParameter(string name, object value)
        {
            return new MySqlParameter(name, value);
        }

        /// <summary>
        /// Gets a data store configured for mysql, storage engine defaults to innodb
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Catalog">The catalog.</param>
        /// <param name="User">The user.</param>
        /// <param name="Password">The password.</param>
        /// <returns></returns>
        public static IDataStore GetDataStore(string Server, string Catalog, string User, string Password)
        {
            return new MySqlDataStore(Server, Catalog, User, Password);
        }

        /// <summary>
        /// Gets a data store configured for mysql, storage engine defaults to innodb
        /// </summary>
        /// <param name="connection">The connection string</param>
        /// <returns></returns>
        public static IDataStore GetDataStore(string connection)
        {
            return new MySqlDataStore(connection);
        }

        /// <summary>
        /// Gets a data store configured for mysql
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Catalog">The catalog.</param>
        /// <param name="User">The user.</param>
        /// <param name="Password">The password.</param>
        /// <param name="engine">The storage engine to use</param>
        /// <returns></returns>
        public static IDataStore GetDataStore(string Server, string Catalog, string User, string Password, StorageEngine engine)
        {
            return new MySqlDataStore(Server, Catalog, User, Password, engine);
        }

        /// <summary>
        /// Gets a data store configured for mysql
        /// </summary>
        /// <param name="connection">The connection string</param>
        /// <param name="engine">The storage engine to use</param>
        /// <returns></returns>
        public static IDataStore GetDataStore(string connection, StorageEngine engine)
        {
            return new MySqlDataStore(connection, engine);
        }

        /// <summary>
        /// Returns a linq query provider
        /// </summary>
        /// <param name="dStore">The datastore to use for querying</param>
        /// <returns></returns>
        public IQueryProvider GetQueryProvider(IDataStore dStore)
        {
            return new MySqlQueryProvider(dStore);
        }

        /// <summary>
        /// Performs a bulk insert of data to the datastore (if supported)
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dstore"></param>
        public void DoBulkInsert(IList items, IDataStore dstore)
        {
            while (items.Count > 0)
                dstore.InsertObjects(items.GetSmallerList(100));
        }

        /// <summary>
        /// Returns a delete formatter
        /// </summary>
        /// <param name="dstore"></param>
        /// <returns></returns>
        public IDeleteFormatter GetDeleteFormatter(IDataStore dstore)
        {
            return new DeleteMySqlFormatter(dstore);
        }

        /// <summary>
        /// Returns a list of tables from the data store
        /// </summary>
        /// <param name="dstore"></param>
        /// <returns></returns>
        public async Task<List<DBObject>> GetSchemaTables(IDataStore dstore)
        {
            MySqlCommand getColumns = new MySqlCommand();
            MySqlCommand getTables = new MySqlCommand();
            getTables.CommandText = "show full tables where table_type != 'view'";
            List<DBObject> items = new List<DBObject>();

            using (IQueryData tables = await dstore.ExecuteCommands.ExecuteCommandQuery(getTables, dstore.Connection))
            {
                foreach (IQueryRow row in tables)
                {
                    DBObject t = new DBObject();
                    t.Name = row.GetDataForRowField(0).ToString();
                    t.Columns = new List<Column>();

                    getColumns.CommandText = string.Concat("DESCRIBE ", t.Name);
                    using (IQueryData columns = await dstore.ExecuteCommands.ExecuteCommandQuery(getColumns, dstore.Connection))
                    {

                        foreach (QueryRow c in columns)
                        {
                            t.Columns.Add(new Column()
                            {
                                Name = c.GetDataForRowField("Field").ToString(),
                                DataType = c.GetDataForRowField("Type").ToString(),
                                IsPrimaryKey = c.GetDataForRowField("Key").ToString().Equals("PRI", StringComparison.InvariantCultureIgnoreCase),
                                DefaultValue = c.GetDataForRowField("Default").ToString()
                            });
                        }

                        items.Add(t);
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// Returns a list of views from the datastore
        /// </summary>
        /// <param name="dstore"></param>
        /// <returns></returns>
        public async Task<List<DBObject>> GetSchemaViews(IDataStore dstore)
        {
            MySqlCommand getColumns = new MySqlCommand();
            MySqlCommand getTables = new MySqlCommand();
            getTables.CommandText = "show full tables where table_type = 'view'";

            List<DBObject> items = new List<DBObject>();

            using (IQueryData tables = await dstore.ExecuteCommands.ExecuteCommandQuery(getTables, dstore.Connection))
            {
                foreach (IQueryRow tab in tables)
                {
                    DBObject t = new DBObject();
                    t.Name = tab.GetDataForRowField(0).ToString();
                    t.Columns = new List<Column>();

                    try
                    {
                        getColumns.CommandText = string.Concat("DESCRIBE ", t.Name);
                        using (IQueryData columns = await dstore.ExecuteCommands.ExecuteCommandQuery(getColumns, dstore.Connection))
                        {
                            foreach (IQueryRow col in columns)
                            {
                                t.Columns.Add(new Column()
                                {
                                    Name = col.GetDataForRowField("Field").ToString(),
                                    DataType = col.GetDataForRowField("Type").ToString(),
                                    IsPrimaryKey = col.GetDataForRowField("Key").ToString().Equals("PRI", StringComparison.InvariantCultureIgnoreCase),
                                    DefaultValue = col.GetDataForRowField("Default").ToString()
                                });
                            }
                        }
                    }
                    catch //this is intentional, mysql will bomb out when a view is messed up if you try to retrieve the columns
                    {
                    }

                    items.Add(t);
                }
            }

            return items;
        }
    }
}
