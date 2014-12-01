using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;

namespace DataAccess.Core.Events
{
    /// <summary>
    /// Event arguments for when a table is created
    /// </summary>
    public class ObjectCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// The data that was used to create the table
        /// </summary>
        public TypeInfo Data { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="TableInfo"></param>
        public ObjectCreatedEventArgs(TypeInfo TableInfo)
        {
            this.Data = TableInfo;
        }
    }
}
