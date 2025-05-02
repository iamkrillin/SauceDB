using DataAccess.Core;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using DataAccess.SQLite.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.SQLite
{
    /// <summary>
    /// A connection to sqlite
    /// </summary>
    public class SqlLiteDataConnection : IDataConnection
    {
        private IConvertToCLR _tConverter;
        private string _connectionString;
        private ICommandGenerator _commandGenerator;
        private IConvertToDatastore _dConverter;


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
        public string LeftEscapeCharacter => "\"";

        /// <summary>
        /// the data stores escape character (right side)
        /// </summary>
        /// <value></value>
        public string RightEscapeCharacter => "\"";

        /// <summary>
        /// The default schema for this data store
        /// </summary>
        /// <value></value>
        public string DefaultSchema => "";

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLiteDataConnection"/> class.
        /// </summary>
        /// <param name="filename">The filename</param>
        public SqlLiteDataConnection(string filename)
        {
            _connectionString = $"Data Source={filename}; Version=3;";
            _tConverter = new StandardCLRConverter();
            _commandGenerator = new SqliteCommandGenerator(this);
            _dConverter = new SQLiteDBConverter();
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        public DbConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        /// <summary>
        /// Gets a data command for this connection type
        /// </summary>
        /// <returns></returns>
        public DbCommand GetCommand()
        {
            return new SQLiteCommand();
        }

        /// <summary>
        /// Gets a data parameter for this connection type
        /// </summary>
        /// <returns></returns>
        public IDbDataParameter GetParameter()
        {
            return new SQLiteParameter();
        }

        /// <summary>
        /// Gets a data parameter for this connection type
        /// </summary>
        /// <param name="name">The parameters name</param>
        /// <param name="value">The value of the parameter</param>
        /// <returns></returns>
        public IDbDataParameter GetParameter(string name, object value)
        {
            return new SQLiteParameter(name, value);
        }

        /// <summary>
        /// Gets a data store configured for SQLite
        /// </summary>
        /// <param name="filename">The file name to use</param>
        /// <returns></returns>
        public static IDataStore GetDataStore(string filename)
        {
            return new SqliteDataStore(filename);
        }

        /// <summary>
        /// Returns a linq query provider
        /// </summary>
        /// <param name="dStore">The datastore to use for querying</param>
        /// <returns></returns>
        public IQueryProvider GetQueryProvider(IDataStore dStore)
        {
            return new SQLiteQueryProvider(dStore);
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
            return new DeleteSqliteFormatter(dstore);
        }


        /// <summary>
        /// Returns a list of tables from the data store
        /// </summary>
        /// <param name="dstore"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<DBObject> GetSchemaTables(IDataStore dstore)
        {
            SQLiteCommand getTables = new SQLiteCommand();
            getTables.CommandText = "select name from sqlite_master where type = 'table'";
            using (IQueryData tables = await dstore.ExecuteCommands.ExecuteCommandQuery(getTables, dstore.Connection))
            {
                foreach (IQueryRow tab in tables)
                {
                    using (SQLiteCommand getColumns = new SQLiteCommand())
                    {
                        DBObject t = new DBObject();
                        t.Name = tab.GetDataForRowField(0).ToString();
                        t.Columns = new List<Column>();

                        getColumns.CommandText = string.Concat("pragma table_info(", t.Name, ");");
                        using (IQueryData columns = await dstore.ExecuteCommands.ExecuteCommandQuery(getColumns, dstore.Connection))
                        {
                            foreach (IQueryRow col in columns)
                            {
                                t.Columns.Add(new Column()
                                {
                                    Name = col.GetDataForRowField("name").ToString(),
                                    DataType = col.GetDataForRowField("type").ToString(),
                                    IsPrimaryKey = (bool)dstore.Connection.CLRConverter.ConvertToType(col.GetDataForRowField("pk"), typeof(bool)),
                                    DefaultValue = col.GetDataForRowField("dflt_value").ToString()
                                });
                            }
                            yield return t;
                        }
                    }
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
            List<DBObject> toReturn = new List<DBObject>();
            SQLiteCommand getTables = new SQLiteCommand();
            getTables.CommandText = "select name from sqlite_master where type = 'view'";
            using (IQueryData tables = await dstore.ExecuteCommands.ExecuteCommandQuery(getTables, dstore.Connection))
            {
                foreach (IQueryRow tab in tables)
                {
                    using (SQLiteCommand getColumns = new SQLiteCommand())
                    {
                        DBObject t = new DBObject();
                        t.Name = tab.GetDataForRowField(0).ToString();
                        t.Columns = new List<Column>();

                        getColumns.CommandText = string.Concat("pragma table_info(", t.Name, ");");
                        using (IQueryData columns = await dstore.ExecuteCommands.ExecuteCommandQuery(getColumns, dstore.Connection))
                        {
                            foreach (IQueryRow col in columns)
                            {
                                t.Columns.Add(new Column()
                                {
                                    Name = col.GetDataForRowField("name").ToString(),
                                    DataType = col.GetDataForRowField("type").ToString(),
                                    IsPrimaryKey = (bool)dstore.Connection.CLRConverter.ConvertToType(col.GetDataForRowField("pk"), typeof(bool)),
                                    DefaultValue = col.GetDataForRowField("dflt_value").ToString()
                                });
                            }

                            yield return t;
                        }
                    }
                }
            }
        }
    }
}
