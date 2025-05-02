using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    [TableName(Schema = "SauceTest")]
    public class ItemWithSchema
    {
        public int id { get; set; }
        public string Something { get; set; }
        public TimeSpan TimeSpent { get; set; }
    }

    [TableName(Schema = "SauceTest")]
    public class ItemWithSchemaAnotherColumn
    {
        public int id { get; set; }
        public string Something { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public string Name { get; set; }
    }
}
