using DataAccess.Core;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DataAccess.Advantage
{
    public class AdvantageDatabaseCommandGenerator : DatabaseCommandGenerator
    {        
        public AdvantageDatabaseCommandGenerator(IDataConnection conn)
            : base(conn)
        {

        }

        public override IEnumerable<System.Data.IDbCommand> GetAddColumnCommnad(DataAccess.Core.Data.DatabaseTypeInfo type, DataAccess.Core.Data.DataFieldInfo dfi)
        {
            throw new NotImplementedException();
        }

        public override System.Data.IDbCommand GetAddSchemaCommand(DataAccess.Core.Data.DatabaseTypeInfo ti)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<System.Data.IDbCommand> GetAddTableCommand(DataAccess.Core.Data.DatabaseTypeInfo ti)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IDbCommand> GetModifyColumnCommand(DataAccess.Core.Data.DatabaseTypeInfo type, DataAccess.Core.Data.DataFieldInfo dfi, string targetFieldType)
        {
            throw new NotImplementedException();
        }

        public override System.Data.IDbCommand GetRemoveColumnCommand(DataAccess.Core.Data.DatabaseTypeInfo type, DataAccess.Core.Data.DataFieldInfo dfi)
        {
            throw new NotImplementedException();
        }

        protected override string AppendParameters(List<ParameterData> parms, IDbCommand command)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < parms.Count; i++)
            {
                if (i > 0) sb.Append(",");
                if (parms[i].Parameter.Value == null)
                {
                    sb.Append("NULL");
                }
                else
                {
                    sb.Append(string.Concat(":", parms[i].Parameter.ParameterName));
                    command.Parameters.Add(parms[i].Parameter);
                }
            }
            return sb.ToString();
        }

        protected override string GetParameterName(System.Data.IDbCommand cmd)
        {
            return string.Concat(":", cmd.Parameters.Count + 1);
        }
    }
}
