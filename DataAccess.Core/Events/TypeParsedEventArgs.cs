using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;

namespace DataAccess.Core.Events
{
    /// <summary>
    /// Fired after a type is parsed
    /// </summary>
    public class TypeParsedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeParsedEventArgs" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public TypeParsedEventArgs(DatabaseTypeInfo type)
        {
            this.Data = type;
        }

        /// <summary>
        /// The type data
        /// </summary>
        public DatabaseTypeInfo Data { get; set; }
    }
}
