using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dynamic
{
    public class DynamicTypeParser : ITypeInformationParser
    {
        private IDataStore _dstore;

        public event EventHandler<Core.Events.TypeParsedEventArgs> OnTypeParsed;
        public ICacheProvider<Type, Core.Data.TypeInfo> Cache { get; set; }        

        public DynamicTypeParser(IDataStore dstore)
        {
            _dstore = dstore;
        }
       
        public IEnumerable<DataFieldInfo> GetTypeFields(Type dataType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataFieldInfo> GetPrimaryKeys(Type dataType)
        {
            throw new NotImplementedException();
        }

        public Core.Data.TypeInfo GetTypeInfo(Type type)
        {
            throw new NotImplementedException("Use GetTypeInfo(dynamic)");
        }

        public Core.Data.TypeInfo GetTypeInfo(dynamic obj, string tablename)
        {
            Dictionary<string, object> data = obj;

            Core.Data.TypeInfo toReturn = new Core.Data.TypeInfo(null);
            toReturn.DataFields = new List<DataFieldInfo>();

            foreach (string s in data.Keys)
            {
                DataFieldInfo dfi = new DataFieldInfo();
                Type ti = data[s].GetType();
                foreach (var pi in ti.GetProperties())
                {
                    dfi.PropertyType = pi.PropertyType;
                    dfi.FieldName = pi.Name;
                    dfi.EscapedFieldName = string.Concat(_dstore.Connection.LeftEscapeCharacter, dfi.FieldName, _dstore.Connection.RightEscapeCharacter);
                    dfi.Setter = pi.SetValue;
                    dfi.Getter = pi.GetValue;
                    dfi.PropertyName = pi.Name;
                    toReturn.DataFields.Add(dfi);
                }

                toReturn.DataFields.Add(new DataFieldInfo()
                {
                    PropertyType = typeof(Int32),
                    FieldName = "ID",
                    EscapedFieldName = string.Concat(_dstore.Connection.LeftEscapeCharacter, "ID", _dstore.Connection.RightEscapeCharacter),
                    Setter = (a, b, c) => { },
                    Getter = (a, b) => { return null; },
                    PropertyName = "ID"
                });
            }

            _dstore.SchemaValidator.ValidateType(toReturn);
            return toReturn;
        }

        public Core.Data.TypeInfo GetTypeInfo(Type type, bool Validate)
        {
            throw new NotImplementedException();
        }
    }
}
