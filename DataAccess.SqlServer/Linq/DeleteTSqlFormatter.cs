using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq;
using DataAccess.Core.Linq.Common;
using System.Linq.Expressions;
using DataAccess.Core.Linq.Common.Expressions;
using DataAccess.Core.Linq.Mapping;
using DataAccess.Core;
#pragma warning disable 1591

namespace DataAccess.SqlServer.Linq
{
    public class DeleteTSqlFormatter : TSqlFormatter, IDeleteFormatter
    {
        protected Dictionary<string, Object> _parameters = new Dictionary<String, Object>();
        private SauceMapping _mapper;

        public DeleteTSqlFormatter(IDataStore dstore)
            : base(new TSqlLanguage())
        {
            _mapper = dstore.GetQueryMapper();
        }

        public string FormatDelete(Expression expression, out Dictionary<string, Object> parameters)
        {
            if (!(expression is ConstantExpression))  //if expression is just constant then I skip it
            {
                if (expression is LambdaExpression)
                    this.Visit(((LambdaExpression)expression).Body);

                else
                    this.Visit(expression);
            }

            parameters = this._parameters;
            return this.ToString();
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            //parametrize constants
            string par = String.Format("{0}", _parameters.Count + 1);
            if (_parameters == null) _parameters = new Dictionary<string, object>();
            _parameters.Add(par, c.Value);
            var result = new NamedValueExpression(par, c);
            return base.VisitNamedValue(result);
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            this.WriteColumnName(_mapper.GetColumnName(m.Member));
            return m;
        }
    }
}
