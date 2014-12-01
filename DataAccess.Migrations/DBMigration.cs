using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DataAccess.Migrations
{
    /// <summary>
    /// Defines DB Migration Operations
    /// </summary>
    public abstract class DBMigration
    {
        /// <summary>
        /// Use this when you want a table to be created
        /// </summary>
        /// <returns></returns>
        public virtual Type AddTable()
        {
           throw new NotImplementedException();
        }

        /// <summary>
        /// Use this when you want more than one table to be created
        /// </summary>
        /// <returns></returns>
        public virtual List<Type> AddTables()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use this when you want to add a column to a table
        /// </summary>
        /// <returns></returns>
        public virtual Expression AddColumn()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use this when you want to add multiple columns
        /// </summary>
        /// <returns></returns>
        public virtual List<Expression> AddColumns()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use this method when you want to remove a column
        /// </summary>
        /// <returns></returns>
        public virtual ColumnData RemoveColumn()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use this method when you want to remove more than one column
        /// </summary>
        /// <returns></returns>
        public virtual List<ColumnData> RemoveColumns()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use this method when you need to execute a db command to migrate data etc etc
        /// </summary>
        /// <returns></returns>
        public virtual IDbCommand RunCommand()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use this method when you need to execute more than one db command to migrate data etc etc
        /// </summary>
        /// <returns></returns>
        public virtual List<IDbCommand> RunCommands()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use this method to run commands via raw sql text
        /// </summary>
        /// <returns></returns>
        public virtual string RunTextCommand()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use this method to run commands via raw sql text
        /// </summary>
        /// <returns></returns>
        public virtual List<string> RunTextCommands()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use this when you want to update a column on a table
        /// </summary>
        /// <returns></returns>
        public virtual Expression UpdateColumn()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use this when you want to update multiple columns
        /// </summary>
        /// <returns></returns>
        public virtual List<Expression> UpdateColumns()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Will send you the instance of the datastore so you can work with it using sauce methods
        /// </summary>
        /// <param name="dstore"></param>
        public virtual void WorkWithDataStore(IDataStore dstore)
        {
            throw new NotImplementedException();
        }
    }
}
