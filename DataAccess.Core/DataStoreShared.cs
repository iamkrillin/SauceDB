using DataAccess.Core.Data;
using DataAccess.Core.Events;
using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


#if(async)
namespace DataAccess.Async
{
    public partial class AsyncDataStore
#else
namespace DataAccess.Core
{
    public partial class DataStore
#endif
    {
        /// <summary>
        /// Creates an object and inits the primary key field
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected virtual object CreateObjectSetKey(Type item, object key)
        {
            return CreateObjectSetKey(item, key, TypeInformationParser.GetTypeInfo(item));
        }

        /// <summary>
        /// Creates an object and inits the primary key field
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="key">The key.</param>
        /// <param name="ti">The type info.</param>
        /// <returns></returns>
        protected static object CreateObjectSetKey(Type item, object key, DatabaseTypeInfo ti)
        {
            object toReturn = item.GetConstructor(new Type[] { }).Invoke(new object[] { });
            ti.PrimaryKeys[0].Setter(toReturn, key, null);
            return toReturn;
        }

        #region Events
        /// <summary>
        /// This event will fire anytime an object is being loaded
        /// </summary>
        public event EventHandler<ObjectInitializedEventArgs> ObjectLoaded;

        /// <summary>
        /// This event will fire just before an object is deleted
        /// </summary>
        public event EventHandler<ObjectDeletingEventArgs> ObjectDeleting;

        /// <summary>
        /// This event will fire just after an object is deleted
        /// </summary>
        public event EventHandler<ObjectDeletingEventArgs> ObjectDeleted;

        /// <summary>
        /// This event will fire just before an object is updated
        /// </summary>
        public event EventHandler<ObjectUpdatingEventArgs> ObjectUpdating;

        /// <summary>
        /// This event will fire just after an object is updated
        /// </summary>
        public event EventHandler<ObjectUpdatingEventArgs> ObjectUpdated;

        /// <summary>
        /// This event will fire just before an object is inserted
        /// </summary>
        public event EventHandler<ObjectInsertingEventArgs> ObjectInserting;

        /// <summary>
        /// This event will fire just after an object is inserted
        /// </summary>
        public event EventHandler<ObjectInsertingEventArgs> ObjectInserted;

        internal void FireObjectLoaded(object item)
        {
            if (ObjectLoaded != null)
                ObjectLoaded(this, new ObjectInitializedEventArgs(item));
        }

        internal void FireObjectInserted(object item, bool result)
        {
            if (result && ObjectInserted != null)
                ObjectInserted(this, new ObjectInsertingEventArgs(item));
        }

        internal void FireObjectUpdated(object item, bool result)
        {
            if (ObjectUpdated != null && result)
                ObjectUpdated(this, new ObjectUpdatingEventArgs(item));
        }

        internal void FireObjectDeleted(object item, bool result)
        {
            if (result && ObjectDeleted != null)
                ObjectDeleted(this, new ObjectDeletingEventArgs(item));
        }

        internal bool CheckObjectUpdating(object item)
        {
            if (ObjectUpdating != null)
            {
                ObjectUpdatingEventArgs args = new ObjectUpdatingEventArgs(item);
                ObjectUpdating(this, args);
                if (args.Cancel)
                    return false;
            }
            return true;
        }
        
        internal bool CheckObjectDeleting(object item)
        {
            if (ObjectDeleting != null)
            {
                ObjectDeletingEventArgs args = new ObjectDeletingEventArgs(item);
                ObjectDeleting(this, args);
                if (args.Cancel)
                    return false;
            }
            return true;
        }
        
        internal bool CheckObjectInserting(object item)
        {
            if (ObjectInserting != null)
            {
                ObjectInsertingEventArgs args = new ObjectInsertingEventArgs(item);
                ObjectInserting(this, args);
                if (args.Cancel)
                    return false;
            }
            return true;
        }

        #endregion
    }
}
