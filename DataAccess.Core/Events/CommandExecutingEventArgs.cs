using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess.Core.Interfaces;

namespace DataAccess.Core.Events
{
    /// <summary>
    /// Fired when a command is about to execute
    /// </summary>
    public class CommandExecutingEventArgs : EventArgs
    {
        /// <summary>
        /// The command
        /// </summary>
        public IDbCommand Command { get; set; }

        /// <summary>
        /// The connection its going to run on
        /// </summary>
        public IDataConnection Connection { get; set; }

        public IDbConnection RawConnection { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutingEventArgs" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="connection">The connection.</param>
        public CommandExecutingEventArgs(IDbCommand command, IDataConnection connection)
        {
            this.Command = command;
            this.Connection = connection;
        }

        public CommandExecutingEventArgs(IDbCommand command, IDataConnection connection, IDbConnection conn)
        {
            this.Command = command;
            this.Connection = connection;
            this.RawConnection = conn;
        }

    }
}
