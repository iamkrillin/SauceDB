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
