using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core.Interfaces
{
    public class FieldMapInfo
    {
        public string TypeString { get; set; }
        public string DefaultLength { get; set; }

        public FieldMapInfo()
        {

        }

        public FieldMapInfo(string type)
        {
            this.TypeString = type;
            DefaultLength = "";
        }

        public FieldMapInfo(string type, string defaultlength)
        {
            this.TypeString = type;
            this.DefaultLength = defaultlength;
        }
    }
}
