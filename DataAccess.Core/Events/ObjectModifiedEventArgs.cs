using DataAccess.Core.Data;
using System;

namespace DataAccess.Core.Events
{
    /// <summary>
    /// Event arguments for when a table is created
    /// </summary>
    public class ObjectModifiedEventArgs : EventArgs
    {
        /// <summary>
        /// The action taken on the object
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// The data that was used to create the table
        /// </summary>
        public DatabaseTypeInfo Data { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="TableInfo">The type info that triggered the change</param>
        /// <param name="Action">A short descritpion of what was done</param>
        public ObjectModifiedEventArgs(DatabaseTypeInfo TableInfo, string Action)
        {
            Data = TableInfo;
            this.Action = Action;
        }
    }
}
