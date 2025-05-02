using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    [TableName(Schema = "NewSchema")]
    public class TestItemNewSchema
    {
        public int id { get; set; }
        public string Something { get; set; }
    }
}
