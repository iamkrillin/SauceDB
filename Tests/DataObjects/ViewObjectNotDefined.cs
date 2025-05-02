using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    [View(TableName = "ViewObjects")]
    public class ViewObjectNotDefined
    {
        public string TestItemPrimaryKeyName { get; set; }
    }

}
