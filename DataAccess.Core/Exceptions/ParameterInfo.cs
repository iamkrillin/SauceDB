using System.Data;

namespace DataAccess.Core.Exceptions
{
    /// <summary>
    /// Represents a parameter that was attached to an IDbCommand
    /// </summary>
    public class ParameterInfo
    {
        /// <summary>
        /// The name of the parameter
        /// </summary>
        public string ParmName { get; set; }

        /// <summary>
        /// The value of the parameter
        /// </summary>
        public object ParmValue { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="v">The IDbDataParameter</param>
        public ParameterInfo(IDbDataParameter v)
        {
            this.ParmName = v.ParameterName;
            this.ParmValue = v.Value;
        }
    }
}
