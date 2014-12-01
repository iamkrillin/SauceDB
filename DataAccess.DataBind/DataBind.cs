using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DataAccess
{
    public static class DataBind
    {
        public static Binding<T> BindCollection<T>(this IDataStore dstore, ObservableCollection<T> collection)
        {
            return new Binding<T>(dstore, collection);            
        }
    }
}
