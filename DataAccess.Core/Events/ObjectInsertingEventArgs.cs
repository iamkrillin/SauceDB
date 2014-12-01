using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace DataAccess.Core.Events
{
    /// <summary>
    /// Fired after an object is inserted
    /// </summary>
    public class ObjectInsertingEventArgs : CancelEventArgs
    {
        /// <summary>
        ///the data being inserted
        /// </summary>
        public object Inserting { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectInsertingEventArgs" /> class.
        /// </summary>
        /// <param name="o">The o.</param>
        public ObjectInsertingEventArgs(object o)
        {
            Inserting = o;
        }
    }
}
