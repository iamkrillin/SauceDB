using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq.Enums;
using DataAccess.Core.Linq.Common.Mapping;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// A custom expression node that represents a table reference in a SQL query
    /// </summary>
    public class TableExpression : AliasedExpression
    {
        /// <summary>
        /// Gets the entity.
        /// </summary>
        public MappingEntity Entity { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableExpression"/> class.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="name">The name.</param>
        public TableExpression(TableAlias alias, MappingEntity entity, string name)
            : base(DbExpressionType.Table, typeof(void), alias)
        {
            this.Entity = entity;
            this.Name = name;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "T(" + this.Name + ")";
        }
    }
}
