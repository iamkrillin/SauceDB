using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;

namespace DataAccess.Core.ObjectValidators
{
    /// <summary>
    /// This validator does nothing
    /// </summary>
    public class DoesNothingObjectVallidator : IDatastoreObjectValidator
    {
        /// <summary>
        /// Not used
        /// </summary>
        public bool CanRemoveColumns { get; set; }

        /// <summary>
        /// Not used
        /// </summary>
        public bool CanAddColumns { get; set; }

        /// <summary>
        /// Not used
        /// </summary>
        public bool CanUpdateColumns { get; set; }

        /// <summary>
        /// This will never fire
        /// </summary>
        public event EventHandler<Events.ObjectCreatedEventArgs> OnObjectCreated;
        
        /// <summary>
        /// this will never fire
        /// </summary>
        public event EventHandler<Events.ObjectModifiedEventArgs> OnObjectModified;

        /// <summary>
        /// This does nothing
        /// </summary>
        /// <param name="ti"></param>
        public void ValidateObject(Data.TypeInfo ti)
        {
        }

        /// <summary>
        /// Returns null
        /// </summary>
        /// <param name="forceReload"></param>
        /// <returns></returns>
        public IEnumerable<Data.DBObject> GetObjects(bool forceReload)
        {
            return null;
        }

        /// <summary>
        /// Returns null
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Data.DBObject> GetObjects()
        {
            return null;
        }

        public bool ObjectExistsInDataStore(TypeInfo info)
        {
            return false;
        }
    }
}
