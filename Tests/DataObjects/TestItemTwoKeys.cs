using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    public class TestItemTwoKeys
    {
        [Key]
        public string Key1 { get; set; }

        [Key]
        public string Key2 { get; set; }

        public string OtherField { get; set; }
    }
}
