using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace DataAccess.DatabaseTests.DataObjects
{
    [View(TableName = "ViewObjects")]
    public class ViewObjectNotDefined
    {
        public string TestItemPrimaryKeyName { get; set; }
    }

}
