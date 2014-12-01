using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DataAccess.Core.Events
{
    /// <summary>
    /// Fired when an object is about to be deleted
    /// </summary>
    public class ObjectDeletingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public object Deleted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="del"></param>
        public ObjectDeletingEventArgs(object del)
        {
            this.Deleted = del;
        }
    }
}
