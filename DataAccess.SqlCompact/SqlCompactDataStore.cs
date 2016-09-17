using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;
using System.Data.SqlServerCe;
using DataAccess.Core.Data;
using DataAccess.Core;
using DataAccess.Core.Events;
using DataAccess.Core.ObjectFinders;
using System.Threading.Tasks;
using DataAccess.Core.Execute;

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

        public override async Task<bool> InsertObject(object item)
        {
            DatabaseTypeInfo ti = TypeInformationParser.GetTypeInfo(item.GetType());

            EventHandler<CommandExecutingEventArgs> inserting = (sender, e) =>
            {
                if (ti.PrimaryKeys.Count == 1)
                {
                    SqlCeCommand cmd = new SqlCeCommand("SELECT @@Identity;");
                    cmd.Connection = (SqlCeConnection)e.RawConnection;
                    object data = cmd.ExecuteScalar();

                    ti.PrimaryKeys[0].Setter(item, Connection.CLRConverter.ConvertToType(data, ti.PrimaryKeys[0].PropertyType));
                }
            };

            this.ExecuteCommands.CommandExecuted += inserting;
            bool result = await base.InsertObject(item);
            this.ExecuteCommands.CommandExecuted -= inserting;
            return result;
        }

        public override async Task<bool> InsertObjects(System.Collections.IList items)
        {
            IDataStore toUse = this;
            TransactionContext ctx = null;

            if (!(this.ExecuteCommands is TransactionCommandExecutor))
            {
                ctx = StartTransaction();
                toUse = ctx.Instance;
            }

            foreach (var v in items)
                await toUse.InsertObject(v);

            if (ctx != null)
            {
                ctx.Commit();
                ctx.Dispose();
            }

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
