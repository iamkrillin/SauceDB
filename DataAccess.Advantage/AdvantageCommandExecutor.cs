using Advantage.Data.Provider;
using DataAccess.Core.Execute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DataAccess.Advantage
{
    public class AdvantageCommandExecutor : ExecuteCommands
    {
        public override System.Data.IDbConnection OpenConnection(Core.Interfaces.IDataConnection connection)
        {
            IDbConnection conn = base.OpenConnection(connection);
            ((AdsConnection)conn).DateFormat = "YYYY-MM-DD";

            return conn;
        }
    }
}
