using DataAccess.Core.Data;
using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheCodeHaven.SauceClassGeneration.DatabaseInterfaces;

namespace TheCodeHaven.SauceClassGeneration
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MyControl : UserControl
    {
        private ObservableCollection<Project> _projects;
        private static ProjectItems _projItems;
        private Solution2 _solution;
        private SolutionEvents _events;
        private DataAccess.Core.Interfaces.IDataStore _currentConnection;
        private List<DBObjectWithType> _objects;
        private object _lockMe = (object)1;

        public MyControl()
        {
            InitializeComponent();
            _events = SauceClassGenerationPackage.Application.Events.SolutionEvents;
            _events.Opened += () => SetSolution((Solution2)SauceClassGenerationPackage.Application.Solution);
            _events.ProjectAdded += (project) => AddProject(project);
            _events.ProjectRemoved += (project) => RemoveProject(project);

            ServerType.ItemsSource = DatabaseManager.Databases;
            _projects = new ObservableCollection<Project>();
            TargetProject.ItemsSource = _projects;

            //they might open the window after a solution is already open, so lets check for that
            SetSolution((Solution2)SauceClassGenerationPackage.Application.Solution);
        }

        private void ServerType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IDataBaseType item = ServerType.SelectedItem as IDataBaseType;
            if (item != null)
            {
                SettingsContainer.Content = item.ConnectionControl;
            }
        }

        private void FetchObjects(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(DoObjectFetch);
        }

        private void DoObjectFetch(object state)
        {
            SauceClassGenerationPackage.OutputWindow.OutputStringThreadSafe("Fetching Objects From database.." + Environment.NewLine);
            try
            {
                IDataBaseType item = null;

                this.InvokeOnMe(() =>
                {
                    this.IsEnabled = false;
                    item = ServerType.SelectedItem as IDataBaseType;
                    _currentConnection = item.ConnectionControl.GetConnection();
                    _currentConnection.SchemaValidator.CanAddColumns = false;
                    _currentConnection.SchemaValidator.CanRemoveColumns = false;
                    _currentConnection.SchemaValidator.CanUpdateColumns = false;
                });

                if (item != null)
                {
                    _objects = new List<DBObjectWithType>();

                    try
                    {
                        _currentConnection.SchemaValidator.TableValidator.GetObjects(true).ToList().ForEach(r => _objects.Add(new DBObjectWithType(r, ObjectType.Table)));
                    }
                    catch (NotImplementedException)
                    {
                        SauceClassGenerationPackage.OutputWindow.OutputStringThreadSafe("Selected data store does not support fetching tables.." + Environment.NewLine);
                    }

                    try
                    {
                        _currentConnection.SchemaValidator.ViewValidator.GetObjects(true).ToList().ForEach(r => _objects.Add(new DBObjectWithType(r, ObjectType.View)));
                    }
                    catch (NotImplementedException)
                    {
                        SauceClassGenerationPackage.OutputWindow.OutputStringThreadSafe("Selected data store does not support fetching views.." + Environment.NewLine);
                    }

                    this.InvokeOnMe(() =>
                        {
                            Tables.ItemsSource = _objects;
                        });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to fetch database objects!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.InvokeOnMe(() =>
                {
                    this.IsEnabled = true;
                });
            }
        }

        private void GenerateClassFiles(object sender, RoutedEventArgs e)
        {
            if (Tables.SelectedItems.Count > 0)
            {
                ThreadPool.QueueUserWorkItem(DoClassGeneration);
            }
            else
            {
                MessageBox.Show("Please select one or more objects to continue", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DoClassGeneration(object state)
        {
            List<DBObjectWithType> objs = null;
            this.InvokeOnMe(() =>
            {
                this.IsEnabled = false;
                objs = Tables.SelectedItems.Cast<DBObjectWithType>().ToList();
                SauceClassGenerationPackage.OutputWindow.Activate();
            });

            ProjectItem folder = GetFolder();
            if (folder != null)
            {
                try
                {
                    foreach (DBObjectWithType obj in objs)
                    {
                        SauceClassGenerationPackage.OutputWindow.OutputStringThreadSafe(string.Format("Working on Object: {0}{1}", obj.Object.Name, Environment.NewLine));
                        CodeClass newClass = GetCodeClass(obj.Object, folder);

                        if (obj.Type == ObjectType.Table)
                            newClass.AddAttribute("DataAccess.Core.Attributes.TableName", GetTableNameAttribute(obj.Object));
                        else if (obj.Type == ObjectType.View)
                            newClass.AddAttribute("DataAccess.Core.Attributes.ViewAttribute", GetTableNameAttribute(obj.Object));

                        bool failure = false;
                        List<Column> columns = obj.Object.Columns;
                        columns.Reverse();

                        foreach (Column v in columns)
                        {
                            if (!AddPropertyToClass(obj, newClass, v))
                                failure = false;
                        }

                        if (failure)
                        {
                            MessageBox.Show("Unable to translate one or more columns to an appropriate CLR type, see output window for more details", "Error Fetching Objects", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        newClass.ProjectItem.Save();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error during class generation!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            this.InvokeOnMe(() =>
            {
                this.IsEnabled = true;
            });
        }

        private bool AddPropertyToClass(DBObjectWithType obj, CodeClass newClass, Column v)
        {
            bool failure = false;
            SauceClassGenerationPackage.OutputWindow.OutputStringThreadSafe(string.Format("Processing Column: {0}.{1}..{2}", obj.Object.Name, v.Name, Environment.NewLine));
            TranslateResult result = TranslateToClr(v.DataType);
            string name = MakeLegalName(v.Name);

            CodeProperty2 newProp = (CodeProperty2)newClass.AddProperty(name, name, result.Type, Type.Missing, vsCMAccess.vsCMAccessPublic);

            MakeAutoProperty(newProp);
            AddDataFieldAttribute(newProp, obj.Object, v);

            if (!result.Success)
            {
                failure = true;
                SauceClassGenerationPackage.OutputWindow.OutputStringThreadSafe(string.Format("Unable to translate {0} to a CLR type for {1}.{2}", v.DataType, obj.Object.Name, v.Name));
                this.InvokeOnMe(() =>
                {
                    SauceClassGenerationPackage.OutputWindow.Activate();
                });
            }
            return failure;
        }

        private string MakeLegalName(string p)
        {
            return p.Replace(" ", "").Replace("-", "");
        }

        private CodeClass GetCodeClass(DBObject obj, ProjectItem folder)
        {
            string fileName;
            if(string.IsNullOrEmpty(obj.Schema)) fileName = string.Format(@"{0}.cs", obj.Name);
            else fileName = string.Format(@"{0}_{1}.cs", obj.Schema, obj.Name);
            
            string template = GetTemplate();
            foreach (ProjectItem v in folder.ProjectItems.Cast<ProjectItem>())
            {
                if (v.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
                {
                    v.Delete();
                    break;
                }
            }

            folder.ProjectItems.AddFromTemplate(template, fileName);
            ProjectItem pi = folder.ProjectItems.Item(fileName);
            pi.Open();

            while (!pi.IsOpen) System.Threading.Thread.Sleep(100);

            CodeClass newClass = pi.FileCodeModel.CodeElements.OfType<CodeNamespace>().First().Children.OfType<CodeClass>().First();
            newClass.Access = vsCMAccess.vsCMAccessPublic;
            return newClass;
        }

        private static void MakeAutoProperty(CodeProperty2 newProp)
        {
            EditPoint setter = newProp.Setter.StartPoint.CreateEditPoint();
            setter.Delete(newProp.Setter.EndPoint);
            setter.Insert("set;");

            EditPoint getter = newProp.Getter.StartPoint.CreateEditPoint();
            getter.Delete(newProp.Getter.EndPoint);
            getter.Insert("get;");
        }

        private void AddDataFieldAttribute(CodeProperty prop, DBObject obj, Column v)
        {
            string propType = v.IsPrimaryKey ? "DataAccess.Core.Attributes.Key" : "DataAccess.Core.Attributes.DataField";
            string propValue = string.Format("FieldName=\"{0}\"{1}{2}", v.Name, GetDefaultValueString(v), GetFieldTypeString(v));
            prop.AddAttribute(propType, propValue);
        }

        private string GetFieldTypeString(Column c)
        {
            if (c.DataType.ToLower().StartsWith("nvarchar"))
                c.DataType = c.DataType.Replace("nvarchar", "varchar");

            if (c.DataType == "varchar")
                return string.Concat(", ", string.Format("FieldTypeString=\"varchar({0})\", FieldType=FieldType.UserString", c.ColumnLength));

            return string.Concat(", ", "FieldTypeString=\"", c.DataType, "\", FieldType=FieldType.UserString");
        }

        private string GetDefaultValueString(Column c)
        {
            string toReturn = "";
            string value = c.DefaultValue;

            if (!string.IsNullOrEmpty(value))
            {
                if (value.Contains("\""))
                    value = value.Replace("\"", "\\\"");

                toReturn = string.Concat(", ", "DefaultValue=\"", value, "\"");
            }
            return toReturn;
        }

        public TranslateResult TranslateToClr(string type)
        {
            TranslateResult toReturn = new TranslateResult();

            type = type.ToLower();
            if (type.StartsWith("varchar") || 
                type.StartsWith("nvarchar") || 
                type.Equals("uniqueidentifier") || 
                type.StartsWith("char") || 
                type.Contains("text") ||
                type.Contains("memo"))
                toReturn.Type = "string";
            else if (type.StartsWith("varbinary") || type.Contains("blob"))
                toReturn.Type = "byte[]";
            else if (type.Equals("bit", StringComparison.InvariantCultureIgnoreCase) ||
                     type.StartsWith("logical"))
                toReturn.Type = "bool";
            else if (type.StartsWith("date") || type.StartsWith("smalldate"))
                toReturn.Type = "DateTime";
            else if (type.StartsWith("time"))
                toReturn.Type = "TimeSpan";
            else if (type.StartsWith("real") || type.StartsWith("numeric") || type.StartsWith("decimal") || type.StartsWith("float"))
                toReturn.Type = "double";
            else if (type.StartsWith("bigint"))
                toReturn.Type = "long";
            else if (type.Contains("int") || type.Equals("smallint"))
                toReturn.Type = "int";
            else //I give up, give an object
            {
                toReturn.Success = false;
                toReturn.Type = "object";
            }

            return toReturn;
        }

        public string GetTableNameAttribute(DBObject obj)
        {
            return string.Format(@"TableName = ""{0}"" {1}", obj.Name, GetSchemaString(obj.Schema));
        }

        private static string GetTypeName(DataAccess.Core.Data.DBObject t)
        {
            string toReturn = t.Name;
            if (!t.Schema.Equals("dbo"))
                toReturn = string.Concat(t.Schema, "_", t.Name);

            if (toReturn.EndsWith("s"))
                toReturn = toReturn.Remove(toReturn.Length - 1, 1);

            return toReturn;
        }

        public string GetSchemaString(string schema)
        {
            string toReturn = "";
            if (schema != null)
            {
                if (!schema.Equals("dbo"))
                    toReturn = string.Concat(", Schema=\"", schema, "\"");
            }
            return toReturn;
        }

        private string GetTemplate()
        {
            return _solution.GetProjectItemTemplate("Class", "CSharp");
        }

        private ProjectItem GetFolder()
        {
            ProjectItem toReturn = null;
            foreach (dynamic v in _projItems.ContainingProject.ProjectItems)
            {
                if (v.Name.Equals("SauceDB"))
                {
                    toReturn = v;
                    break;
                }
            }

            if (toReturn == null)
            {
                try
                {
                    toReturn = _projItems.ContainingProject.ProjectItems.AddFolder("SauceDB");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error While Adding Files!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return toReturn;
        }

        public void SetSolution(Solution2 solution)
        {
            ThreadPool.QueueUserWorkItem(DoSolutionLoad, solution);
        }

        private void DoSolutionLoad(object state)
        {
            Solution2 solution = state as Solution2;
            lock (_lockMe)
            {
                this.InvokeOnMe(() =>
                    {
                        _projects.Clear();
                    });
                
                if (solution != null)
                {
                    SauceClassGenerationPackage.OutputWindow.OutputStringThreadSafe("Solution Changed Reading Projects.." + Environment.NewLine);

                    _solution = solution;
                    foreach (Project d in solution.Projects)
                    {
                        AddProject(d);
                    }
                }
            }
        }

        private void TargetProject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GenerateButton.IsEnabled = TargetProject.SelectedItem != null;

            if (TargetProject.SelectedItem != null)
            {
                _projItems = ((dynamic)TargetProject.SelectedItem).ProjectItems;
            }
        }

        public void AddProject(Project Project)
        {
            this.InvokeOnMe(() =>
            {
                SauceClassGenerationPackage.OutputWindow.OutputStringThreadSafe(string.Format("Adding Project: {0}..{1}", Project.Name, Environment.NewLine));
                _projects.Add(Project);
            });
        }

        public void RemoveProject(Project Project)
        {
            if (TargetProject.SelectedItem == Project)
                TargetProject.SelectedItem = null;

            _projects.Remove(Project);
        }
    }
}