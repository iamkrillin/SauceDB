using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace DataAccess.DatabaseTests.DataObjects
{
    [TableName(Schema = "NewSchema")]
    public class TestItemNewSchema
    {
        public int id { get; set; }
        public string Something { get; set; }
    }
}
