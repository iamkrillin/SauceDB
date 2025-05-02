using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    public class TestObjectSameFields
    {
        public int ID { get; set; }
        public int SortOrder { get; set; }

        [DataField(PrimaryKeyType = typeof(TestJoinObjectSameFields))]
        public int ForeignKey { get; set; }
    }

    public class TestJoinObjectSameFields
    {
        public int ID { get; set; }
        public int SortOrder { get; set; }
    }
}
