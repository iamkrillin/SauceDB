using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Events;

namespace DataAccess.Core.Execute
{
    /// <summary>
    /// Executes command within a transaction
    /// </summary>
    public class TransactionCommandExecutor : IExecuteDatabaseCommand
    {
        private TransactionInfo _info;
        public event EventHandler<Events.CommandExecutingEventArgs> CommandExecuting;
        public event EventHandler<Events.CommandExecutingEventArgs> CommandExecuted;

        private IExecuteDatabaseCommand _base;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionCommandExecutor" /> class.
        /// </summary>
        /// <param name="info">The transacation info.</param>
        /// <param name="storeExecutor">The executor used by the data store</param>
        public TransactionCommandExecutor(TransactionInfo info, IExecuteDatabaseCommand storeExecutor)
            : base()
        {
            _info = info;
            _base = storeExecutor;
        }

        protected void FireExecutingEvent(IDbCommand command, IDataConnection connection, IDbConnection conn)
        {
            CommandExecuting?.Invoke(this, new CommandExecutingEventArgs(command, connection, conn));
        }

        protected void FireExecutedEvent(IDbCommand command, IDataConnection connection, IDbConnection conn)
        {
            CommandExecuted?.Invoke(this, new CommandExecutingEventArgs(command, connection, conn));
        }

        protected T ExecuteCommand<T>(IDbCommand command, IDataConnection connection, Func<IDbConnection, T> action)
        {
            command.Connection = _info.Connection;
            command.Transaction = _info.Transaction;

            T toReturn = default(T);
            foreach (IDbDataParameter parm in command.Parameters)
                connection.DatastoreConverter.CoerceValue(parm);

            InitCommand(command, _info.Connection);

            try
            {
                FireExecutingEvent(command, connection, _info.Connection);
                toReturn = action(_info.Connection);
                FireExecutedEvent(command, connection, _info.Connection);
            }
            catch (Exception e)
            {
                throw new QueryException(e, command);
            }
            finally
            {
                command.Dispose();
            }

            return toReturn;
        }

        public virtual IQueryData ExecuteCommandQueryAction(IDbCommand command, IDataConnection connection, IDbConnection r)
        {
            return _base.ExecuteCommandQueryAction(command, connection, r);
        }

        public virtual IQueryData ExecuteCommandQuery(IDbCommand command, IDataConnection connection)
        {
            return ExecuteCommand<IQueryData>(command, connection, r =>
            {
                return _base.ExecuteCommandQueryAction(command, connection, r);
            });
        }

        public virtual int ExecuteCommand(IDbCommand command, IDataConnection connection)
        {
            return ExecuteCommand<int>(command, connection, r =>
            {

                return command.ExecuteNonQuery();
            });
        }

        public virtual void InitCommand(IDbCommand command, IDbConnection conn)
        {
            _base.InitCommand(command, conn);
        }
    }
}
