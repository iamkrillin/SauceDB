// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using DataAccess.Core;
using DataAccess.Core.Linq.Common;
using DataAccess.Core.Linq;
using DataAccess.Core.Linq.Common.Expressions;
using DataAccess.Core.Linq.Common.Language;
#pragma warning disable 1591

namespace DataAccess.SqlCompact.Linq
{
    /// <summary>
    /// TSQL specific QueryLanguage
    /// </summary>
    public class TSqlLanguage : QueryLanguage
    {
        public override bool AllowsMultipleCommands { get { return true; } }
        public override bool AllowSubqueryInSelectWithoutFrom { get { return true; } }
        public override bool AllowDistinctInAggregates { get { return true; } }

        public TSqlLanguage()
        {

        }

        public override Expression GetGeneratedIdExpression(MemberInfo member)
        {
            return new FunctionExpression(TypeHelper.GetMemberType(member), "SCOPE_IDENTITY()", null);
        }

        public override QueryLinguist CreateLinguist(QueryTranslator translator)
        {
            return new TSqlLinguist(this, translator);
        }
    }
}
