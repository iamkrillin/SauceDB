using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace DataAccess.Samples
{
    public class Student
    {
        [Key(FieldName = "StudentID")] //this field is aliased and will appear in the db as "StudentID"
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [IgnoredField] //this field will never make it to the datastore
        public string Initials { get; set; }

        [AdditionalInit]
        private void SetInitials()
        {
            Initials = string.Concat(FirstName[0], LastName[1]);
        }
    }
}
