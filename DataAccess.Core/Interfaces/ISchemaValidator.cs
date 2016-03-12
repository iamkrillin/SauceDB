using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;
using DataAccess.Core.Events;

namespace DataAccess.Core.Interfaces
{
    /// <summary>
    /// Represents an object that will perform schema validation/modification on a data store
    /// </summary>
    public interface ISchemaValidator
    {
        /// <summary>
        /// If false, the schema validator will never remove columns (defaults to false)
        /// </summary>
        bool CanRemoveColumns { get; set; }

        /// <summary>
        /// If false, the schema validator will never add columns (defaults to true)
        /// </summary>
        bool CanAddColumns { get; set; }

        /// <summary>
        /// If false, the schema validator will never update columns (defaults to true)
        /// </summary>
        bool CanUpdateColumns { get; set; }

        /// <summary>
        /// The component to use when validating tables
        /// </summary>
        IDatastoreObjectValidator TableValidator { get; set; }

        /// <summary>
        /// The component to use when validating views
        /// </summary>
        IDatastoreObjectValidator ViewValidator { get; set; }

        /// <summary>
        /// Performs schema validation/modification to match the type
        /// </summary>
        /// <param name="typeInfo"></param>
        void ValidateType(DatabaseTypeInfo typeInfo);
    }
}
