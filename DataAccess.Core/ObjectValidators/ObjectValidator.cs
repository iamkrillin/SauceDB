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
    public abstract class ObjectValidator(IDataStore dStore) : IDatastoreObjectValidator
    {
        public bool CanRemoveColumns { get; set; }
        public bool CanAddColumns { get; set; }
        public bool CanUpdateColumns { get; set; }
        
        public event EventHandler<ObjectCreatedEventArgs> OnObjectCreated;
        public event EventHandler<ObjectModifiedEventArgs> OnObjectModified;
        
        protected IDataStore _dstore = dStore;
        protected List<DBObject> Objects { get; set; } = new List<DBObject>();
        
        public abstract Task ValidateObject(TypeParser tparser, DatabaseTypeInfo ti);
        public abstract IEnumerable<DBObject> GetObjects();

        public virtual IEnumerable<DBObject> GetObjects(bool forceReload)
        {
            if (forceReload) Objects.Clear();
            return GetObjects();
        }

        protected DBObject GetObject(DatabaseTypeInfo typeInfo)
        {
            return _dstore.ObjectFinder.GetObject(GetObjects(), typeInfo);
        }
        
        protected void FireCreated(DatabaseTypeInfo ti)
        {
            OnObjectCreated?.Invoke(this, new ObjectCreatedEventArgs(ti));
        }

        protected void FireModified(DatabaseTypeInfo ti, string action)
        {
            FireModified(ti, action, null);
        }

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
