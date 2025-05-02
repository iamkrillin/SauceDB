using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    public class TestObjectMaxTextField
    {
        public int ID { get; set; }

        [DataField(FieldLength = int.MaxValue)]
        public string Text { get; set; }
    }
}
