using DataAccess.Core.Interfaces;
using System;
using System.Linq;
using System.Reflection;

namespace DataAccess.Core
{
    /// <summary>
    /// Represents a function to be called on object for additional initialization
    /// </summary>
    [Serializable]
    public class AdditionalInitFunction
    {
        private bool _needsParm;
        private Func<object, object[], object> _function;

        /// <summary>
        /// Will be true if the method is static
        /// </summary>
        public bool StaticMethod { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="MethodInfo">The method info to invoke with</param>
        public AdditionalInitFunction(MethodInfo MethodInfo)
        {
            ParameterInfo[] parms = MethodInfo.GetParameters();
            if (parms.Length > 0)
            {
                if (parms.Length == 1)
                {
                    if (typeof(IDataStore).IsAssignableFrom(parms.First().ParameterType))
                        _needsParm = true;
                    else
                        throw new DataStoreException("Only one parameter is allowed on additional init functions (IDataStore)");
                }
                else
                {
                    throw new DataStoreException("Only one parameter is allowed on additional init functions (IDataStore)");
                }
            }
            else
            {
                _needsParm = false;
            }

            StaticMethod = MethodInfo.IsStatic;
            _function = MethodInfo.Invoke;
        }

        /// <summary>
        /// Invokes the additional init function on an object
        /// </summary>
        /// <param name="store">The IDatastore that loaded the object</param>
        /// <param name="item">The object</param>
        public void Invoke(IDataStore store, object item)
        {
            if (_needsParm)
                _function.Invoke(item, new object[] { store });
            else
                _function.Invoke(item, null);
        }
    }
}
