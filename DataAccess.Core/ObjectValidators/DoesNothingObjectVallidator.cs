using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Core.ObjectValidators
{
    public class DoesNothingObjectVallidator : IDatastoreObjectValidator
    {
        public bool CanRemoveColumns { get; set; }
        public bool CanAddColumns { get; set; }
        public bool CanUpdateColumns { get; set; }

        public event EventHandler<Events.ObjectCreatedEventArgs> OnObjectCreated;
        public event EventHandler<Events.ObjectModifiedEventArgs> OnObjectModified;

        public Task ValidateObject(TypeParser tparser, Data.DatabaseTypeInfo ti)
        {
            return Task.CompletedTask;
        }

        public IEnumerable<Data.DBObject> GetObjects(bool forceReload)
        {
            return null;
        }

        public IEnumerable<Data.DBObject> GetObjects()
        {
            return null;
        }

        public bool ObjectExistsInDataStore(DatabaseTypeInfo info)
        {
            return false;
        }
    }
}
