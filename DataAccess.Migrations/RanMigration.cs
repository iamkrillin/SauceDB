using DataAccess.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Migrations
{
    [TableName(Schema="Sauce")]
    public class RanMigration
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
    }
}
