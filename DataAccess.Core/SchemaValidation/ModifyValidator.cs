using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using System.Data;
using DataAccess.Core.Events;
using DataAccess.Core.ObjectValidators;

namespace DataAccess.Core.Schema
{
    /// <summary>
    /// Performs schema validation/modification of a data store table
    /// </summary>
    public class ModifySchemaValidator : SchemaValidator, ISchemaValidator
    {       
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifySchemaValidator"/> class.
        /// </summary>
        /// <param name="dStore">The data store to grab the connection from</param>
        public ModifySchemaValidator(IDataStore dStore)
            : base(dStore, new ModifyTableValidator(dStore), new ViewValidator(dStore))
        {
        }        
    }
}
