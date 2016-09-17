using DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.DatabaseTests.DataObjects
{
    public class TestObjectMaxTextField
    {
        public int ID { get; set; }

        [DataField(FieldLength=Int32.MaxValue)]
        public string Text { get; set; }
    }
}
