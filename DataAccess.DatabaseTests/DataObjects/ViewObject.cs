using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace DataAccess.DatabaseTests.DataObjects
{
    [BypassValidation]
    [View]
    public class ViewObject
    {
        public string TestItemPrimaryKeyName { get; set; }
    }    
}
