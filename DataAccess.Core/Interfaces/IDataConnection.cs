using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess.Core.Data;
using System.Collections;
using System.Threading.Tasks;

namespace DataAccess.Core
{
    /// <summary>
    /// A connection to a data store
    /// </summary>
    public interface IDataConnection
    {
        /// <summary>
        /// Converts data on the way out that is Datastore -> CLR
        /// </summary>
        IConvertToCLR CLRConverter { get; }

        /// <summary>
        /// Coverts data on the way in that is, CLR -> Datastore
        /// </summary>
        IConvertToDatastore DatastoreConverter { get; }

        /// <summary>
        /// The command generator for this data store
        /// </summary>
        ICommandGenerator CommandGenerator { get; }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        IDbConnection GetConnection();

        /// <summary>
        /// Gets a data command for this connection type
        /// </summary>
        /// <returns></returns>
        IDbCommand GetCommand();

        /// <summary>
        /// Gets a data parameter for this connection type
        /// </summary>
        /// <param name="name">The parameters name</param>
        /// <param name="value">The value of the parameter</param>
        /// <returns></returns>
        IDbDataParameter GetParameter(string name, object value);

        /// <summary>
        /// Returns a list of tables from the data store
        /// </summary>
        /// <param name="dstore"></param>
        /// <returns></returns>
        Task<List<DBObject>> GetSchemaTables(IDataStore dstore);

        /// <summary>
        /// Returns a list of views from the datastore
        /// </summary>
        /// <param name="dstore"></param>
        /// <returns></returns>
        Task<List<DBObject>> GetSchemaViews(IDataStore dstore);

        /// <summary>
        /// the data stores escape character (left side)
        /// </summary>
        string LeftEscapeCharacter { get; }

        /// <summary>
        /// the data stores escape character (right side)
        /// </summary>
        string RightEscapeCharacter { get; }

        /// <summary>
        /// The default schema for this data store
        /// </summary>
        string DefaultSchema { get; }

        /// <summary>
        /// Returns a linq query provider
        /// </summary>
        /// <param name="dStore">The datastore to use for querying</param>
        /// <returns></returns>
        IQueryProvider GetQueryProvider(IDataStore dStore);

        /// <summary>
        /// Returns a delete formatter
        /// </summary>
        /// <returns></returns>
        IDeleteFormatter GetDeleteFormatter(IDataStore dstore);

        /// <summary>
        /// Performs a bulk insert of data to the datastore (if supported)
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dstore"></param>
        void DoBulkInsert(IList items, IDataStore dstore);
    }
}
