using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    [BypassValidation]
    [View]
    public class ViewObject
    {
        public string TestItemPrimaryKeyName { get; set; }
    }
}
