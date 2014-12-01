// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using DataAccess.Core.Linq.Common;
using DataAccess.Core.Linq.Common.Language;
using DataAccess.Core.Linq.Common.Mapping;
using DataAccess.Core.Linq.Mapping;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// Defines query execution and materialization policies. 
    /// </summary>
    public class QueryTranslator
    {
        /// <summary>
        /// Gets the linguist.
        /// </summary>
        public QueryLinguist Linguist { get; private set; }

        /// <summary>
        /// Gets the mapper.
        /// </summary>
        public QueryMapper Mapper { get; private set; }

        /// <summary>
        /// Gets the police.
        /// </summary>
        public QueryPolice Police { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryTranslator"/> class.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="policy">The policy.</param>
        public QueryTranslator(QueryLanguage language, SauceMapping mapping, QueryPolicy policy)
        {
            this.Linguist = language.CreateLinguist(this);
            this.Mapper = mapping.CreateMapper(this);
            this.Police = policy.CreatePolice(this);
        }

        /// <summary>
        /// Translates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public virtual Expression Translate(Expression expression)
        {
            // pre-evaluate local sub-trees
            expression = PartialEvaluator.Eval(expression, this.Mapper.Mapping.CanBeEvaluatedLocally);

            // apply mapping (binds LINQ operators too)
            expression = this.Mapper.Translate(expression);

            // any policy specific translations or validations
            expression = this.Police.Translate(expression);

            // any language specific translations or validations
            expression = this.Linguist.Translate(expression);

            return expression;
        }
    }
}