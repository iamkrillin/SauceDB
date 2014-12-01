using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;
using DataAccess.Core.ObjectFinders;

namespace DataAccess.MySql
{
    /// <summary>
    /// Builds a datastore for mysql
    /// </summary>
    public class MySqlDataStore : DataStore
    {
        /// <summary>
        /// Inits a datastore for mysql (innodb for new tables)
        /// </summary>
        /// <param name="Server">The server to use</param>
        /// <param name="Catalog">The catalot to use</param>
        /// <param name="User">The user to connect as</param>
        /// <param name="Password">The users password</param>
        public MySqlDataStore(string Server, string Catalog, string User, string Password)
            : this(new MySqlServerConnection(Server, Catalog, User, Password, StorageEngine.InnoDB))
        {

        }

        /// <summary>
        /// Inits a datastore for mysql (innodb for new tables)
        /// </summary>
        /// <param name="connection">The connection string to use</param>
        public MySqlDataStore(string connection)
            : this(new MySqlServerConnection(connection, StorageEngine.InnoDB))
        {

        }

        /// <summary>
        /// Inits a datastore for mysql (innodb for new tables)
        /// </summary>
        /// <param name="Server">The server to use</param>
        /// <param name="Catalog">The catalot to use</param>
        /// <param name="User">The user to connect as</param>
        /// <param name="Password">The users password</param>
        /// <param name="engine">The engine type for new tables</param>
        public MySqlDataStore(string Server, string Catalog, string User, string Password, StorageEngine engine)
            : this(new MySqlServerConnection(Server, Catalog, User, Password, engine))
        {

        }

        /// <summary>
        /// Inits a datastore for mysql (innodb for new tables)
        /// </summary>
        /// <param name="connection">The connection string to use</param>
        /// <param name="engine">The engine type for new tables</param>
        public MySqlDataStore(string connection, StorageEngine engine)
            : this(new MySqlServerConnection(connection, engine))
        {

        }

        internal MySqlDataStore(MySqlServerConnection conn)
            : base(conn)
        {
            ObjectFinder = new NoSchemaSupportObjectFinder();
        }
    }
}
