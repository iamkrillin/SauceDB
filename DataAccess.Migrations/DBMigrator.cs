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
using System.Text.RegularExpressions;

namespace DataAccess.Migrations
{
    public abstract class DBMigrator
    {
        private const string VIEW_MARKER = "MIGRATIONS.VIEWS";

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

            DatabaseTypeInfo ti = _dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(RanMigration));

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
            DatabaseTypeInfo ti = _dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(exp.Object);
            Console.WriteLine("Preparing to Update Column {0} On {1}", exp.Column, _dstore.GetTableName(exp.Object));
            _tables.ModifyColumn(ti.DataFields.Where(r => r.PropertyName.Equals(exp.Column)).First(), ti);
        }

        protected virtual void RemoveColumnFromTable(ColumnData columnData)
        {
            DatabaseTypeInfo ti = _dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(columnData.Table);
            Console.WriteLine("Preparing to Remove Column {0} From {1}", columnData.Column, _dstore.GetTableName(columnData.Table));
            _tables.RemoveColumn(new DataFieldInfo() { FieldName = columnData.Column }, ti);
        }

        protected virtual void AddColumnToTable(Expression expression)
        {
            ColumnExpression exp = new ExpressionParser().ParseExpression(expression);
            DatabaseTypeInfo ti = _dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(exp.Object);
            Console.WriteLine("Preparing to Add Column {0} To {1}", exp.Column, _dstore.GetTableName(exp.Object));
            DataFieldInfo toAdd = ti.DataFields.Where(r => r.PropertyName.Equals(exp.Column)).First();

            if (toAdd.PrimaryKeyType != null)//need to check for foreign keys and make sure those tables exist..
                _tables.ValidateObject(_dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(toAdd.PrimaryKeyType));

            _tables.AddColumn(toAdd, ti);
        }

        protected virtual void AddTable(Type type)
        {
            DatabaseTypeInfo ti = _dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(type);
            Console.WriteLine("Adding Table {0}", _dstore.GetTableName(type));

            //need to check for foreign keys and make sure those tables exist..
            foreach (DataFieldInfo v in ti.DataFields)
            {
                if (v.PrimaryKeyType != null)
                    _tables.ValidateObject(_dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(v.PrimaryKeyType));
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
            work.Add(() => HandleIndexes(asmb));
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

        private void HandleIndexes(Assembly asmb)
        {
            Console.WriteLine("Recreating Indexes...");
            List<string> indexes = asmb.GetManifestResourceNames().Where(r => r.ToUpper().Contains("MIGRATIONS.INDEXES")).ToList();

            foreach (string script in indexes)
                RunScript(asmb, script);
        }

        protected virtual List<ViewData> ResolveViews(Assembly asmb)
        {
            List<string> views = asmb.GetManifestResourceNames().Where(r => r.ToUpper().Contains(VIEW_MARKER)).ToList();
            List<ViewData> toReturn = new List<ViewData>();

            Regex optionsRegex = new Regex("#OPTIONS=(?<options>.*)");
            Regex requiredRegex = new Regex("#REQUIRED=(?<name>.*)");

            foreach (string viewResource in views)
            {
                string script = asmb.LoadResource(viewResource);
                string name = FigureName(viewResource);
                ViewData vd = new ViewData(name, script);

                if (optionsRegex.IsMatch(script))
                {
                    vd.Options = optionsRegex.Match(vd.Script).Groups["options"].Value.Replace("\r", "").Replace("\n", "");
                    vd.Script = optionsRegex.Replace(vd.Script, "");
                }

                if (requiredRegex.IsMatch(script))
                {
                    vd.Required = requiredRegex.Match(vd.Script).Groups["name"].Value.Replace("\r", "").Replace("\n", "");
                    vd.Script = requiredRegex.Replace(vd.Script, "");
                }

                toReturn.Add(vd);
            }

            return toReturn;
        }

        private string FigureName(string viewResource)
        {
            int start = viewResource.IndexOf(VIEW_MARKER, StringComparison.InvariantCultureIgnoreCase) + VIEW_MARKER.Length + 1; //also remove the . after the market text
            string name = viewResource.Substring(start, viewResource.Length - start);
            return name.Substring(0, name.IndexOf(".sql", StringComparison.InvariantCultureIgnoreCase));
        }

        protected virtual void HandleViews(Assembly asmb)
        {
            List<ViewData> items = ResolveViews(asmb);
            var dbViews = _views.GetObjects().OrderBy(r => r.Name).ToList();

            //may have to run the following multiple times, [some options can prevent updating/removal]
            bool needsRepeat = true;

            while (needsRepeat)
                needsRepeat = RemoveOptions(asmb, items, dbViews);

            items = SortList(items);

            foreach (ViewData item in items)
            {
                Console.WriteLine("Working on view {0}: ", item.Name);

                if (ViewExists(dbViews, item.Name))
                    RunCommand($"ALTER VIEW {item.Name} {item.Options} as {item.Script}");
                else
                    RunCommand($"CREATE VIEW {item.Name} {item.Options} as {item.Script}");
            }
        }

        private List<ViewData> SortList(List<ViewData> items)
        {
            List<ViewData> views = items.OrderBy(r => r.Name).ToList();

            foreach(ViewData toMove in views.ToList())
            {
                if(!string.IsNullOrEmpty(toMove.Required))
                {
                    //see if the specified view exists..
                    ViewData found = items.Where(r => r.Name.Equals(toMove.Required, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                    if (found != null)
                    {
                        int newIndex = views.IndexOf(found);
                        views.Remove(toMove); //remove from current spot in list
                        views.Insert(newIndex, toMove); //insert at new spot, this will move all the other items forward one spot
                    }
                }
            }

            return views;
        }

        private bool RemoveOptions(Assembly asmb, List<ViewData> items, List<DBObject> dbViews)
        {
            bool HadErrors = false;
            foreach (ViewData item in items)
            {
                if (ViewExists(dbViews, item.Name) && !string.IsNullOrEmpty(item.Options))
                {
                    Console.WriteLine("Removing Options On View {0}: ", item.Name);

                    try
                    {
                        RunCommand($"ALTER VIEW {item.Name} as {item.Script}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        HadErrors = true;
                    }
                }
            }

            return HadErrors;
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
