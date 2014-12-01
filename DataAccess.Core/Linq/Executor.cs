using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess.Core.Linq.Common.Mapping;
using DataAccess.Core.Linq.Common;

namespace DataAccess.Core.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public class Executor : QueryExecutor
    {
        /// <summary>
        /// Gets the provider.
        /// </summary>
        public DBQueryProvider Provider { get; private set; }
        /// <summary>
        /// Gets a value indicating whether [buffer result rows].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [buffer result rows]; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool BufferResultRows { get { return false; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Executor"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public Executor(DBQueryProvider provider)
        {
            this.Provider = provider;
        }

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public override object Convert(object value, Type type)
        {
            if (value == null)
            {
                return TypeHelper.GetDefault(type);
            }
            type = TypeHelper.GetNonNullableType(type);
            Type vtype = value.GetType();
            if (type != vtype)
            {
                if (type.IsEnum)
                {
                    if (vtype == typeof(string))
                    {
                        return Enum.Parse(type, (string)value);
                    }
                    else
                    {
                        Type utype = Enum.GetUnderlyingType(type);
                        if (utype != vtype)
                        {
                            value = System.Convert.ChangeType(value, utype);
                        }
                        return Enum.ToObject(type, value);
                    }
                }
                return System.Convert.ChangeType(value, type);
            }
            return value;
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <param name="fnProjector">The fn projector.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns></returns>
        public override IEnumerable<T> Execute<T>(QueryCommand command, Func<FieldReader, T> fnProjector, MappingEntity entity, object[] paramValues)
        {
            IDbCommand cmd = this.GetCommand(command, paramValues);
            return Provider.Store.ExecuteCommandLoadList<T>(cmd);
        }

        /// <summary>
        /// Get an ADO command object initialized with the command-text and parameters
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns></returns>
        protected virtual IDbCommand GetCommand(QueryCommand query, object[] paramValues)
        {
            // create command object (and fill in parameters)
            IDbCommand cmd = this.Provider.Store.Connection.GetCommand();
            cmd.CommandText = query.CommandText;
            SetParameterValues(query, cmd, paramValues);
            return cmd;
        }

        /// <summary>
        /// Sets the parameter values.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="command">The command.</param>
        /// <param name="paramValues">The param values.</param>
        protected virtual void SetParameterValues(QueryCommand query, IDbCommand command, object[] paramValues)
        {
            if (query.Parameters.Count > 0 && command.Parameters.Count == 0)
            {
                for (int i = 0, n = query.Parameters.Count; i < n; i++)
                {
                    IDbDataParameter parm = Provider.Store.Connection.GetParameter(query.Parameters[i].Name, paramValues != null ? paramValues[i] : null);
                    command.Parameters.Add(parm);
                }
            }
            else if (paramValues != null)
            {
                for (int i = 0, n = command.Parameters.Count; i < n; i++)
                {
                    IDbDataParameter p = (IDbDataParameter)command.Parameters[i];
                    if (p.Direction == System.Data.ParameterDirection.Input
                     || p.Direction == System.Data.ParameterDirection.InputOutput)
                    {
                        p.Value = paramValues[i] ?? DBNull.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns></returns>
        public override int ExecuteCommand(QueryCommand query, object[] paramValues)
        {
            return Provider.Store.ExecuteCommand(GetCommand(query, paramValues));
        }
    }
}
