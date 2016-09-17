using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;
using DataAccess.Core.ObjectValidators;

namespace DataAccess.Core.Schema
{
    /// <summary>
    /// Performs schema validation/ throws exceptions when discrpencies are found
    /// </summary>
    public class NotifyValidator : SchemaValidator, ISchemaValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyValidator"/> class.
        /// </summary>
        /// <param name="dStore">The data store to grab the connection from</param>
        public NotifyValidator(IDataStore dStore)
            : base(dStore, new NotifyTableValidator(dStore), new ViewValidator(dStore))
        {
        }
    }
}
