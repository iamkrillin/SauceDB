using DataAccess.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrations
{
    [TableName(TableName="Persons")]
    public class PersonTwo : Person
    {
        public string MiddleName { get; set; }
    }
}
