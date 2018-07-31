using Advantage.Data.Provider;
using DataAccess.Core;
using DataAccess.Core.Conversion;
using DataAccess.Core.Data;
using DataAccess.Core.Data.Results;
using DataAccess.Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DataAccess.Advantage
{
    public class AdvantageConnection : IDataConnection
    {
        private string _conn;
        public string LeftEscapeCharacter { get { return "\""; } }
        public string RightEscapeCharacter { get { return "\""; } }
        public IConvertToCLR CLRConverter { get; private set; }
        public ICommandGenerator CommandGenerator { get; private set; }
        public IConvertToDatastore DatastoreConverter { get; private set; }
        public string DefaultSchema { get { return ""; } }

        public AdvantageConnection(string conn)
        {
            _conn = conn;
            CLRConverter = new StandardCLRConverter();
            DatastoreConverter = new AdvantageDatabaseConverter();
            CommandGenerator = new AdvantageDatabaseCommandGenerator(this);
        }

        public AdvantageConnection(string datapath, string tabletype, string servertype)
            : this(string.Format("data source={0};tabletype={1};servertype={2};SecurityMode=IGNORERIGHTS", datapath, tabletype, servertype))
        {
        }

        public void DoBulkInsert(IList items, IDataStore dstore)
        {
            throw new NotImplementedException();
        }

        public IDbCommand GetCommand()
        {
            return new AdsCommand();
        }

        public IDbCommand GetCommand(string text)
        {
            return new AdsCommand(text);
        }

        public IDbConnection GetConnection()
        {
            AdsConnection toReturn = new AdsConnection(_conn);            
            return toReturn;
        }

        public IDeleteFormatter GetDeleteFormatter(IDataStore dstore)
        {
            throw new NotImplementedException();
        }

        public IDbDataParameter GetParameter(string name, object value)
        {
            return new AdsParameter(name, value);
        }

        public IQueryProvider GetQueryProvider(IDataStore dStore)
        {
            return new AdvantageQueryProvider(dStore);
        }

        public IEnumerable<DBObject> GetSchemaTables(IDataStore dstore)
        {
            AdsConnection connection = (AdsConnection)GetConnection();
            connection.Open();
            using (IQueryData columns = dstore.ExecuteCommands.ExecuteCommandQuery(GetCommand("EXECUTE PROCEDURE sp_GetColumns(null, null, null, null)"), this))
            { 
                foreach (string table in connection.GetTableNames())
                {
                    yield return LoadObjectInfo(dstore, table, columns);
                }
            }
        }

        private void AddColumn(IDataStore dstore, IQueryRow columns, DBObject t)
        {
            Column toAdd = new Column();
            toAdd.ColumnLength = ((string)CLRConverter.ConvertToType(columns.GetDataForRowField("COLUMN_SIZE"), typeof(string))).Trim();
            toAdd.DataType = ((string)CLRConverter.ConvertToType(columns.GetDataForRowField("TYPE_NAME"), typeof(string))).Trim();
            toAdd.DefaultValue = null;
            toAdd.IsPrimaryKey = false;
            toAdd.Name =((string)CLRConverter.ConvertToType(columns.GetDataForRowField("COLUMN_NAME"), typeof(string))).Trim();           

            t.Columns.Add(toAdd);
        }

        public DBObject LoadObjectInfo(IDataStore dstore, string table, IQueryData columns)
        {
            DBObject t = new DBObject();
            t.Name = table;
            t.Columns = new List<Column>();

            foreach (IQueryRow o in columns) //all of the columns for all of the tables were returned, so we need to only get the one I'm working on...
            {
                if (o.FieldHasMapping("Table_Name")) //make sure the table name is present
                {
                    if(((string)o.GetDataForRowField("TABLE_NAME")).Trim().Equals(table, StringComparison.InvariantCultureIgnoreCase) && o.FieldHasMapping("Column_Name"))
                    {
                        AddColumn(dstore, o, t);
                    }
                    else
                    {
                        o.ResetUsed();
                    }
                }
            }
            return t;
        }

        public IEnumerable<DBObject> GetSchemaViews(IDataStore dstore)
        {
            throw new NotImplementedException();
        }
    }
}
