using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace DataAccess.Samples
{
    public class StudentClasses
    {
        [Key(PrimaryKeyType = typeof(Student))]
        public int Student { get; set; }

        [Key(PrimaryKeyType = typeof(Class))]
        public int Class { get; set; }
    }
}
