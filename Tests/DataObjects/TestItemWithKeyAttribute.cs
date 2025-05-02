using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    public class TestItemWithKeyAttribute
    {
        [Key]
        public string Something { get; set; }
    }
}
