using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    [TableName(TableName = "TestItemThreeFields")]
    public class TestItemThreeFieldsPlusOneForeign
    {
        public string id { get; set; }
        public string something { get; set; }
        public string something2 { get; set; }

        [DataField(PrimaryKeyType = typeof(TestItem))]
        public int Something3 { get; set; }
    }
}
