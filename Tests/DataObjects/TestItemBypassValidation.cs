using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    [BypassValidation]
    public class TestItemBypassValidation
    {
        public int id { get; set; }
        public string Something { get; set; }
    }
}
