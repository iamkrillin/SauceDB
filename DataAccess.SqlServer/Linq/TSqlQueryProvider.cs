// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725
#pragma warning disable 1591

using DataAccess.Core.Interfaces;
using DataAccess.Core.Linq;
using DataAccess.Core.Linq.Common;

namespace DataAccess.SqlServer.Linq
{
    public class TSqlQueryProvider : DBQueryProvider
    {
        public TSqlQueryProvider(IDataStore dStore)
            : base(new TSqlLanguage(), dStore.GetQueryMapper(), new QueryPolicy(), dStore)
        {
        }
    }
}
