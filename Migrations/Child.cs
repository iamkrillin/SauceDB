using DataAccess.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrations
{
    public class Child
    {
        public int ID { get; set; }

        [DataField(PrimaryKeyType=typeof(Person))]
        public int Parent { get; set; }
        public string Name { get; set; }
    }
}
