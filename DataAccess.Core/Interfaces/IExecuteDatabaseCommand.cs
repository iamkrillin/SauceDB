using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess.Core.Data;
using DataAccess.Core.Events;
using DataAccess.Core.Execute;

namespace DataAccess.Core.Interfaces
{
    /// <summary>
    /// Represents a class for executing data store commands
    /// </summary>
    public interface IExecuteDatabaseCommand
    {
        /// <summary>
        /// This event will fire just before a command is executed
        /// </summary>
        event EventHandler<CommandExecutingEventArgs> CommandExecuting;

        /// <summary>
        /// This event will fire just after a command is executed
        /// </summary>
        event EventHandler<CommandExecutingEventArgs> CommandExecuted;

        /// <summary>
        /// This method will take a command and a connection, open the connection and execute the command
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="connection">The connection to use</param>
        /// <returns></returns>
        IQueryData ExecuteCommandQuery(IDbCommand command, IDataConnection connection);

        /// <summary>
        /// This method will take a connection that is already open and run a query on it
        /// </summary>
        /// <param name="command"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        IQueryData ExecuteCommandQueryAction(IDbCommand command, IDataConnection connection, IDbConnection r);

        /// <summary>
        /// Executes a command on the data store
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="connection">The connection to use</param>
        int ExecuteCommand(IDbCommand command, IDataConnection connection);

        void InitCommand(IDbCommand command, IDbConnection conn);
    }
}
