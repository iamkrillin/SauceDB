using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;

namespace DataAccess.Oracle
{
    public class OracleDataStore : DataStore
    {       
        public OracleDataStore(string ConnectionString)
            : this(new OracleConnection(ConnectionString))
        {
        }

        public OracleDataStore(string Server, string User, string Password)
            : this(new OracleConnection(Server, User, Password))
        {

        }

        public OracleDataStore(OracleConnection conn)
            : base(conn)
        {
            SchemaValidator.TableValidator = new OracleTableValidator(this);
        }
    }
}
