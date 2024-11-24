using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using System.Data;
using DataAccess.Core.Data;
using DataAccess.Core.Events;
using System.Data.Common;
using System.Threading.Tasks;

namespace DataAccess.Core.ObjectValidators
{
    /// <summary>
    /// Validates tables on the datastore
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ModifyTableValidator"/> class.
    /// </remarks>
    /// <param name="dstore">The dstore.</param>
    public class ModifyTableValidator(IDataStore dstore) : ObjectValidator(dstore), IValidateTables
    {

        /// <summary>
        /// Validates an objects info against the datastore
        /// </summary>
        /// <param name="ti"></param>
        public override void ValidateObject(DatabaseTypeInfo ti)
        {
            DBObject table = GetObject(ti);

            if (table == null)
                CreateNewTable(ti);
            else
                ValidateExistingTable(ti, table);
        }

        /// <summary>
        /// Creates a new table.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        public virtual void CreateNewTable(DatabaseTypeInfo typeInfo)
        {
            AddTable(typeInfo);
            if (typeInfo.OnTableCreate != null)
            {
                foreach (AdditionalInitFunction v in typeInfo.OnTableCreate)
                {
                    if (!v.StaticMethod)
                        throw new DataStoreException("A static method is required for 'OnTableCreate'");

                    v.Invoke(this._dstore, null);
                }
            }
        }

        /// <summary>
        /// Validates an existing table.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="t">The t.</param>
        protected virtual void ValidateExistingTable(DatabaseTypeInfo typeInfo, DBObject t)
        {
            List<Column> valid = [];
            bool dirty = false;

            foreach (DataFieldInfo dfi in typeInfo.DataFields)
            {
                Column c = t.Columns.Where(R => R.Name.Equals(dfi.FieldName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (c == null)
                {
                    if (AddColumn(dfi, typeInfo))
                    {
                        dirty = true;
                        valid.Add(new Column() { Name = dfi.FieldName });
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(c.DataType) && !dfi.PrimaryKey)
                    {
                        //check column type
                        if (!c.ResolvedColumnType.Equals(_dstore.Connection.CommandGenerator.TranslateTypeToSql(dfi), StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (ModifyColumn(dfi, typeInfo))
                            {
                                dirty = true;
                            }
                        }
                    }

                    valid.Add(c);
                }
            }

            if (CheckForDeletedColumns(typeInfo, t, valid))
                dirty = true;

            if (dirty) Objects.Clear(); //this will cause the table info to reload
        }

        /// <summary>
        /// Checks for deleted columns.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="t">The t.</param>
        /// <param name="valid">The valid.</param>
        /// <returns></returns>
        protected virtual bool CheckForDeletedColumns(DatabaseTypeInfo typeInfo, DBObject t, List<Column> valid)
        {
            if (CanRemoveColumns)
            {
                bool dirty = false;
                if (valid.Count != t.Columns.Count)
                {
                    dirty = true;
                    foreach (Column c in t.Columns)
                    {
                        Column found = valid.Where(R => R.Name.Equals(c.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        if (found == null)
                            RemoveColumn(new DataFieldInfo() { FieldName = c.Name }, typeInfo);
                    }
                }
                return dirty;
            }
            else
                return false;
        }

        /// <summary>
        /// Returns the tables from the data store
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public virtual DBObject AddTable(DatabaseTypeInfo ti)
        {
            if (ti.DataFields.Count > 0)
            {
                CheckSchema(ti);

                foreach (DbCommand cmd in _dstore.Connection.CommandGenerator.GetAddTableCommand(ti))
                    _dstore.ExecuteCommands.ExecuteCommand(cmd, _dstore.Connection);

                FireCreated(ti);
                Objects.Clear();
                return GetObject(ti);
            }
            else
                return null;
        }

        /// <summary>
        /// Adds the column.
        /// </summary>
        /// <param name="field">The field to add</param>
        /// <param name="ti">The type its being added to</param>
        /// <returns></returns>
        public virtual bool AddColumn(DataFieldInfo field, DatabaseTypeInfo ti)
        {
            if (CanAddColumns)
            {
                bool result = false;
                if (ti.DataFields.Count > 0)
                {
                    foreach (DbCommand cmd in _dstore.Connection.CommandGenerator.GetAddColumnCommnad(ti, field))
                        _dstore.ExecuteCommands.ExecuteCommand(cmd, _dstore.Connection);

                    FireModified(ti, "Added {0} to {1}", field.FieldName, ti.TableName);
                    result = true;
                }
                return result;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removed a column from a data table
        /// </summary>
        /// <param name="field">The field to remove</param>
        /// <param name="ti">The type to remove it from</param>
        /// <returns></returns>
        public virtual bool RemoveColumn(DataFieldInfo field, DatabaseTypeInfo ti)
        {
            if (CanRemoveColumns)
            {
                bool result = false;
                if (ti.DataFields.Count > 0)
                {
                    DbCommand cmd = _dstore.Connection.CommandGenerator.GetRemoveColumnCommand(ti, field);
                    _dstore.ExecuteCommands.ExecuteCommand(cmd, _dstore.Connection);
                    FireModified(ti, "Removed Column {0} from {1}", field.FieldName, ti.TableName);
                    result = true;
                }
                return result;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Modifies a column from a data table
        /// </summary>
        /// <param name="dfi">The data field.</param>
        /// <param name="typeInfo">The type info.</param>
        public bool ModifyColumn(DataFieldInfo dfi, DatabaseTypeInfo typeInfo)
        {
            if (CanUpdateColumns)
            {
                foreach (DbCommand cmd in _dstore.Connection.CommandGenerator.GetModifyColumnCommand(typeInfo, dfi))
                    _dstore.ExecuteCommands.ExecuteCommand(cmd, _dstore.Connection);

                FireModified(typeInfo, "Added {0} to {1}", dfi.FieldName, typeInfo.TableName);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a list of objects from the datastore
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<DBObject> GetObjects()
        {
            if (Objects.Count < 1)
            {
                lock (Objects)
                {
                    Objects.AddRange(_dstore.Connection.GetSchemaTables(_dstore).ToBlockingEnumerable());
                }
            }

            return Objects;
        }

        /// <summary>
        /// executes a command to add a schema
        /// </summary>
        /// <param name="ti">The ti.</param>
        public virtual void CheckSchema(DatabaseTypeInfo ti)
        {
            if (!ti.UnEscapedSchema.Equals(_dstore.Connection.DefaultSchema, StringComparison.InvariantCultureIgnoreCase))
            {
                DbCommand cmd = _dstore.Connection.CommandGenerator.GetAddSchemaCommand(ti);

                if(cmd != null)
                    _dstore.ExecuteCommands.ExecuteCommand(cmd, _dstore.Connection);
            }
        }
    }
}
