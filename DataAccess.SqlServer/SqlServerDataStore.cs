using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;

namespace DataAccess.SqlServer
{
    /// <summary>
    /// Creates a datastore for sql server
    /// </summary>
    public class SqlServerDataStore : DataStore
    {
        /// <summary>
        /// Creates a new datastore (integrated auth)
        /// </summary>
        /// <param name="Server">the server to connect to</param>
        /// <param name="Catalog">The catalog to use</param>
        public SqlServerDataStore(string Server, string Catalog)
            : this(new SqlServerConnection(Server, Catalog))
        {

        }

        /// <summary>
        /// Creates a new datastore
        /// </summary>
        /// <param name="ConnectionString">The connection string to use</param>
        public SqlServerDataStore(string ConnectionString)
            : this(new SqlServerConnection(ConnectionString))
        {
        }

        /// <summary>
        /// Creates a new datastore
        /// </summary>
        /// <param name="Server">The server to connect to</param>
        /// <param name="Catalog">The catalog to use</param>
        /// <param name="User">The user to connect as</param>
        /// <param name="Password">The users password</param>
        public SqlServerDataStore(string Server, string Catalog, string User, string Password)
            : this(new SqlServerConnection(Server, Catalog, User, Password))
        {

        }

        public SqlServerDataStore(SqlServerConnection conn)
            : base(conn)
        {
        }
    }
}
