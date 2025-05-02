using DataAccess.Core.Events;
using System;
using System.Data.Common;
using System.Threading.Tasks;

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
        Task<IQueryData> ExecuteCommandQuery(DbCommand command, IDataConnection connection);

        /// <summary>
        /// This method will take a connection that is already open and run a query on it
        /// </summary>
        /// <param name="command"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        Task<IQueryData> ExecuteCommandQueryAction(DbCommand command, IDataConnection connection, DbConnection r);

        /// <summary>
        /// Executes a command on the data store
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="connection">The connection to use</param>
        Task<int> ExecuteCommand(DbCommand command, IDataConnection connection);

        void InitCommand(DbCommand command, DbConnection conn);
    }
}
