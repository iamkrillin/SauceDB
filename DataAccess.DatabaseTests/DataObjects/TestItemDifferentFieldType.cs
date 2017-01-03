using DataAccess.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DatabaseTests.DataObjects
{
    public class TestItemDifferentFieldType
    {
        [DataField(FieldTypeString = "varchar(1000)", FieldType = FieldType.UserString)]
        public string Foo { get; set; }
    }
}
