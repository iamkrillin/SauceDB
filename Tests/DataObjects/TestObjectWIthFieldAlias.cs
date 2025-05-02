using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    public class TestObjectWithFieldAlias
    {
        public int ID { get; set; }

        [DataField(FieldName = "AnotherField")]
        public string Field { get; set; }
    }
}
