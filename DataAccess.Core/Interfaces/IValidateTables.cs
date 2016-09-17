using DataAccess.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Core
{
    /// <summary>
    /// Exposes some more methods to handle table modifications
    /// </summary>
    public interface IValidateTables : IDatastoreObjectValidator
    {
        /// <summary>
        /// Will add a table to the datastore and return the result
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        Task<DBObject> AddTable(DatabaseTypeInfo ti);

        /// <summary>
        /// Will add a column to the datastore
        /// </summary>
        /// <param name="field"></param>
        /// <param name="ti"></param>
        /// <returns></returns>
        bool AddColumn(DataFieldInfo field, DatabaseTypeInfo ti);

        /// <summary>
        /// will remove a column from the datastore
        /// </summary>
        /// <param name="field"></param>
        /// <param name="ti"></param>
        /// <returns></returns>
        bool RemoveColumn(DataFieldInfo field, DatabaseTypeInfo ti);

        /// <summary>
        /// Modifies a column on a table to match the domain object (data type and such)
        /// </summary>
        /// <param name="dfi"></param>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        bool ModifyColumn(DataFieldInfo dfi, DatabaseTypeInfo typeInfo);

        /// <summary>
        /// Makes sure the schema is valid and present (if required )
        /// </summary>
        /// <param name="ti"></param>
        void CheckSchema(DatabaseTypeInfo ti);
    }
}
