﻿using DataAccess.Core.Data;
using System.Threading.Tasks;

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
        Task ValidateType(DatabaseTypeInfo typeInfo);
    }
}
