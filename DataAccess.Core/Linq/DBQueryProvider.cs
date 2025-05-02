// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using DataAccess.Core.Interfaces;
using DataAccess.Core.Linq.Common;
using DataAccess.Core.Linq.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
#pragma warning disable 1591

namespace DataAccess.Core.Linq
{
    public class DBQueryProvider : QueryProvider, ICreateExecutor
    {
        protected QueryLanguage language;
        protected SauceMapping mapping;
        protected QueryPolicy policy;

        public IDataStore Store { get; set; }

        public DBQueryProvider(QueryLanguage language, SauceMapping mapping, QueryPolicy policy, IDataStore dStore)
        {
            if (language == null)
                throw new InvalidOperationException("Language not specified");
            if (mapping == null)
                throw new InvalidOperationException("Mapping not specified");
            if (policy == null)
                throw new InvalidOperationException("Policy not specified");

            this.language = language;
            this.mapping = mapping;
            this.Store = dStore;
            this.policy = policy;
        }

        /// <summary>
        /// Convert the query expression into an execution plan
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual Expression GetExecutionPlan(Expression expression)
        {
            // strip off lambda for now
            LambdaExpression lambda = expression as LambdaExpression;
            if (lambda != null)
                expression = lambda.Body;

            QueryTranslator translator = this.CreateTranslator();

            // translate query into client & server parts
            Expression translation = translator.Translate(expression);

            var parameters = lambda != null ? lambda.Parameters : null;
            Expression provider = this.Find(expression, parameters);
            if (provider == null)
            {
                Expression rootQueryable = this.Find(expression, parameters, typeof(IQueryable));
                provider = Expression.Property(rootQueryable, typeof(IQueryable).GetProperty("Provider"));
            }

            return translator.Police.BuildExecutionPlan(translation, provider);
        }

        protected virtual QueryTranslator CreateTranslator()
        {
            return new QueryTranslator(this.language, this.mapping, this.policy);
        }

        private Expression Find(Expression expression, IList<ParameterExpression> parameters)
        {
            return TypedSubtreeFinder.Find(expression, this.GetType());
        }

        private Expression Find(Expression expression, IList<ParameterExpression> parameters, Type type)
        {
            if (parameters != null)
            {
                Expression found = parameters.FirstOrDefault(p => type.IsAssignableFrom(p.Type));
                if (found != null)
                    return found;
            }
            return TypedSubtreeFinder.Find(expression, type);
        }

        public override string GetQueryText(Expression expression)
        {
            Expression plan = this.GetExecutionPlan(expression);
            var commands = CommandGatherer.Gather(plan).Select(c => c.CommandText).ToArray();
            return string.Join("\n\n", commands);
        }

        public override object Execute(Expression expression)
        {
            LambdaExpression lambda = expression as LambdaExpression;
            Expression plan = this.GetExecutionPlan(expression);

            if (lambda != null)
            {
                // compile & return the execution plan so it can be used multiple times
                LambdaExpression fn = Expression.Lambda(lambda.Type, plan, lambda.Parameters);
                return fn.Compile();
            }
            else
            {
                // compile the execution plan and invoke it
                Expression<Func<object>> efn = Expression.Lambda<Func<object>>(Expression.Convert(plan, typeof(object)));
                Func<object> fn = efn.Compile();
                return fn();
            }
        }

        public QueryExecutor CreateExecutor()
        {
            return new Executor(this);
        }
    }
}
