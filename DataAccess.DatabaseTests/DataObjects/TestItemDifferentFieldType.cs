using DataAccess.Core;

namespace DataAccess.DatabaseTests.DataObjects
{
    public class TestItemDifferentFieldType
    {
        [DataField(FieldTypeString = "varchar(1000)", FieldType = FieldType.UserString)]
        public string Foo { get; set; }
    }
}
