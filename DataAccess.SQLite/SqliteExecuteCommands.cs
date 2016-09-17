using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess.Core.Events;
using DataAccess.Core;
using DataAccess.Core.Execute;

namespace DataAccess.SQLite
{
    /// <summary>
    /// A generic command executor
    /// </summary>
    public class SqliteExecuteCommands : ExecuteCommands
    {
        private Dictionary<string, Dictionary<string, int>> _mappings = new Dictionary<string, Dictionary<string, int>>();

        public override IQueryData ExecuteCommandQueryAction(IDbCommand command, IDataConnection connection, IDbConnection r)
        {
            SqLiteQueryData toReturn = new SqLiteQueryData();
            FireExecutingEvent(command, connection, r);
            using (IDataReader reader = command.ExecuteReader())
                MapReturnData(command, toReturn, reader);

            FireExecutedEvent(command, connection, r);
            toReturn.QuerySuccessful = true;
            return toReturn;
        }

        /// <summary>
        /// Maps return data to the query data
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="toReturn">To return.</param>
        /// <param name="reader">The reader.</param>
        protected void MapReturnData(IDbCommand command, SqLiteQueryData toReturn, IDataReader reader)
        {
            if (reader.Read())
            {
                if (!_mappings.ContainsKey(command.CommandText))
                {
                    _mappings[command.CommandText] = new Dictionary<string, int>();
                    DataTable schema = reader.GetSchemaTable();
                    if (schema != null)
                    {
                        for (int i = 0; i < schema.Rows.Count; i++)
                        {
                            toReturn.AddFieldMapping(schema.Rows[i]["ColumnName"].ToString(), i);
                        }
                    }

                    _mappings[command.CommandText] = toReturn.GetMappings();
                }

                toReturn.SetFieldMappings(_mappings[command.CommandText]);
                AddRecord(toReturn, reader);

                while (reader.Read())
                    AddRecord(toReturn, reader);
            }
        }

        /// <summary>
        /// Adds a record to the result set
        /// </summary>
        protected static void AddRecord(SqLiteQueryData toReturn, IDataReader reader)
        {
            object[] values = new object[reader.FieldCount];
            reader.GetValues(values);
            toReturn.AddRowData(values);
        }
    }
}
