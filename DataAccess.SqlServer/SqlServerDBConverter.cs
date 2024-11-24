using DataAccess.Core.Attributes;
using DataAccess.Core.Conversion;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace DataAccess.SqlServer
{
    public class SqlServerDBConverter : StandardDBConverter
    {
        public override void CoerceValue(IDbDataParameter value)
        {
            if (value.Value != null)
            {
                Type t = value.Value.GetType();
                if (value.Value.GetType().Namespace.Equals("Microsoft.SqlServer.Types"))
                {
                    SqlParameter parm = (SqlParameter)value;
                    parm.SqlDbType = SqlDbType.Udt;
                    parm.UdtTypeName = t.Name.Replace("Sql", "");
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

            if (name.Equals("DATETIMEOFFSET", StringComparison.CurrentCultureIgnoreCase))
                return "DATETIMEOFFSET(" + ResolveLength(dfi.FieldLength, "7") + ")";
            else if (type.Namespace.Equals("Microsoft.SqlServer.Types"))
                return type.Name.Replace("Sql", "");
            else
                return base.MapType(type, dfi);
        }

        public override void PopulateMappings()
        {
            FieldMappings.Add(FieldType.Binary, new FieldMapInfo("varbinary({0})", "MAX"));
            FieldMappings.Add(FieldType.Bool, new FieldMapInfo("bit"));
            FieldMappings.Add(FieldType.Char, new FieldMapInfo("varchar({0})", "1"));
            FieldMappings.Add(FieldType.UnicodeChar, new FieldMapInfo("nvarchar({0})", "1"));
            FieldMappings.Add(FieldType.Date, new FieldMapInfo("DATETIME"));
            FieldMappings.Add(FieldType.Int, new FieldMapInfo("int"));
            FieldMappings.Add(FieldType.Long, new FieldMapInfo("bigint"));
            FieldMappings.Add(FieldType.Money, new FieldMapInfo("Money"));
            FieldMappings.Add(FieldType.Real, new FieldMapInfo("real"));
            FieldMappings.Add(FieldType.String, new FieldMapInfo("varchar({0})", "200"));
            FieldMappings.Add(FieldType.Default, new FieldMapInfo("varchar({0})", "200"));
            FieldMappings.Add(FieldType.Time, new FieldMapInfo("time({0})", "7"));
            FieldMappings.Add(FieldType.UnicodeString, new FieldMapInfo("nvarchar({0})", "200"));
            FieldMappings.Add(FieldType.Text, new FieldMapInfo("TEXT"));
            FieldMappings.Add(FieldType.UnicodeText, new FieldMapInfo("NTEXT"));
        }
    }
}
