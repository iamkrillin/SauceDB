using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using System.Data;
using DataAccess.Core;
using System.Collections;
using DataAccess.Core.Data;
using DataAccess.Core.Conversion;
using Mono.Data.Sqlite;
using DataAccess.Xamarin.iOS.Linq;
using DataAccess.Xamarin.iOS.Conversion;
using DataAccess.Core.Data.Results;

namespace DataAccess.Xamarin.iOS
{
    /// <summary>
    /// A connection to sqlite
    /// </summary>
    public class iOSDataConnection : IDataConnection
    {
        private IConvertToCLR _tConverter;
        private string _connectionString;
        private ICommandGenerator _commandGenerator;
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
        /// Initializes a new instance of the <see cref="iOSDataConnection"/> class.
        /// </summary>
        /// <param name="filename">The filename</param>
        public iOSDataConnection(string filename)
        {
            _connectionString = string.Format("Data Source={0}; Version=3;", filename);
            _tConverter = new iOSCLRConverter();
            _commandGenerator = new iOSCommandGenerator();
            _dConverter = new StandardDBConverter();
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        /// <summary>
        /// Gets a data command for this connection type
        /// </summary>
        /// <returns></returns>
        public IDbCommand GetCommand()
        {
            return new SqliteCommand();
        }

        /// <summary>
        /// Gets a data parameter for this connection type
        /// </summary>
        /// <returns></returns>
        public IDbDataParameter GetParameter()
        {
            return new SqliteParameter();
        }

        /// <summary>
        /// Gets a data parameter for this connection type
        /// </summary>
        /// <param name="name">The parameters name</param>
        /// <param name="value">The value of the parameter</param>
        /// <returns></returns>
        public IDbDataParameter GetParameter(string name, object value)
        {
            return new SqliteParameter(name, value);
        }

        /// <summary>
        /// Gets a data store configured for SQLite
        /// </summary>
        /// <param name="filename">The file name to use</param>
        /// <returns></returns>
        public static IDataStore GetDataStore(string filename)
        {
            return new iOSDataStore(filename);
        }

        /// <summary>
        /// Returns a linq query provider
        /// </summary>
        /// <param name="dStore">The datastore to use for querying</param>
        /// <returns></returns>
        public IQueryProvider GetQueryProvider(IDataStore dStore)
        {
            return new iOSQueryProvider(dStore);
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
            return new DeleteSqliteFormatter(dstore);
        }


        /// <summary>
        /// Returns a list of tables from the data store
        /// </summary>
        /// <param name="dstore"></param>
        /// <returns></returns>
        public IEnumerable<DBObject> GetSchemaTables(IDataStore dstore)
        {
            SqliteCommand getTables = new SqliteCommand();
            getTables.CommandText = "select name from sqlite_master where type = 'table'";
            QueryData tables = dstore.ExecuteCommands.ExecuteCommandQuery(getTables, dstore.Connection);
            foreach(QueryRow tab in tables)
            {
                using (SqliteCommand getColumns = new SqliteCommand())
                {
                    DBObject t = new DBObject();
                    t.Name = tab.GetDataForRowField(0).ToString();
                    t.Columns = new List<Column>();

                    getColumns.CommandText = string.Concat("pragma table_info(", t.Name, ");");
                    QueryData columns = dstore.ExecuteCommands.ExecuteCommandQuery(getColumns, dstore.Connection);

                    foreach (QueryRow col in columns)
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

        /// <summary>
        /// Returns a list of views from the datastore
        /// </summary>
        /// <param name="dstore"></param>
        /// <returns></returns>
        public IEnumerable<DBObject> GetSchemaViews(IDataStore dstore)
        {
            List<DBObject> toReturn = new List<DBObject>();
            SqliteCommand getTables = new SqliteCommand();
            getTables.CommandText = "select name from sqlite_master where type = 'view'";
            QueryData tables = dstore.ExecuteCommands.ExecuteCommandQuery(getTables, dstore.Connection);
            foreach(QueryRow tab in tables)
            {
                using (SqliteCommand getColumns = new SqliteCommand())
                {
                    DBObject t = new DBObject();
                    t.Name = tab.GetDataForRowField(0).ToString();
                    t.Columns = new List<Column>();

                    getColumns.CommandText = string.Concat("pragma table_info(", t.Name, ");");
                    QueryData columns = dstore.ExecuteCommands.ExecuteCommandQuery(getColumns, dstore.Connection);

                    foreach(QueryRow col in columns)
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
