using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    public class Groups
    {
        public int ID { get; set; }
        public string Group { get; set; }
    }

    [TableName(TableName = "Groups")]
    public class Groups2
    {
        public int ID { get; set; }
        public string Group { get; set; }
    }
}
