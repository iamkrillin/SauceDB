using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq.Common;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Data;
using System.Reflection;
using DataAccess.Core.Linq.Common.Mapping;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Mapping
{
    /// <summary>
    /// Provides table and column mapping using sauce rules
    /// </summary>
    public class SauceMapping
    {
        private IDataStore _dstore;

        /// <summary>
        /// Initializes a new instance of the <see cref="SauceMapping"/> class.
        /// </summary>
        /// <param name="dStore">The d store.</param>
        public SauceMapping(IDataStore dStore)
        {
            _dstore = dStore;
        }

        /// <summary>
        /// Gets the table id.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public string GetTableId(Type type)
        {
            return _dstore.GetTableName(type);
        }

        /// <summary>
        /// Get the meta entity directly corresponding to the CLR type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual MappingEntity GetEntity(Type type)
        {
            return GetEntity(type, this.GetTableId(type));
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="tableId">The table id.</param>
        /// <returns></returns>
        public MappingEntity GetEntity(Type elementType, string tableId)
        {
            if (tableId == null)
                tableId = _dstore.GetTableName(elementType);
            return new BasicMappingEntity(elementType, tableId);
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <param name="contextMember">The context member.</param>
        /// <returns></returns>
        public MappingEntity GetEntity(MemberInfo contextMember)
        {
            Type elementType = TypeHelper.GetElementType(TypeHelper.GetMemberType(contextMember));
            return this.GetEntity(elementType);
        }

        /// <summary>
        /// Clones the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public object CloneEntity(MappingEntity entity, object instance)
        {
            var clone = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(entity.EntityType);
            foreach (var mi in GetMappedMembers(entity))
            {
                if (this.IsColumn(entity, mi))
                {
                    mi.SetValue(clone, mi.GetValue(instance));
                }
            }
            return clone;
        }

        /// <summary>
        /// Determines whether the specified entity is column.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="member">The member.</param>
        /// <returns>
        ///   <c>true</c> if the specified entity is column; otherwise, <c>false</c>.
        /// </returns>
        public bool IsColumn(MappingEntity entity, MemberInfo member)
        {
            return IsMapped(entity, member);
        }

        /// <summary>
        /// Determines whether the specified entity is mapped.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="member">The member.</param>
        /// <returns>
        ///   <c>true</c> if the specified entity is mapped; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMapped(MappingEntity entity, MemberInfo member)
        {
            TypeInfo ti = _dstore.TypeInformationParser.GetTypeInfo(entity.EntityType);
            DataFieldInfo field = ti.DataFields.Where(R => R.PropertyName.Equals(member.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            return field != null && field.LoadField;
        }

        /// <summary>
        /// Determines whether [is primary key] [the specified entity].
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="member">The member.</param>
        /// <returns>
        ///   <c>true</c> if [is primary key] [the specified entity]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPrimaryKey(MappingEntity entity, MemberInfo member)
        {
            TypeInfo ti = _dstore.TypeInformationParser.GetTypeInfo(entity.EntityType);
            return ti.PrimaryKeys.Where(R => R.PropertyName.Equals(member.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() != null;
        }

        /// <summary>
        /// Gets the primary key.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public object GetPrimaryKey(MappingEntity entity, object instance)
        {
            object firstKey = null;
            List<object> keys = null;
            TypeInfo ti = _dstore.TypeInformationParser.GetTypeInfo(entity.EntityType);
            foreach (DataFieldInfo v in ti.PrimaryKeys)
            {
                if (firstKey == null)
                {
                    firstKey = v.Getter(instance, null);
                }
                else
                {
                    if (keys == null)
                    {
                        keys = new List<object>();
                        keys.Add(firstKey);
                    }
                    keys.Add(v.Getter(instance, null));
                }
            }
            if (keys != null)
            {
                return new CompoundKey(keys.ToArray());
            }
            return firstKey;
        }

        /// <summary>
        /// Gets the primary key query.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="source">The source.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        public Expression GetPrimaryKeyQuery(MappingEntity entity, Expression source, Expression[] keys)
        {
            // make predicate
            ParameterExpression p = Expression.Parameter(entity.ElementType, "p");
            Expression pred = null;
            var idMembers = GetPrimaryKeyMembers(entity).ToList();
            if (idMembers.Count != keys.Length)
            {
                throw new InvalidOperationException("Incorrect number of primary key values");
            }
            for (int i = 0, n = keys.Length; i < n; i++)
            {
                MemberInfo mem = idMembers[i];
                Type memberType = TypeHelper.GetMemberType(mem);
                if (keys[i] != null && TypeHelper.GetNonNullableType(keys[i].Type) != TypeHelper.GetNonNullableType(memberType))
                {
                    throw new InvalidOperationException("Primary key value is wrong type");
                }
                Expression eq = Expression.MakeMemberAccess(p, mem).Equal(keys[i]);
                pred = (pred == null) ? eq : pred.And(eq);
            }
            var predLambda = Expression.Lambda(pred, p);

            return Expression.Call(typeof(Queryable), "SingleOrDefault", new Type[] { entity.ElementType }, source, predLambda);
        }

        /// <summary>
        /// Gets the mapped members.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public IEnumerable<MemberInfo> GetMappedMembers(MappingEntity entity)
        {
            Type type = entity.EntityType;
            HashSet<MemberInfo> members = new HashSet<MemberInfo>(type.GetFields().Cast<MemberInfo>().Where(m => this.IsMapped(entity, m)));
            members.UnionWith(type.GetProperties().Cast<MemberInfo>().Where(m => IsMapped(entity, m)));
            return members.OrderBy(m => m.Name);
        }

        /// <summary>
        /// Creates the mapper.
        /// </summary>
        /// <param name="translator">The translator.</param>
        /// <returns></returns>
        public QueryMapper CreateMapper(QueryTranslator translator)
        {
            return new BasicMapper(this, translator);
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public string GetTableName(MappingEntity entity)
        {
            return GetTableId(entity.EntityType);
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public string GetColumnName(MappingEntity entity, MemberInfo member)
        {
            TypeInfo ti = _dstore.TypeInformationParser.GetTypeInfo(entity.EntityType);
            DataFieldInfo field = ti.DataFields.Where(R => R.PropertyName.Equals(member.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            return field != null ? field.EscapedFieldName : "";
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <returns></returns>
        public string GetColumnName(MemberInfo memberInfo)
        {
            TypeInfo ti = _dstore.TypeInformationParser.GetTypeInfo(memberInfo.DeclaringType);
            DataFieldInfo field = ti.DataFields.Where(R => R.PropertyName.Equals(memberInfo.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            return field != null ? field.EscapedFieldName : "";
        }

        /// <summary>
        /// Gets the primary key members.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public virtual IEnumerable<MemberInfo> GetPrimaryKeyMembers(MappingEntity entity)
        {
            return this.GetMappedMembers(entity).Where(m => this.IsPrimaryKey(entity, m));
        }

        /// <summary>
        /// Determines whether a given expression can be executed locally. 
        /// (It contains no parts that should be translated to the target environment.)
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual bool CanBeEvaluatedLocally(Expression expression)
        {
            // any operation on a query can't be done locally
            ConstantExpression cex = expression as ConstantExpression;
            if (cex != null)
            {
                IQueryable query = cex.Value as IQueryable;
                if (query != null && query.Provider == this)
                    return false;
            }
            MethodCallExpression mc = expression as MethodCallExpression;
            if (mc != null && (mc.Method.DeclaringType == typeof(Enumerable) || mc.Method.DeclaringType == typeof(Queryable)))
            {
                return false;
            }
            if (expression.NodeType == ExpressionType.Convert && expression.Type == typeof(object))
                return true;
            return expression.NodeType != ExpressionType.Parameter && expression.NodeType != ExpressionType.Lambda;
        }
    }
}
