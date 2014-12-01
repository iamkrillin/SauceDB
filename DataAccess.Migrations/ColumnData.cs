using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Migrations
{
    public class ColumnData
    {
        public Type Table { get; set; }
        public string Column { get; set; }

        public ColumnData()
        {
            
        }

        public ColumnData(Type table, string column)
        {
            this.Table = table;
            this.Column = column;
        }
    }
}
