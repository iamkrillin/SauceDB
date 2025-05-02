using DataAccess.Core.Data;
using System;

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
        public DatabaseTypeInfo Data { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="TableInfo"></param>
        public ObjectCreatedEventArgs(DatabaseTypeInfo TableInfo)
        {
            this.Data = TableInfo;
        }
    }
}
