using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    public class TestItemSmallString
    {
        public int ID { get; set; }

        [DataField(FieldLength = 5)]
        public string SmallString { get; set; }
    }
}
