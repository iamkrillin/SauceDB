using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;
using System.Collections;
using DataAccess.Core.ObjectFinders;
using DataAccess.Core.Schema;
using DataAccess.Core.Execute;

namespace DataAccess.Xamarin.iOS
{
    /// <summary>
    /// Creates a new data store for SQLite
    /// </summary>
    public class iOSDataStore : DataStore
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="filename">The file name to use</param>
        public iOSDataStore(string filename)
        {
            this.Connection = new iOSDataConnection(filename);
            this.ExecuteCommands = new ExecuteCommands();
            this.SchemaValidator = new ModifySchemaValidator(this);
            this.TypeInformationParser = new TypeParser(this);
            this.ObjectFinder = new NoSchemaSupportObjectFinder();
        }

        /// <summary>
        /// SQLite doesn't support multiple inserts with one command, so this loops
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public override bool InsertObjects(IList items)
        {
            bool result = false;
            foreach (var v in items)
            {
                result = InsertObject(v);
                if (!result) break;
            }
            return result;
        }
    }
}
