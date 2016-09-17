using DataAccess.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core.Conversion
{
    public abstract class StandardDBConverter : IConvertToDatastore
    {
        public Dictionary<FieldType, FieldMapInfo> FieldMappings = new Dictionary<FieldType, FieldMapInfo>();

        public StandardDBConverter()
        {
            PopulateMappings();
        }

        public abstract void PopulateMappings();

        public virtual void CoerceValue(System.Data.IDbDataParameter value)
        {

        }

        public virtual object CoerceValue(object value)
        {
            return value;
        }

        /// <summary>
        /// Resolves the length for a field
        /// </summary>
        /// <param name="userSpecified">The user specified length</param>
        /// <param name="default">The default length</param>
        /// <returns></returns>
        protected virtual string ResolveLength(int? userSpecified, string @default)
        {
            return userSpecified.HasValue ? userSpecified.Value.ToString() : @default;
        }

        public virtual string MapFieldType(FieldType type, DataFieldInfo dfi)
        {
            if (type == FieldType.UserString)
            {
                return dfi.DataFieldString;
            }
            else
            {
                if (dfi.DataFieldType != FieldType.Default)
                {
                    FieldMapInfo info = FieldMappings[dfi.DataFieldType];
                    return string.Format(info.TypeString, ResolveLength(dfi.FieldLength, info.DefaultLength));
                }
                else if (FieldMappings.ContainsKey(type))
                {
                    FieldMapInfo info = FieldMappings[type];
                    return string.Format(info.TypeString, ResolveLength(dfi.FieldLength, info.DefaultLength));
                }
                else
                {
                    return "varchar(200)";
                }
            }
        }

        public virtual string MapType(Type type, Data.DataFieldInfo dfi)
        {
            string name = type.Name.ToUpper();

            switch (name)
            {
                case "STRING":
                    return MapFieldType(FieldType.String, dfi);
                case "INT32":
                    return MapFieldType(FieldType.Int, dfi);
                case "INT64":
                    return MapFieldType(FieldType.Long, dfi);
                case "SINGLE":
                case "FLOAT":                
                case "DOUBLE":
                    return MapFieldType(FieldType.Real, dfi);
                case "DECIMAL":
                    return MapFieldType(FieldType.Money, dfi);
                case "BYTE[]":
                    return MapFieldType(FieldType.Binary, dfi);
                case "BOOLEAN":
                    return MapFieldType(FieldType.Bool, dfi);
                case "DATETIME":
                case "DATETIMEOFFSET":
                    return MapFieldType(FieldType.Date, dfi);
                case "TIMESPAN":
                    return MapFieldType(FieldType.Time, dfi);
                case "CHAR":
                    return MapFieldType(FieldType.Char, dfi);
                default:
                    return MapFieldType(FieldType.Default, dfi);
            }
        }
    }
}
