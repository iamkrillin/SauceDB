using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace DataAccess.DatabaseTests.DataObjects
{
    public class TestItemForeignKeyWithString
    {
        [DataField(SetOnInsert = true)]
        public string ID { get; set; }

        [DataField(PrimaryKeyType = typeof(TestItemPrimaryKey))]
        public string FKeyField { get; set; }
    }
}
