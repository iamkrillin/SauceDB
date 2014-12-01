using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;

namespace DataAccess.SqlServer
{
    /// <summary>
    /// A type converter specific to SQL server
    /// </summary>
    public class SqlServerTypeConverter : StandardCLRConverter
    {
        /// <summary>
        /// Converts to type, handles SQL min date
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public override object ConvertToType(object data, Type type)
        {
            object result = base.ConvertToType(data, type);
            DateTime? val = result as DateTime?;

            if (val != null)
            {
                if (val.Value.Year <= 1753) //min date for sql server is 01-01-1753
                    result = null;
            }
            return result;
        }
    }
}
