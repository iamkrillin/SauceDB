using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Data;
using System.Data;
using DataAccess.SqlServer;
using System.Threading.Tasks;
using System.Data.Common;

namespace DataAccess.DatabaseTests.Tests
{
    public static class InitHelper
    {
        public static void InitDataStore(this IDataStore dStore)
        {
            AttachEvents(dStore);
            DropObjects(dStore.SchemaValidator.ViewValidator, dStore, "VIEW");
            DropObjects(dStore.SchemaValidator.TableValidator, dStore, "TABLE");
        }

        public static void AttachEvents(IDataStore dStore)
        {
            dStore.ObjectDeleting += (s, arg) => Console.WriteLine("Deleting {0}", arg.Deleted.ToString());
            dStore.ObjectLoaded += (s, arg) => Console.WriteLine("Loaded {0}", arg.Item.ToString());
            dStore.ObjectUpdating += (s, arg) => Console.WriteLine("Updated {0}", arg.Updating.ToString());
            dStore.ExecuteCommands.CommandExecuting += (s, arg) => Console.WriteLine("Executing Command: {0}", arg.Command.CommandText);
            dStore.SchemaValidator.TableValidator.OnObjectCreated += (s, arg) => Console.WriteLine("Creating Table: {0}", arg.Data.TableName);
            dStore.SchemaValidator.ViewValidator.OnObjectCreated += (s, arg) => Console.WriteLine("Creating View: {0}", arg.Data.TableName);
            dStore.SchemaValidator.TableValidator.OnObjectModified += (s, arg) => Console.WriteLine(arg.Action);
            dStore.SchemaValidator.ViewValidator.OnObjectModified += (s, arg) => Console.WriteLine(arg.Action);
        }

        public static async Task DropObjects(IDatastoreObjectValidator valid, IDataStore dStore, string name)
        {
            IEnumerable<DBObject> data = valid.GetObjects().ToList();
            while (data.Count() != 0)
            {
                foreach (DBObject t in data)
                {
                    DbCommand cmd = dStore.Connection.GetCommand();
                    cmd.CommandText = string.Format("DROP {0} {1}", name, GetTableName(t, dStore), dStore);
                    try { await dStore.ExecuteCommand(cmd); }
                    catch { }
                }
                data = valid.GetObjects(true);
            }
        }

        public static string GetTableName(DBObject t, IDataStore dstore)
        {
            string toReturn = t.Name;
            if (dstore.Connection.GetType() == typeof(SqlServerConnection))
            {
                toReturn = string.Concat(t.Schema, ".", t.Name);
            }
            return toReturn;
        }
    }
}

