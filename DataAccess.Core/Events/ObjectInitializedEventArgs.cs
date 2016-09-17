using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core.Events
{
    /// <summary>
    /// Event arguments for when an object is loading
    /// </summary>
    public class ObjectInitializedEventArgs : EventArgs
    {
        /// <summary>
        /// The item that was just loaded
        /// </summary>
        public object Item { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectInitializedEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public ObjectInitializedEventArgs(object item)
        {
            Item = item;
        }
    }
}
