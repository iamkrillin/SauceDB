using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

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
