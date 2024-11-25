using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Types;

namespace Tests.DataObjects
{
    public class TestItemWithGeography
    {
        public int ID { get; set; }
        public SqlGeography Location { get; set; }
    }
}
