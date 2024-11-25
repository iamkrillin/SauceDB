using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using System.Data;
using DataAccess.Core.Events;
using System.Threading.Tasks;

namespace DataAccess.Core.ObjectValidators
{
    /// <summary>
    /// Will validate an object against the data store
    /// </summary>
    public abstract class ObjectValidator : IDatastoreObjectValidator
    {
        /// <summary>
        /// If false, the schema validator should never try to remove columns
        /// </summary>
        public bool CanRemoveColumns { get; set; }

        /// <summary>
        /// If false, the schema validator should never try to add columns
        /// </summary>
        public bool CanAddColumns { get; set; }

        /// <summary>
        /// If false, the schema validator should never try to update columns
        /// </summary>
        public bool CanUpdateColumns { get; set; }

        /// <summary>
        /// This event is fired when an object is added by the validator
        /// </summary>
        public event EventHandler<ObjectCreatedEventArgs> OnObjectCreated;

        /// <summary>
        /// This event is fired when an object is modified by the validator
        /// </summary>
        public event EventHandler<ObjectModifiedEventArgs> OnObjectModified;

        /// <summary>
        /// The data store that holds this validator
        /// </summary>
        protected IDataStore _dstore;

        /// <summary>
        /// Gets or sets the objects.
        /// </summary>
        /// <value>
        /// The objects.
        /// </value>
        protected List<DBObject> Objects { get; set; }

        /// <summary>
        /// Validates an objects info against the datastore
        /// </summary>
        /// <param name="ti"></param>
        public abstract Task ValidateObject(TypeParser tparser, DatabaseTypeInfo ti);

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectValidator"/> class.
        /// </summary>
        /// <param name="dStore">The d store.</param>
        public ObjectValidator(IDataStore dStore)
        {
            _dstore = dStore;
            Objects = new List<DBObject>();
        }

        /// <summary>
        /// Returns a list of objects from the datastore
        /// </summary>
        /// <param name="forceReload">Will force the fetch of a fresh copy</param>
        /// <returns></returns>
        public virtual IEnumerable<DBObject> GetObjects(bool forceReload)
        {
            if (forceReload) Objects.Clear();
            return GetObjects();
        }

        /// <summary>
        /// Returns a list of objects from the datastore
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<DBObject> GetObjects();        

        /// <summary>
        /// Gets the object., if you need to change this behavior, see <seealso cref="DataAccess.Core.Interfaces.IFindDataObjects"/>
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <returns></returns>
        protected DBObject GetObject(DatabaseTypeInfo typeInfo)
        {
            return _dstore.ObjectFinder.GetObject(GetObjects(), typeInfo);
        }
        
        /// <summary>
        /// Fires the created event
        /// </summary>
        /// <param name="ti">The ti.</param>
        protected void FireCreated(DatabaseTypeInfo ti)
        {
            OnObjectCreated?.Invoke(this, new ObjectCreatedEventArgs(ti));
        }

        /// <summary>
        /// Fires the modified event
        /// </summary>
        /// <param name="ti">The ti.</param>
        /// <param name="action">A description of the action taken</param>
        protected void FireModified(DatabaseTypeInfo ti, string action)
        {
            FireModified(ti, action, null);
        }

        /// <summary>
        /// Fires the modified event
        /// </summary>
        /// <param name="ti">The ti.</param>
        /// <param name="action">A description of the action taken</param>
        /// <param name="stringFormatData">Allows you to do stirng.format() style action message, just provide the items and a formattable string for action else null</param>
        protected void FireModified(DatabaseTypeInfo ti, string action, params object[] stringFormatData)
        {
            if (OnObjectModified != null)
            {
                if (stringFormatData != null)
                    action = string.Format(action, stringFormatData);

                OnObjectModified(this, new ObjectModifiedEventArgs(ti, action));
            }
        }

        public bool ObjectExistsInDataStore(DatabaseTypeInfo info)
        {
            return GetObject(info) != null;
        }
    }
}
