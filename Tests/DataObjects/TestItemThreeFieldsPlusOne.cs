using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    [TableName(TableName = "TestItemThreeFields")]
    public class TestItemThreeFieldsPlusOne
    {
        public string id { get; set; }
        public string something { get; set; }
        public string something2 { get; set; }
        public int Something3 { get; set; }
    }
}
