// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
#pragma warning disable 1591

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using DataAccess.Core.Linq.Common;
using DataAccess.Core.Linq.Common.Mapping;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICreateExecutor
    {
        QueryExecutor CreateExecutor();
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class QueryExecutor
    {
        // called from compiled execution plan
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public abstract object Convert(object value, Type type);
        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <param name="fnProjector">The fn projector.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns></returns>
        public abstract IEnumerable<T> Execute<T>(QueryCommand command, Func<FieldReader, T> fnProjector, MappingEntity entity, object[] paramValues);
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns></returns>
        public abstract int ExecuteCommand(QueryCommand query, object[] paramValues);
    }
}