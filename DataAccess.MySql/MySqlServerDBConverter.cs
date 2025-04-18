using DataAccess.Core.Attributes;
using DataAccess.Core.Conversion;
using DataAccess.Core.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DataAccess.MySql
{
    public class MySqlServerDBConverter : StandardDBConverter
    {
        public override void CoerceValue(IDbDataParameter value)
        {
            if (value.Value != null)
            {
                Type t = value.Value.GetType();
                if (value.Value.GetType().Namespace.Equals("MySql.Data.Types"))
                {
                    MySqlParameter parm = (MySqlParameter)value;
                    parm.MySqlDbType = MySqlDbType.Geometry;
                }
            }
        }

        public override void PopulateMappings()
        {
            FieldMappings.Add(FieldType.Binary, new FieldMapInfo("LONGBLOB"));
            FieldMappings.Add(FieldType.Bool, new FieldMapInfo("TINYINT({0})", "1"));
            FieldMappings.Add(FieldType.Char, new FieldMapInfo("VARCHAR({0})", "1"));
            FieldMappings.Add(FieldType.UnicodeChar, new FieldMapInfo("VARCHAR({0}) CHARSET utf8", "1"));
            FieldMappings.Add(FieldType.Date, new FieldMapInfo("DATETIME"));
            FieldMappings.Add(FieldType.Int, new FieldMapInfo("INT"));
            FieldMappings.Add(FieldType.Long, new FieldMapInfo("BIGINT"));
            FieldMappings.Add(FieldType.Real, new FieldMapInfo("DOUBLE"));
            FieldMappings.Add(FieldType.String, new FieldMapInfo("VARCHAR({0})", "200"));
            FieldMappings.Add(FieldType.Money, new FieldMapInfo("NUMERIC"));
            FieldMappings.Add(FieldType.Default, new FieldMapInfo("VARCHAR({0})", "200"));
            FieldMappings.Add(FieldType.Time, new FieldMapInfo("TIME"));
            FieldMappings.Add(FieldType.UnicodeString, new FieldMapInfo("VARCHAR({0}) CHARSET utf8", "200"));
            FieldMappings.Add(FieldType.Text, new FieldMapInfo("LONGTEXT"));
            FieldMappings.Add(FieldType.UnicodeText, new FieldMapInfo("LONGTEXT CHARSET utf8"));
        }

        public override string MapFieldType(FieldType type, Core.Data.DataFieldInfo dfi)
        {
            if(dfi.FieldLength.HasValue && dfi.FieldLength.Value == Int32.MaxValue)
            {//they want to store lots o' text, lets change the type to a text field here since mysql has a row limit
                if(type == FieldType.String) return base.MapFieldType(FieldType.Text, dfi);
                if(type == FieldType.UnicodeString) return base.MapFieldType(FieldType.UnicodeText, dfi);
            }

            return base.MapFieldType(type, dfi);
        }

        public override string MapType(Type type, Core.Data.DataFieldInfo dfi)
        {
            if (type.Namespace.Equals("MySql.Data.Types"))
                return type.Name.Replace("MySql", "");
            else            
                return base.MapType(type, dfi);
        }
    }
}
