using DataAccess.Core.Conversion;
using DataAccess.Core.Data;
using DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;

namespace DataAccess.SqlCompact
{
    public class SqlCompactDBConverter : StandardDBConverter
    {
        public override void CoerceValue(IDbDataParameter value)
        {
            if (value.Value != null)
            {
                Type t = value.Value.GetType();
                if (value.Value.GetType().Namespace.Equals("Microsoft.SqlServer.Types"))
                {
                    SqlCeParameter parm = (SqlCeParameter)value;
                    parm.SqlDbType = SqlDbType.Udt;
                    //parm.UdtTypeName = t.Name.Replace("Sql", "");
                }
            }
        }

        public override object CoerceValue(object value)
        {
            if (value != null)
            {
                if (value is TimeSpan)
                {
                    if (value is TimeSpan)
                        return ((TimeSpan)value).ToString();
                }
            }

            return value;
        }

        public override void PopulateMappings()
        {
            FieldMappings.Add(FieldType.Binary, new FieldMapInfo("nvarchar({0})", "200"));
            FieldMappings.Add(FieldType.Bool, new FieldMapInfo("bit"));
            FieldMappings.Add(FieldType.Char, new FieldMapInfo("nvarchar({0})", "1"));
            FieldMappings.Add(FieldType.UnicodeChar, new FieldMapInfo("nvarchar({0})", "1"));
            FieldMappings.Add(FieldType.Date, new FieldMapInfo("DATETIME"));
            FieldMappings.Add(FieldType.Int, new FieldMapInfo("int"));
            FieldMappings.Add(FieldType.Long, new FieldMapInfo("bigint"));
            FieldMappings.Add(FieldType.Real, new FieldMapInfo("real"));
            FieldMappings.Add(FieldType.Money, new FieldMapInfo("Money"));
            FieldMappings.Add(FieldType.String, new FieldMapInfo("NVARCHAR({0})", "200"));
            FieldMappings.Add(FieldType.Default, new FieldMapInfo("NVARCHAR({0})", "200"));
            FieldMappings.Add(FieldType.Time, new FieldMapInfo("NVARCHAR(20)"));
            FieldMappings.Add(FieldType.UnicodeString, new FieldMapInfo("NVARCHAR({0})", "200"));
            FieldMappings.Add(FieldType.Text, new FieldMapInfo("ntext"));
            FieldMappings.Add(FieldType.UnicodeText, new FieldMapInfo("ntext"));
        }

        public override string MapFieldType(FieldType type, DataFieldInfo dfi)
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
                    return "nvarchar(200)";
                }
            }
        }

        protected override string ResolveLength(int? userSpecified, string @default)
        {
            if (userSpecified.HasValue && userSpecified.Value == Int32.MaxValue)
                return "MAX";
            else
                return base.ResolveLength(userSpecified, @default);
        }

        public override string MapType(Type type, DataFieldInfo dfi)
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
                    return "nvarchar(200)";
            }
        }
    }
}
