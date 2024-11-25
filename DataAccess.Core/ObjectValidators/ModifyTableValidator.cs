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
        public override async Task ValidateObject(TypeParser tparser, DatabaseTypeInfo ti)
        {
            DBObject table = GetObject(ti);

            if (table == null)
                await CreateNewTable(tparser, ti);
            else
                await ValidateExistingTable(tparser, ti, table);
        }
        
        public virtual async Task CreateNewTable(TypeParser tparser, DatabaseTypeInfo typeInfo)
        {
            await AddTable(tparser, typeInfo);
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

        protected virtual async Task ValidateExistingTable(TypeParser tparser, DatabaseTypeInfo typeInfo, DBObject t)
        {
            List<Column> valid = [];
            bool dirty = false;

            foreach (DataFieldInfo dfi in typeInfo.DataFields)
            {
                Column c = t.Columns.Where(R => R.Name.Equals(dfi.FieldName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (c == null)
                {
                    if (await AddColumn(tparser, dfi, typeInfo))
                    {
                        dirty = true;
                        valid.Add(new Column() { Name = dfi.FieldName });
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(c.DataType) && !dfi.PrimaryKey)
                    {
                        var sqlTYpe = await _dstore.Connection.CommandGenerator.TranslateTypeToSql(_dstore.TypeParser, dfi);

                        //check column type
                        if (!c.ResolvedColumnType.Equals(sqlTYpe, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (await ModifyColumn(dfi, typeInfo))
                            {
                                dirty = true;
                            }
                        }
                    }

                    valid.Add(c);
                }
            }

            if (await CheckForDeletedColumns(typeInfo, t, valid))
                dirty = true;

            if (dirty) Objects.Clear(); //this will cause the table info to reload
        }

        protected virtual async Task<bool> CheckForDeletedColumns(DatabaseTypeInfo typeInfo, DBObject t, List<Column> valid)
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
                            await RemoveColumn(new DataFieldInfo() { FieldName = c.Name }, typeInfo);
                    }
                }
                return dirty;
            }
            else
                return false;
        }

        public virtual async Task<DBObject> AddTable(TypeParser tparser, DatabaseTypeInfo ti)
        {
            if (ti.DataFields.Count > 0)
            {
                await CheckSchema(ti);

                foreach (DbCommand cmd in await _dstore.Connection.CommandGenerator.GetAddTableCommand(tparser, ti))
                    await _dstore.ExecuteCommands.ExecuteCommand(cmd, _dstore.Connection);

                FireCreated(ti);
                Objects.Clear();
                return GetObject(ti);
            }
            else
                return null;
        }

        public virtual async Task<bool> AddColumn(TypeParser tparser, DataFieldInfo field, DatabaseTypeInfo ti)
        {
            if (CanAddColumns)
            {
                bool result = false;
                if (ti.DataFields.Count > 0)
                {
                    foreach (DbCommand cmd in await _dstore.Connection.CommandGenerator.GetAddColumnCommnad(tparser, ti, field))
                        await _dstore.ExecuteCommands.ExecuteCommand(cmd, _dstore.Connection);

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

        public virtual async Task<bool> RemoveColumn(DataFieldInfo field, DatabaseTypeInfo ti)
        {
            if (CanRemoveColumns)
            {
                bool result = false;
                if (ti.DataFields.Count > 0)
                {
                    DbCommand cmd = _dstore.Connection.CommandGenerator.GetRemoveColumnCommand(ti, field);
                    await _dstore.ExecuteCommands.ExecuteCommand(cmd, _dstore.Connection);
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

        public async Task<bool> ModifyColumn(DataFieldInfo dfi, DatabaseTypeInfo typeInfo)
        {
            if (CanUpdateColumns)
            {
                var modifyCommands = await _dstore.Connection.CommandGenerator.GetModifyColumnCommand(_dstore.TypeParser, typeInfo, dfi);

                foreach (DbCommand cmd in modifyCommands)
                    await _dstore.ExecuteCommands.ExecuteCommand(cmd, _dstore.Connection);

                FireModified(typeInfo, "Added {0} to {1}", dfi.FieldName, typeInfo.TableName);
                return true;
            }
            else
            {
                return false;
            }
        }

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

        public virtual async Task CheckSchema(DatabaseTypeInfo ti)
        {
            if (!ti.UnEscapedSchema.Equals(_dstore.Connection.DefaultSchema, StringComparison.InvariantCultureIgnoreCase))
            {
                DbCommand cmd = _dstore.Connection.CommandGenerator.GetAddSchemaCommand(ti);

                if (cmd != null)
                    await _dstore.ExecuteCommands.ExecuteCommand(cmd, _dstore.Connection);
            }
        }
    }
}
