using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Events;
using DataAccess.Core.Data;

namespace DataAccess.Core.Interfaces
{
    /// <summary>
    /// Represents an object that validates a type against a datastore object
    /// </summary>
    public interface IDatastoreObjectValidator
    {
        /// <summary>
        /// If false, this validator should never attempt to remove columns
        /// </summary>
        bool CanRemoveColumns { get; set; }

        /// <summary>
        /// If false, this validator should never attempt to add columns
        /// </summary>
        bool CanAddColumns { get; set; }

        /// <summary>
        /// If false, this validator should never attempt to update columns
        /// </summary>
        bool CanUpdateColumns { get; set; }

        /// <summary>
        /// This event is fired when an object is added by the validator
        /// </summary>
        event EventHandler<ObjectCreatedEventArgs> OnObjectCreated;

        /// <summary>
        /// This event is fired when an object is modified by the validator
        /// </summary>
        event EventHandler<ObjectModifiedEventArgs> OnObjectModified;

        /// <summary>
        /// Validates an objects info against the datastore
        /// </summary>
        /// <param name="ti"></param>
        void ValidateObject(DatabaseTypeInfo ti);

        /// <summary>
        /// Returns a list of objects from the datastore
        /// </summary>
        /// <param name="forceReload">Will force the fetch of a fresh copy</param>
        /// <returns></returns>
        IEnumerable<DBObject> GetObjects(bool forceReload);

        /// <summary>
        /// Returns a list of objects from the datastore
        /// </summary>
        /// <returns></returns>
        IEnumerable<DBObject> GetObjects();

        /// <summary>
        /// Will return true if a given object exists in the datastore
        /// </summary>
        /// <returns></returns>
        bool ObjectExistsInDataStore(DatabaseTypeInfo info);
    }
}
