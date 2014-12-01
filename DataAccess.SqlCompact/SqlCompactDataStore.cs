using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;
using System.Data.SqlServerCe;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Events;
using DataAccess.Core.ObjectFinders;

namespace DataAccess.SqlCompact
{
    /// <summary>
    /// Creates a datastore for sql server
    /// </summary>
    public class SqlCompactDataStore : DataStore
    {
        /// <summary>
        /// Creates a new datastore (integrated auth)
        /// </summary>
        /// <param name="Server">the server to connect to</param>
        /// <param name="Catalog">The catalog to use</param>
        public SqlCompactDataStore(string file)
            : this(new SqlCompactConnection(file))
        {
            ExecuteCommands = new SqlCompactExecuteCommands();
        }

        internal SqlCompactDataStore(SqlCompactConnection conn)
            : base(conn)
        {
            ExecuteCommands = new SqlCompactExecuteCommands();
            ObjectFinder = new NoSchemaSupportObjectFinder();
        }

        internal SqlCompactDataStore(IDataConnection Connection, IExecuteDatabaseCommand ExecuteCommands, ITypeInformationParser TypeInformationParser)
            : base(Connection, ExecuteCommands, TypeInformationParser)
        {
        }

        public override bool InsertObject(object item)
        {
            TypeInfo ti = TypeInformationParser.GetTypeInfo(item.GetType());

            EventHandler<CommandExecutingEventArgs> inserting = (sender, e) =>
            {
                if (ti.PrimaryKeys.Count == 1)
                {
                    SqlCeCommand cmd = new SqlCeCommand("SELECT @@Identity;");
                    cmd.Connection = (SqlCeConnection)e.RawConnection;
                    object data = cmd.ExecuteScalar();

                    ti.PrimaryKeys[0].Setter(item, Connection.CLRConverter.ConvertToType(data, ti.PrimaryKeys[0].PropertyType) ,null);
                }
            };

            this.ExecuteCommands.CommandExecuted += inserting;
            bool result =  base.InsertObject(item);
            this.ExecuteCommands.CommandExecuted -= inserting;
            return result;
        }

        public override bool InsertObjects(System.Collections.IList items)
        {
            foreach (var v in items)
                InsertObject(v);

            return true;
        }

        public override IDataStore GetNewInstance()
        {
            IDataStore dstore = new SqlCompactDataStore(Connection, ExecuteCommands, TypeInformationParser);
            dstore.TypeInformationParser = new TypeParser(dstore);
            dstore.SchemaValidator = SchemaValidator;
            dstore.ExecuteCommands = ExecuteCommands;
            return dstore;
        }
    }
}
