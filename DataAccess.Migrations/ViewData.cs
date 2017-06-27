using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Migrations
{
    public class ViewData
    {
        public string Name { get; set; }
        public string Script { get; set; }
        public string Options { get; set; }

        public ViewData(string name, string script)
        {
            this.Name = name;
            this.Script = script;
            this.Options = "";
        }
    }
}
