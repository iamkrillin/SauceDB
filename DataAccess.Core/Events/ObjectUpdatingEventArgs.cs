using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DataAccess.Core.Events
{
    /// <summary>
    /// Fired when an object is being updated
    /// </summary>
    public class ObjectUpdatingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public object Updating { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        public ObjectUpdatingEventArgs(object o)
        {
            Updating = o;
        }
    }
}