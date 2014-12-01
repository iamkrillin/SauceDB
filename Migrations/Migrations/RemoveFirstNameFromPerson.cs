using DataAccess.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrations.Migrations
{
    [RequiredAfter(typeof(AddMiddleNameToPerson))]
    public class RemoveFirstNameFromPerson : DBMigration
    {
        public override ColumnData RemoveColumn()
        {
            return new ColumnData()
            {
                Column = "FirstName",
                Table = typeof(Person)
            };
        }
    }
}
