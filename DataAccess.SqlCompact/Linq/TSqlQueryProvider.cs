// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Data;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Linq;
using DataAccess.Core.Linq.Mapping;
using DataAccess.Core.Linq.Common;

namespace DataAccess.SqlCompact.Linq
{
    public class TSqlQueryProvider : DBQueryProvider
    {
        public TSqlQueryProvider(IDataStore dStore)
            : base(new TSqlLanguage(), dStore.GetQueryMapper(), new QueryPolicy(), dStore)
        {
        }
    }
}
