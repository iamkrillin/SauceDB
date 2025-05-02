using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    public class TestItemPrivateInitMethod
    {
        public int ID { get; set; }
        public string Name { get; set; }

        [IgnoredField]
        public int Length { get; set; }

        [AdditionalInit]
        protected void SetLength()
        {
            Length = Name.Length;
        }
    }

    public class ChildClassWIithParentPrivateInitMethod : TestItemPrivateInitMethod
    {
        public string Foo { get; set; }
    }
}
