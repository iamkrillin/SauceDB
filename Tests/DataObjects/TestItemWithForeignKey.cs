using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    public class TestItemWithForeignKey
    {
        public int id { get; set; }
        public string Something { get; set; }

        [DataField(PrimaryKeyType = typeof(TestItem))]
        public int ForeignKey { get; set; }
    }
}
