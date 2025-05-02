using DataAccess.Core;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.SQLite
{
    public class SqliteCommandGenerator : DatabaseCommandGenerator
    {
        public SqliteCommandGenerator(IDataConnection conn)
            : base(conn)
        {

        }

        public override async Task<DbCommand> GetInsertCommand(TypeParser tparser, object item)
        {
            List<DataFieldInfo> info = await tparser.GetPrimaryKeys(item.GetType());
            DbCommand cmd = await base.GetInsertCommand(tparser, item);
            if (info.Count() == 1)
                cmd.CommandText = cmd.CommandText.Replace(";", string.Format("; SELECT last_insert_rowid() as {0};", info.First().EscapedFieldName));

            return cmd;
        }

        public override async Task<List<DbCommand>> GetAddTableCommand(TypeParser tparser, DatabaseTypeInfo ti)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder pFields = new StringBuilder();
            SQLiteCommand cmd = new SQLiteCommand();
            StringBuilder fKey = new StringBuilder();

            sb.AppendFormat("CREATE TABLE {0} (", ResolveTableName(ti, false));
            for (int i = 0; i < ti.DataFields.Count; i++)
            {
                DataFieldInfo dfi = ti.DataFields[i];
                if (i > 0) sb.Append(",");

                string sType = await TranslateTypeToSql(tparser, dfi);

                if (dfi.PrimaryKey)
                {
                    sb.AppendFormat("{0} {1}", dfi.EscapedFieldName, sType);

                    if (dfi.PropertyType == typeof(int))
                    {
                        sb.Append(" PRIMARY KEY AUTOINCREMENT");
                    }
                    else
                    {
                        if (pFields.Length > 0) pFields.Append(",");
                        pFields.Append(dfi.EscapedFieldName);
                    }
                }
                else
                {
                    sb.AppendFormat("{0} {1}", dfi.EscapedFieldName, sType);

                    if (dfi.PrimaryKeyType == null)
                        sb.Append(" NULL");
                }

                if (dfi.PrimaryKeyType != null)
                {
                    if (fKey.Length > 0) fKey.Append(",");
                    fKey.Append(GetForeignKeySql(dfi, ti, await tparser.GetTypeInfo(dfi.PrimaryKeyType)));
                }
            }

            if (fKey.Length > 0)
                sb.AppendFormat(", {0}", fKey.ToString());

            if (pFields.Length > 0)
                sb.AppendFormat(", PRIMARY KEY({0})", pFields);

            sb.Append(");");
            cmd.CommandText = sb.ToString();
            return [cmd];
        }

        private string GetForeignKeySql(DataFieldInfo field, DatabaseTypeInfo targetTable, DatabaseTypeInfo pkeyTable)
        {
            return $" FOREIGN KEY({field.EscapedFieldName}) REFERENCES {ResolveTableName(pkeyTable, false)}({pkeyTable.PrimaryKeys.First().EscapedFieldName}) ON DELETE {TranslateFkeyType(field.ForeignKeyType)} ON UPDATE {TranslateFkeyType(field.ForeignKeyType)}";
        }

        public override DbCommand GetRemoveColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi)
        {
            throw new DataStoreException("This is not supported by SQLite");
        }

        public override async Task<List<DbCommand>> GetAddColumnCommnad(TypeParser tparser, DatabaseTypeInfo type, DataFieldInfo dfi)
        {
            if (dfi.PrimaryKey)
                throw new DataStoreException("Adding a primary key to an existing table is not supported");
            else
            {
                SQLiteCommand scmd = new SQLiteCommand();
                scmd.CommandText = $"ALTER TABLE {ResolveTableName(type, false)} ADD COLUMN {dfi.EscapedFieldName} {await TranslateTypeToSql(tparser, dfi)}";
                return [scmd];
            }
        }

        public override List<DbCommand> GetModifyColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi, string targetFieldType)
        {
            throw new Exception("this is not supported right now");
        }

        public override DbCommand GetAddSchemaCommand(DatabaseTypeInfo ti)
        {
            return null;
        }
    }
}
