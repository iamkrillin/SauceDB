using DataAccess.Core.Attributes;
using DataAccess.Core.Conversion;
using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.PostgreSQL
{
    public class PostgreDBConverter : StandardDBConverter
    {
        public override void PopulateMappings()
        {
            FieldMappings.Add(FieldType.Binary, new FieldMapInfo("bytea[]"));
            FieldMappings.Add(FieldType.Bool, new FieldMapInfo("boolean"));
            FieldMappings.Add(FieldType.Char, new FieldMapInfo("character"));
            FieldMappings.Add(FieldType.UnicodeChar, new FieldMapInfo("character"));
            FieldMappings.Add(FieldType.Date, new FieldMapInfo("date"));
            FieldMappings.Add(FieldType.Int, new FieldMapInfo("INTEGER"));
            FieldMappings.Add(FieldType.Long, new FieldMapInfo("bigint"));
            FieldMappings.Add(FieldType.Real, new FieldMapInfo("real"));
            FieldMappings.Add(FieldType.String, new FieldMapInfo("character varying"));
            FieldMappings.Add(FieldType.Default, new FieldMapInfo("character varying"));
            FieldMappings.Add(FieldType.Time, new FieldMapInfo("INTERVAL"));
            FieldMappings.Add(FieldType.UnicodeString, new FieldMapInfo("character varying"));
            FieldMappings.Add(FieldType.Money, new FieldMapInfo("NUMERIC"));
            FieldMappings.Add(FieldType.Text, new FieldMapInfo("text"));
            FieldMappings.Add(FieldType.UnicodeText, new FieldMapInfo("text"));
        }
    }
}
