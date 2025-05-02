using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    public class TestItemIgnoredField
    {
        public int id { get; set; }
        public string Something { get; set; }

        [IgnoredField]
        public string Ignored { get; set; }
    }
}
