using System;
using System.Linq;
using System.Reflection;

namespace DataAccess.Core
{
    /// <summary>
    /// Represents a function to be called on object for a query predicate
    /// </summary>
    [Serializable]
    public class QueryPredicateFunction
    {
        private MethodInfo _mi;

        /// <summary>
        /// Will be true if the method is static
        /// </summary>
        public bool StaticMethod { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="MethodInfo">The method info to invoke with</param>
        public QueryPredicateFunction(MethodInfo MethodInfo)
        {
            ParameterInfo[] parms = MethodInfo.GetParameters();
            if (parms.Length != 1)
            {
                throw new DataStoreException("A query predicate requires one parameter, IQueryable<T>");
            }

            if (!MethodInfo.IsStatic)
            {
                throw new DataStoreException("A query predicate must have a function signature similar to 'private static IQueryable<T> QueryPredicate(IQueryable<T> query)'");
            }

            StaticMethod = MethodInfo.IsStatic;
            _mi = MethodInfo;

        }

        public IQueryable<T> Invoke<T>(IQueryable<T> query)
        {
            if (_mi.ContainsGenericParameters)
                return (IQueryable<T>)_mi.MakeGenericMethod(typeof(T)).Invoke(null, new object[] { query });
            else
                return (IQueryable<T>)_mi.Invoke(null, new object[] { query });
        }
    }
}
