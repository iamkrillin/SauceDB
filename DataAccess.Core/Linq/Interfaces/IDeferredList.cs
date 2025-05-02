using System.Collections;
using System.Collections.Generic;

namespace DataAccess.Core.Linq.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDeferredList : IList, IDeferLoadable
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeferredList<T> : IList<T>, IDeferredList
    {
    }
}
