using DataAccess.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.DatabaseTests.DataObjects
{
    public class TestObjectWithFieldAlias
    {
        public int ID { get; set; }

        [DataField(FieldName="AnotherField")]
        public string Field { get; set; }
    }
}
