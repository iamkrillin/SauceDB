using DataAccess.Core.Linq.Enums;
using System;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// A custom expression node that represents a reference to a column in a SQL query
    /// </summary>
    public class ColumnExpression : DbExpression, IEquatable<ColumnExpression>
    {
        /// <summary>
        /// Gets the alias.
        /// </summary>
        public TableAlias Alias { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnExpression"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="queryType">Type of the query.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="name">The name.</param>
        public ColumnExpression(Type type, TableAlias alias, string name)
            : base(DbExpressionType.Column, type)
        {
            if (name == null) throw new ArgumentNullException("name");
            this.Alias = alias;
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
            return this.Alias.ToString() + ".C(" + this.Name + ")";
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Alias.GetHashCode() + Name.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        ///   </exception>
        public override bool Equals(object obj)
        {
            return Equals(obj as ColumnExpression);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the other parameter; otherwise, false.
        /// </returns>
        public bool Equals(ColumnExpression other)
        {
            return other != null && ((object)this) == (object)other || (Alias == other.Alias && Name == other.Name);
        }
    }
}
