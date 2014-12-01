using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Migrations
{
    /// <summary>
    /// Indicates that a script is requried to be ran after another
    /// </summary>
    public class RequiredAfter : Attribute
    {
        public Type RunAfter { get; set; }

        public RequiredAfter(Type RunAfter)
        {
            this.RunAfter = RunAfter;
        }
    }
}
