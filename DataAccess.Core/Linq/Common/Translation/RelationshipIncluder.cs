﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

#pragma warning disable 1591

using DataAccess.Core.Linq.Common.Expressions;
using DataAccess.Core.Linq.Common.Mapping;
using System.Linq.Expressions;
using System.Reflection;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// Adds relationship to query results depending on policy
    /// </summary>
    public class RelationshipIncluder : DbExpressionVisitor
    {
        QueryMapper mapper;
        QueryPolicy policy;
        ScopedDictionary<MemberInfo, bool> includeScope = new ScopedDictionary<MemberInfo, bool>(null);

        private RelationshipIncluder(QueryMapper mapper)
        {
            this.mapper = mapper;
            this.policy = mapper.Translator.Police.Policy;
        }

        public static Expression Include(QueryMapper mapper, Expression expression)
        {
            return new RelationshipIncluder(mapper).Visit(expression);
        }

        protected override Expression VisitProjection(ProjectionExpression proj)
        {
            Expression projector = this.Visit(proj.Projector);
            return this.UpdateProjection(proj, proj.Select, projector, proj.Aggregator);
        }

        protected override Expression VisitEntity(EntityExpression entity)
        {
            var save = this.includeScope;
            this.includeScope = new ScopedDictionary<MemberInfo, bool>(this.includeScope);
            try
            {
                if (this.mapper.HasIncludedMembers(entity))
                {
                    entity = this.mapper.IncludeMembers(
                            entity,
                            m =>
                            {
                                if (this.includeScope.ContainsKey(m))
                                {
                                    return false;
                                }
                                if (this.policy.IsIncluded(m))
                                {
                                    this.includeScope.Add(m, true);
                                    return true;
                                }
                                return false;
                            });
                }
                return base.VisitEntity(entity);
            }
            finally
            {
                this.includeScope = save;
            }
        }
    }
}
