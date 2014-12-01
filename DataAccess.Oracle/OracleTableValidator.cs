using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;
using DataAccess.Core.ObjectValidators;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Data;
using System.Data;
using System.Reflection;

namespace DataAccess.Oracle
{
    public class OracleTableValidator : NoSchemaSupportTableValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerTableValidator"/> class.
        /// </summary>
        /// <param name="dstore">The dstore.</param>
        public OracleTableValidator(IDataStore dstore)
            : base(dstore)
        {
        }
    }
}
