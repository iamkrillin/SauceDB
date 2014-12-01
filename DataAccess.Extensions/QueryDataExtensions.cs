using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;
using System.Dynamic;
using System.Data;
using DataAccess.Core.Data.Results;

namespace DataAccess.Core
{
    /// <summary>
    /// Some extensions to QueryData, mostly useful for ad hoc style queries
    /// </summary>
    public static class QueryDataExtensions
    {
        /// <summary>
        /// Returns a list of dynamic objects that represent the data
        /// </summary>
        /// <param name="qd">The data</param>
        /// <returns></returns>
        public static List<dynamic> ToDynamicList(this QueryData qd)
        {
            List<dynamic> toReturn = new List<dynamic>();
            if (qd.QuerySuccessful)
            {
                foreach (QueryRow o in qd)
                {
                    dynamic newRecord = new ExpandoObject();
                    var d = newRecord as IDictionary<string, object>;
                    foreach (string s in qd.GetFields())
                        d.Add(s, o.GetDataForRowField(s));

                    toReturn.Add(newRecord);
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Returns the query data as a DataTable
        /// </summary>
        /// <param name="qd">The data</param>
        /// <returns></returns>
        public static DataTable ToDataTable(this QueryData qd)
        {
            DataTable toReturn = new DataTable();
            qd.GetFields().ForEach(r => toReturn.Columns.Add(r));
            foreach(QueryRow o in qd)
            {
                DataRow dr = toReturn.NewRow();
                foreach (string s in qd.GetFields())
                    dr[s] = o.GetDataForRowField(s);

                toReturn.Rows.Add(dr);
            }


            return toReturn;
        }
    }
}
