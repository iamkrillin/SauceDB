using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    public class TestItemDifferentFieldType
    {
        [DataField(FieldTypeString = "varchar(1000)", FieldType = FieldType.UserString)]
        public string Foo { get; set; }
    }
}
