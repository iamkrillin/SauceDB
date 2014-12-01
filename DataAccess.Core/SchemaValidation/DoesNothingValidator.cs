using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Data;
using DataAccess.Core.Events;

namespace DataAccess.Core.Schema
{
    /// <summary>
    /// This schema validator does nothing
    /// </summary>
    public class DoesNothingValidator : ISchemaValidator
    {
        public bool CanRemoveColumns { get { return false; } set { } }
        public bool CanAddColumns { get { return false; } set { } }
        public bool CanUpdateColumns { get { return false; } set { } }

        /// <summary>
        /// These are empty properties
        /// </summary>
        public IDatastoreObjectValidator TableValidator { get; set; }

        /// <summary>
        /// These are empty properties
        /// </summary>
        public IDatastoreObjectValidator ViewValidator { get; set; }

        /// <summary>
        /// Always returns an empty list
        /// </summary>
        /// <returns></returns>
        public List<DBObject> GetTables()
        {
            return new List<DBObject>();
        }

        /// <summary>
        /// Always returns true
        /// </summary>
        public bool AddColumn(DataFieldInfo field, TypeInfo ti)
        {
            return true;
        }

        /// <summary>
        /// Always returns true
        /// </summary>
        public bool RemoveColumn(DataFieldInfo field, TypeInfo ti)
        {
            return true;
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        public void ValidateType(TypeInfo typeInfo)
        {

        }

        /// <summary>
        /// Always returns an empty list
        /// </summary>
        public List<DBObject> GetTables(bool forceReload)
        {
            return new List<DBObject>();
        }

#pragma warning disable 0067
        /// <summary>
        /// Occurs when [on object created].
        /// </summary>
        public event EventHandler<ObjectCreatedEventArgs> OnObjectCreated;

        /// <summary>
        /// Occurs when [on object modified].
        /// </summary>
        public event EventHandler<ObjectModifiedEventArgs> OnObjectModified;
    }
}
