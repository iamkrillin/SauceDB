using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess.Core.Linq.Common.Language;

namespace DataAccess.Core.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public class DbQueryType : QueryType
    {
        //SqlDbType dbType;
        bool notNull;
        int length;
        short precision;
        short scale;

        /// <summary>
        /// Gets the type of the SQL db.
        /// </summary>
        /// <value>
        /// The type of the SQL db.
        /// </value>
        //public SqlDbType SqlDbType { get { return this.dbType; } }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public override int Length { get { return this.length; } }

        /// <summary>
        /// Gets a value indicating whether [not null].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [not null]; otherwise, <c>false</c>.
        /// </value>
        public override bool NotNull { get { return this.notNull; } }

        /// <summary>
        /// Gets the precision.
        /// </summary>
        public override short Precision { get { return this.precision; } }

        /// <summary>
        /// Gets the scale.
        /// </summary>
        public override short Scale { get { return this.scale; } }

        /// <summary>
        /// Gets the type of the db.
        /// </summary>
        /// <value>
        /// The type of the db.
        /// </value>
        //public DbType DbType { get { return DbTypeSystem.GetDbType(this.dbType); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbQueryType"/> class.
        /// </summary>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="notNull">if set to <c>true</c> [not null].</param>
        /// <param name="length">The length.</param>
        /// <param name="precision">The precision.</param>
        /// <param name="scale">The scale.</param>
        public DbQueryType(bool notNull, int length, short precision, short scale)
        {
            //this.dbType = dbType;
            this.notNull = notNull;
            this.length = length;
            this.precision = precision;
            this.scale = scale;
        }
    }
}
