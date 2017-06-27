using DataAccess.Core;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using DataAccess.Core.ObjectValidators;
using DataAccess.Core.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DataAccess.Migrations
{
    public abstract class DBMigrator
    {
        private ModifyTableValidator _tables;
        private IDatastoreObjectValidator _views;
        private IDataStore _dstore;
        public bool ExceptionOnError { get; set; }

        public DBMigrator()
        {
            RetrieveDataStore();
        }

        protected abstract IDataStore GetDataStore();
        protected void RetrieveDataStore()
        {
            _dstore = GetDataStore().GetNewInstance();

            _tables = new ModifyTableValidator(_dstore);
            _tables.CanAddColumns = true;
            _tables.CanRemoveColumns = true;
            _tables.CanUpdateColumns = true;


            _views = _dstore.SchemaValidator.ViewValidator;
            if (_views == null) _views = new ViewValidator(_dstore);

            DatabaseTypeInfo ti = _dstore.TypeInformationParser.GetTypeInfo(typeof(RanMigration));

            if (_dstore.SchemaValidator.TableValidator == null || !_dstore.SchemaValidator.TableValidator.CanAddColumns)
                _tables.ValidateObject(ti);

            _dstore.SchemaValidator = new DoesNothingValidator();
        }

        public virtual List<Type> ResolveMigrations(Assembly asmb)
        {
            return asmb.GetTypes().Where(r => r.IsSubclassOf(typeof(DBMigration))).ToList();
        }

        protected virtual void UpdateColumn(Expression expression)
        {
            ColumnExpression exp = new ExpressionParser().ParseExpression(expression);
            DatabaseTypeInfo ti = _dstore.TypeInformationParser.GetTypeInfo(exp.Object);
            Console.WriteLine("Preparing to Update Column {0} On {1}", exp.Column, _dstore.GetTableName(exp.Object));
            _tables.ModifyColumn(ti.DataFields.Where(r => r.PropertyName.Equals(exp.Column)).First(), ti);
        }

        protected virtual void RemoveColumnFromTable(ColumnData columnData)
        {
            DatabaseTypeInfo ti = _dstore.TypeInformationParser.GetTypeInfo(columnData.Table);
            Console.WriteLine("Preparing to Remove Column {0} From {1}", columnData.Column, _dstore.GetTableName(columnData.Table));
            _tables.RemoveColumn(new DataFieldInfo() { FieldName = columnData.Column }, ti);
        }

        protected virtual void AddColumnToTable(Expression expression)
        {
            ColumnExpression exp = new ExpressionParser().ParseExpression(expression);
            DatabaseTypeInfo ti = _dstore.TypeInformationParser.GetTypeInfo(exp.Object);
            Console.WriteLine("Preparing to Add Column {0} To {1}", exp.Column, _dstore.GetTableName(exp.Object));
            DataFieldInfo toAdd = ti.DataFields.Where(r => r.PropertyName.Equals(exp.Column)).First();

            if (toAdd.PrimaryKeyType != null)//need to check for foreign keys and make sure those tables exist..
                _tables.ValidateObject(_dstore.TypeInformationParser.GetTypeInfo(toAdd.PrimaryKeyType));

            _tables.AddColumn(toAdd, ti);
        }

        protected virtual void AddTable(Type type)
        {
            DatabaseTypeInfo ti = _dstore.TypeInformationParser.GetTypeInfo(type);
            Console.WriteLine("Adding Table {0}", _dstore.GetTableName(type));

            //need to check for foreign keys and make sure those tables exist..
            foreach (DataFieldInfo v in ti.DataFields)
            {
                if (v.PrimaryKeyType != null)
                    _tables.ValidateObject(_dstore.TypeInformationParser.GetTypeInfo(v.PrimaryKeyType));
            }

            _tables.CreateNewTable(ti); //this will also run the ontablecreate stuff w00t
        }

        public void RunMigrations(Assembly asmb)
        {
            List<Type> migrations = ResolveMigrations(asmb);
            migrations = SortMigrations(migrations, null);
            RemovePastWork(migrations);

            List<Action> work = GetWork(migrations);
            work.Add(() => HandleViews(asmb));
            work.Add(() => HandleStoredProcs(asmb));

            for (int i = 0; i < work.Count; i++)
            {
                try
                {
                    Console.WriteLine("Processing Migration Step {0}/{1}", i + 1, work.Count);
                    work[i]();
                }
                catch (Exception ex)
                {
                    if (ex is QueryException || ex is DataStoreException)
                    {
                        if (!ExceptionOnError)
                        {
                            Console.WriteLine("Error Running Migration Step {0}: {1}, You can stop now or press enter to continue.", i + 1, ex.Message);
                            Console.ReadLine();
                            continue;
                        }
                    }

                    throw;
                }
            }

            StoreWork(migrations);

            Console.WriteLine("All Done!");
        }

        protected virtual void HandleViews(Assembly asmb)
        {
            string viewResource = asmb.GetManifestResourceNames().First(r => r.EndsWith("views.txt", StringComparison.InvariantCultureIgnoreCase));
            List<string> views = asmb.LoadResource(viewResource).Split(Environment.NewLine.ToCharArray()).ToList();
            List<ViewData> items = new List<ViewData>();

            Console.WriteLine("Parsing View File....");
            foreach (string s in views)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    string[] parts = s.Split('|');
                    ViewData data = new ViewData(parts[0], parts[1]);

                    if (parts.Length == 3)
                        data.Options = parts[2];

                    items.Add(data);
                }
            }

            var dbViews = _views.GetObjects().OrderBy(r=> r.Name).ToList();
            foreach (var item in items)
            {
                Console.Write("Working on view {0}: ", item.Name);
                //see if we have a view that matches this one already present in the DB.

                string resource = asmb.GetManifestResourceNames().Single(r => r.EndsWith(item.Script));
                if (string.IsNullOrEmpty(resource))
                    throw new Exception(string.Format("Resource for {0} was not found", item.Script));

                string script = asmb.LoadResource(resource);
                string verb;

                if (ViewExists(dbViews, item.Name))
                {
                    Console.WriteLine("Updating...");
                    verb = "ALTER";
                }
                else
                {
                    Console.WriteLine("Creating...");
                    verb = "CREATE";
                }

                RunCommand($"{verb} VIEW {item.Name} {item.Options} as {script}");
            }
        }

        private bool ViewExists(List<DBObject> items, string name)
        {
            foreach (DBObject v in items)
            {
                string fullName = v.Schema.Equals("dbo", StringComparison.InvariantCultureIgnoreCase) ? v.Name : string.Concat(v.Schema, ".", v.Name);
                if (fullName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        private void RunScript(Assembly asmb, string script)
        {
            Console.WriteLine("Running Script {0}...", script);
            RunCommand(asmb.LoadResource(script));
        }

        private void RunCommand(string cmdText)
        {
            IDbCommand cmd = _dstore.Connection.GetCommand();
            cmd.CommandText = cmdText;
            ExecuteCommand(cmd);
        }

        protected virtual void HandleStoredProcs(Assembly asmb)
        {
            Console.WriteLine("Updating Stored Procedures...");
            List<string> procs = asmb.GetManifestResourceNames().Where(r => r.ToUpper().Contains("MIGRATIONS.STOREDPROCS")).ToList();

            RemoveStoredProcedures(procs);
            AddStoredProcedures(procs, asmb);
        }

        private void AddStoredProcedures(List<string> procs, Assembly asmb)
        {
            foreach (string script in procs.Where(r => r.Contains("_")))
                RunScript(asmb, script);

            foreach (string script in procs.Where(r => !r.Contains("_")))
                RunScript(asmb, script);
        }

        private void RemoveStoredProcedures(List<string> procs)
        {
            foreach (string name in procs)
            {
                string toDrop = ResolveName(name);

                try
                {
                    Console.WriteLine("Removing Stored Procedure {0}...", toDrop);
                    IDbCommand cmd = _dstore.Connection.GetCommand();
                    cmd.CommandText = string.Format("DROP PROCEDURE {0};", toDrop);
                    ExecuteCommand(cmd);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error with view {0} - {1}", toDrop, ex.Message);
                }
            }
        }

        private string ResolveName(string name)
        {
            string[] parts = name.Split('.');
            return parts[parts.Length - 2];
        }

        protected virtual void RemovePastWork(List<Type> migrations)
        {
            foreach (Type t in migrations.ToList())
            {
                if (_dstore.Query<RanMigration>().Where(r => r.Type.Equals(t.ToString())).FirstOrDefault() != null)
                {
                    migrations.Remove(t);
                }
            }
        }

        protected virtual void StoreWork(List<Type> migrations)
        {
            foreach (Type t in migrations)
            {
                _dstore.InsertObject(new RanMigration()
                {
                    Date = DateTime.Now,
                    Type = t.ToString()
                });
            }
        }

        protected virtual List<Action> GetWork(List<Type> migrations)
        {
            List<Action> work = new List<Action>();
            foreach (Type t in migrations)
            {
                DBMigration mig = (DBMigration)t.GetConstructor(Type.EmptyTypes).Invoke(null);
                CheckFunction(mig.AddTable, work, AddTable);
                CheckFunction(mig.AddTables, work, AddTable);

                CheckFunction(mig.AddColumn, work, AddColumnToTable);
                CheckFunction(mig.AddColumns, work, AddColumnToTable);

                CheckFunction(mig.RemoveColumn, work, RemoveColumnFromTable);
                CheckFunction(mig.RemoveColumns, work, RemoveColumnFromTable);

                CheckFunction(mig.UpdateColumn, work, UpdateColumn);
                CheckFunction(mig.UpdateColumns, work, UpdateColumn);

                CheckFunction(mig.RunCommand, work, ExecuteCommand);
                CheckFunction(mig.RunCommands, work, ExecuteCommand);

                CheckFunction(mig.RunTextCommand, work, ExecuteCommand);
                CheckFunction(mig.RunTextCommands, work, ExecuteCommand);

                work.Add(() =>
                {
                    try
                    {
                        mig.WorkWithDataStore(_dstore);
                    }
                    catch (NotImplementedException ex)
                    {//safe to ignore                        
                    }
                });
            }

            return work;
        }

        private void CheckFunction<T>(Func<List<T>> Func, List<Action> WorkList, Action<T> WorkAction)
        {
            try
            {
                List<T> workdata = Func();
                foreach (var v in workdata)
                    WorkList.Add(() => WorkAction(v));
            }
            catch (NotImplementedException ex)
            { //ignore     
            }
        }

        private void CheckFunction<T>(Func<T> Func, List<Action> WorkList, Action<T> WorkAction)
        {
            try
            {
                T workdata = Func();
                WorkList.Add(() => WorkAction(workdata));
            }
            catch (NotImplementedException ex)
            { //ignore     
            }
        }

        protected virtual void ExecuteCommand(string dbCommand)
        {
            IDbCommand cmd = _dstore.Connection.GetCommand();
            cmd.CommandText = dbCommand;
            ExecuteCommand(cmd);
        }

        protected virtual void ExecuteCommand(IDbCommand dbCommand)
        {
            _dstore.ExecuteCommand(dbCommand);
        }

        protected virtual List<Type> SortMigrations(List<Type> migrations, Type current)
        {
            List<Type> sorted = new List<Type>();

            if (current == null)
            {
                foreach (Type t in migrations.ToList())
                {
                    object[] items = t.GetCustomAttributes(typeof(RequiredAfter), false);
                    if (items.Length == 0)
                    {
                        sorted.Add(t);
                        migrations.Remove(t);
                        sorted.AddRange(SortMigrations(migrations, t));
                    }
                }
            }
            else
            {
                foreach (Type t in migrations.ToList())
                {
                    RequiredAfter after = t.GetCustomAttributes(typeof(RequiredAfter), false).FirstOrDefault() as RequiredAfter;
                    if (after != null)
                    {
                        if (after.RunAfter == current)
                        {
                            sorted.Add(t);
                            migrations.Remove(t);
                            sorted.AddRange(SortMigrations(migrations, t));
                        }
                    }
                }
            }

            return sorted;
        }
    }
}
