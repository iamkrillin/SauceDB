using System.Data;

namespace DataAccess.Core.Data
{
    /// <summary>
    /// Holds Command Parameters
    /// </summary>
    public class ParameterData
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="parm">The parameter object</param>
        /// <param name="field">The field its for</param>
        public ParameterData(IDbDataParameter parm, string field)
        {
            this.Parameter = parm;
            this.Field = field;
        }

        /// <summary>
        /// The parameter object
        /// </summary>
        public IDbDataParameter Parameter { get; set; }

        /// <summary>
        /// The field its for
        /// </summary>
        public string Field { get; set; }
    }
}
