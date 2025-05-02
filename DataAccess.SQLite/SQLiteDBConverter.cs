using DataAccess.Core.Attributes;
using DataAccess.Core.Conversion;
using DataAccess.Core.Interfaces;

namespace DataAccess.SQLite
{
    public class SQLiteDBConverter : StandardDBConverter
    {
        public override void PopulateMappings()
        {
            FieldMappings.Add(FieldType.Binary, new FieldMapInfo("BLOB"));
            FieldMappings.Add(FieldType.Bool, new FieldMapInfo("BOOL"));
            FieldMappings.Add(FieldType.Char, new FieldMapInfo("VARCHAR"));
            FieldMappings.Add(FieldType.UnicodeChar, new FieldMapInfo("VARCHAR"));
            FieldMappings.Add(FieldType.Date, new FieldMapInfo("DATETIME"));
            FieldMappings.Add(FieldType.Int, new FieldMapInfo("INTEGER"));
            FieldMappings.Add(FieldType.Long, new FieldMapInfo("INTEGER"));
            FieldMappings.Add(FieldType.Real, new FieldMapInfo("DOUBLE"));
            FieldMappings.Add(FieldType.Money, new FieldMapInfo("DECIMAL"));
            FieldMappings.Add(FieldType.String, new FieldMapInfo("VARCHAR"));
            FieldMappings.Add(FieldType.Time, new FieldMapInfo("VARCHAR"));
            FieldMappings.Add(FieldType.UnicodeString, new FieldMapInfo("VARCHAR"));
            FieldMappings.Add(FieldType.Text, new FieldMapInfo("VARCHAR"));
            FieldMappings.Add(FieldType.UnicodeText, new FieldMapInfo("VARCHAR"));
            FieldMappings.Add(FieldType.Default, new FieldMapInfo("VARCHAR"));
        }
    }
}
