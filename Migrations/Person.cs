using DataAccess.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrations
{
    public class Person
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
    }

    [TableName(TableName="Persons")]
    public class PersonPlusNullable : Person
    {
        public int? FooBar { get; set; }
    }
}
