using DataAccess.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrations.Migrations
{
    [RequiredAfter(typeof(CreatePerson))]
    public class AddNullableFieldToTable : DBMigration
    {
        public override System.Linq.Expressions.Expression AddColumn()
        {
            return this.MakeExpression<PersonPlusNullable, int?>(r => r.FooBar);
        }
    }
}
