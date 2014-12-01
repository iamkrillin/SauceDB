using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core.Data.Results
{
    public class FieldData
    {
        public object Data { get; set; }
        public bool Used { get; set; }

        public FieldData(object data)
        {
            this.Data = data;
            Used = false;
        }
    }
}
