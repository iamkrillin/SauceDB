using DataAccess.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Migrations.Migrations
{
    [RequiredAfter(typeof(CreatePerson))]
    public class AddMiddleNameToPerson : DBMigration
    {
        public override Expression AddColumn()
        {
            return this.MakeExpression<PersonTwo, string>(r => r.MiddleName);
        }
    }
}
