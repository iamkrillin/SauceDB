using DataAccess.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrations.Migrations
{
    [RequiredAfter(typeof(CreatePerson))]
    public class AddTableWithForeignKey : DBMigration
    {
        public override Type AddTable()
        {
            return typeof(Child);
        }
    }
}
