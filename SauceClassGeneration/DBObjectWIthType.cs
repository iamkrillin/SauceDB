using DataAccess.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCodeHaven.SauceClassGeneration
{
    public class DBObjectWithType
    {
        public DBObject Object { get; set; }
        public ObjectType Type { get; set; }

        public DBObjectWithType(DBObject obj, ObjectType type)
        {
            this.Object = obj;
            this.Type = type;
        }
    }
}
