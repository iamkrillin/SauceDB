using DataAccess.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.DatabaseTests.DataObjects
{
    public class TestItemWithPredicate
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        [QueryPredicate]
        private static IQueryable<TestItemWithPredicate> QueryPredicate(IQueryable<TestItemWithPredicate> query)
        {
            return query.Where(r => r.IsDeleted == false);
        }
    }
}
