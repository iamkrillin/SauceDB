using DataAccess.Core;
using DataAccess.Core.Data;
using DataAccess.Core.Events;
using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dynamic
{
    public class DynamicDataStore : DataStore
    {
        DynamicTypeParser _dynamic;

        public DynamicDataStore(IDataStore dstore)
            : base(dstore.Connection)
        {
            _dynamic = new DynamicTypeParser(this);
            TypeInformationParser = _dynamic;
        }

        public bool SaveObject(dynamic obj, string tablename)
        {
            TypeInfo ti = _dynamic.GetTypeInfo(obj, tablename);



            return false;
        }
    }
}
