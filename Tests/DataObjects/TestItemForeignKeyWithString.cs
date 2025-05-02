using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    public class TestItemForeignKeyWithString
    {
        [DataField(SetOnInsert = true)]
        public string ID { get; set; }

        [DataField(PrimaryKeyType = typeof(TestItemPrimaryKey))]
        public string FKeyField { get; set; }
    }
}
