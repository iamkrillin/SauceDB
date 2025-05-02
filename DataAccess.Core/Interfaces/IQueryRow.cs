using DataAccess.Core.Data.Results;

namespace DataAccess.Core.Interfaces
{
    /// <summary>
    /// Represents a row in a query set
    /// </summary>
    public interface IQueryRow
    {
        /// <summary>
        /// Returns if a field is present and available for use
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool FieldAvailable(string key);

        /// <summary>
        /// The raw field data
        /// </summary>
        FieldData[] FieldData { get; set; }

        /// <summary>
        /// Returns if a field has been mapped to a location in the result set
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        bool FieldHasMapping(string field);

        /// <summary>
        /// Finds a field in a result set, null if not found
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        int? FindField(string field);

        /// <summary>
        /// Returns the data for a field location
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        object GetData(int field);

        /// <summary>
        /// Returns the data for a field by name
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        object GetData(string field);


        /// <summary>
        /// Returns the data for a field location
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        object GetDataForRowField(int field);

        /// <summary>
        /// Returns the data for a field by name
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        object GetDataForRowField(string field);

        /// <summary>
        /// Makes all fields available for use
        /// </summary>
        void ResetUsed();

        /// <summary>
        /// Sets the backing data
        /// </summary>
        /// <param name="data"></param>
        void SetFieldData(object[] data);
    }
}
