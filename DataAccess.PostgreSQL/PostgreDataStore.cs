using DataAccess.Core;
using DataAccess.Core.ObjectFinders;

namespace DataAccess.PostgreSQL
{
    /// <summary>
    /// creates a datastore for postgre
    /// </summary>
    public class PostgreDataStore : DataStore
    {
        /// <summary>
        /// Builds a postgre data store
        /// </summary>
        /// <param name="Server">The server to use</param>
        /// <param name="Catalog">The catalog to use</param>
        /// <param name="User">The user to connect as</param>
        /// <param name="Password">The user password</param>
        public PostgreDataStore(string Server, string Catalog, string User, string Password)
            : this(new PostgreSQLServerConnection(Server, Catalog, User, Password))
        {

        }

        /// <summary>
        /// Builds a postgre data store
        /// </summary>
        /// <param name="connectionstring">The connection string to use</param>
        public PostgreDataStore(string connectionstring)
            : this(new PostgreSQLServerConnection(connectionstring))
        {
        }

        internal PostgreDataStore(PostgreSQLServerConnection connection)
            : base(connection)
        {
            ObjectFinder = new NoSchemaSupportObjectFinder();
        }
    }
}
