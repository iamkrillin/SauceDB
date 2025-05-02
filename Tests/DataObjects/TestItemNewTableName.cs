using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    [TableName(TableName = "SomeNewTable")]
    public class TestItemNewTableName
    {
        public int id { get; set; }
        public string Something { get; set; }
    }
}
