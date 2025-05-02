using DataAccess.Core;
using DataAccess.Core.ObjectFinders;
using System.Collections;
using System.Threading.Tasks;

namespace DataAccess.SQLite
{
    /// <summary>
    /// Creates a new data store for SQLite
    /// </summary>
    public class SqliteDataStore : DataStore
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="filename">The file name to use</param>
        public SqliteDataStore(string filename)
            : base(new SqlLiteDataConnection(filename))
        {
            ObjectFinder = new NoSchemaSupportObjectFinder();
            ExecuteCommands = new SqliteExecuteCommands();
        }

        /// <summary>
        /// SQLite doesn't support multiple inserts with one command, so this loops
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public override async Task<bool> InsertObjects(IList items)
        {
            bool result = false;
            foreach (var v in items)
            {
                result = await InsertObject(v);
                if (!result) break;
            }
            return result;
        }

        public override TransactionContext StartTransaction()
        {
            TransactionContext context = base.StartTransaction();


            return context;
        }
    }
}
