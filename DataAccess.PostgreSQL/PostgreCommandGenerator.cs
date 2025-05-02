using DataAccess.Core;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.PostgreSQL
{
    /// <summary>
    /// Command generator for postgre
    /// </summary>
    public class PostgreCommandGenerator : DatabaseCommandGenerator
    {
        public PostgreCommandGenerator(IDataConnection connection)
            : base(connection)
        {

        }

        public override async Task<List<DbCommand>> GetAddTableCommand(TypeParser tParser, DatabaseTypeInfo ti)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder pFields = new StringBuilder();
            StringBuilder contrain = new StringBuilder();
            NpgsqlCommand cmd = new NpgsqlCommand();

            sb.AppendFormat("CREATE TABLE {0} (", ResolveTableName(ti, false));
            for (int i = 0; i < ti.DataFields.Count; i++)
            {
                DataFieldInfo dfi = ti.DataFields[i];
                if (i > 0) sb.Append(',');

                string sType = await TranslateTypeToSql(tParser, dfi);

                if (dfi.PrimaryKey)
                {
                    if (pFields.Length > 0) pFields.Append(',');
                    pFields.Append(dfi.EscapedFieldName);

                    if (dfi.PropertyType == typeof(int) && ti.PrimaryKeys.Count == 1)
                        sb.AppendFormat("{0} serial NOT NULL ", dfi.EscapedFieldName);
                    else
                        sb.AppendFormat("{0} {1} NOT NULL ", dfi.EscapedFieldName, sType);
                }
                else
                    sb.AppendFormat("{0} {1} NULL ", dfi.EscapedFieldName, sType);

                if (dfi.PrimaryKeyType != null)
                {
                    if (contrain.Length > 0) contrain.Append(",");
                    DatabaseTypeInfo pkType = await tParser.GetTypeInfo(dfi.PrimaryKeyType);
                    contrain.AppendFormat(@"CONSTRAINT ""{0}"" FOREIGN KEY ({1}) REFERENCES {2} ({3}) MATCH SIMPLE ON UPDATE {4} ON DELETE {4}", Guid.NewGuid().ToString(), dfi.EscapedFieldName, ResolveTableName(pkType, false), pkType.PrimaryKeys.First().EscapedFieldName, TranslateFkeyType(dfi.ForeignKeyType));
                }
            }

            if (pFields.Length > 0)
                sb.AppendFormat(", CONSTRAINT \"{0}\" PRIMARY KEY ({1})", Guid.NewGuid().ToString(), pFields.ToString());

            if (contrain.Length > 0)
            {
                if (pFields.Length > 0) sb.Append(",");
                sb.Append(contrain.ToString());
            }

            sb.Append(");");
            cmd.CommandText = sb.ToString();
            return [cmd];
        }

        public override async Task<DbCommand> GetInsertCommand(TypeParser tParser, object item)
        {
            DbCommand cmd = await base.GetInsertCommand(tParser, item);
            cmd.CommandText = cmd.CommandText.Replace(";", " returning *;");
            return cmd;
        }

        public override DbCommand GetRemoveColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi)
        {
            NpgsqlCommand command = new NpgsqlCommand();
            command.CommandText = $"ALTER TABLE {ResolveTableName(type, false)} DROP COLUMN {dfi.FieldName};";
            return command;
        }

        public override async Task<List<DbCommand>> GetAddColumnCommnad(TypeParser tparser, DatabaseTypeInfo type, DataFieldInfo dfi)
        {
            if (dfi.PrimaryKey)
                throw new DataStoreException("Adding a primary key to an existing table is not supported");
            else
            {
                NpgsqlCommand scmd = new NpgsqlCommand();
                StringBuilder sb = new StringBuilder();

                string sType = await TranslateTypeToSql(tparser, dfi);
                sb.AppendFormat("ALTER TABLE {0} ADD {1} {2} NULL ", ResolveTableName(type, false), dfi.EscapedFieldName, sType);

                if (dfi.PrimaryKeyType != null)
                {
                    DatabaseTypeInfo pkType = await tparser.GetTypeInfo(dfi.PrimaryKeyType);
                    sb.Append(',');
                    sb.AppendFormat(@"ADD CONSTRAINT ""{0}"" FOREIGN KEY ({1}) REFERENCES {2} ({3}) MATCH SIMPLE ON UPDATE {4} ON DELETE {4}", Guid.NewGuid().ToString(), dfi.EscapedFieldName, ResolveTableName(pkType, false), pkType.PrimaryKeys.First().EscapedFieldName, TranslateFkeyType(dfi.ForeignKeyType));
                }

                sb.Append(';');
                scmd.CommandText = sb.ToString();
                return [scmd];
            }
        }

        public override List<DbCommand> GetModifyColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi, string targetFieldType)
        {
            targetFieldType = targetFieldType.Replace("varchar", "character varying");
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.CommandText = $"ALTER TABLE {ResolveTableName(type, false)} ALTER COLUMN {dfi.EscapedFieldName} TYPE {targetFieldType};";
            return [cmd];
        }

        public override async Task<string> ResolveTableName(TypeParser tParser, Type type)
        {
            return ResolveTableName(await tParser.GetTypeInfo(type), false);
        }

        public override string ResolveTableName(DatabaseTypeInfo ti)
        {
            return ResolveTableName(ti, false);
        }

        public override DbCommand GetAddSchemaCommand(DatabaseTypeInfo ti)
        {
            return null;
        }
    }
}
