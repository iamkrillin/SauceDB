// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DataAccess.Core.Linq.Common.Language
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class QueryTypeSystem
    {
        /// <summary>
        /// Parses the specified type declaration.
        /// </summary>
        /// <param name="typeDeclaration">The type declaration.</param>
        /// <returns></returns>
        public abstract QueryType Parse(string typeDeclaration);

        /// <summary>
        /// Gets the type of the column.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        //public abstract QueryType GetColumnType(Type type);

        /// <summary>
        /// Gets the variable declaration.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="suppressSize">if set to <c>true</c> [suppress size].</param>
        /// <returns></returns>
        //public abstract string GetVariableDeclaration(QueryType type, bool suppressSize);
    }
}