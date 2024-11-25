using DataAccess.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.DataObjects
{
    public class TestObjectMaxTextField
    {
        public int ID { get; set; }

        [DataField(FieldLength = int.MaxValue)]
        public string Text { get; set; }
    }
}
