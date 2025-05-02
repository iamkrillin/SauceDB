using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    [TableName(TableName = "TestItemThreeFields")]
    public class TestItemThreeFieldsMinusOne
    {
        public string id { get; set; }
        public string something { get; set; }
    }
}
