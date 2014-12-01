using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DataAccess.Migrations
{
    public class ExpressionParser : ExpressionVisitor
    {
        public ColumnExpression ParseExpression(Expression ex)
        {
            return Visit(ex) as ColumnExpression;
        }

        public override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return null;
            }

            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    return VisitUnary((UnaryExpression)exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.Power:
                    return VisitBinary((BinaryExpression)exp);
                case ExpressionType.TypeIs:
                    return VisitTypeIs((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)exp);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);
                case ExpressionType.MemberAccess:
                    return VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression)exp);
                case ExpressionType.New:
                    return this.VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression)exp);
                default:
                    return VisitUnknown(exp);
            }
        }

        protected virtual Expression VisitUnknown(Expression expression)
        {
            throw new Exception(string.Format("Unhandled expression type: '{0}'", expression.NodeType));
        }

        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return this.VisitMemberAssignment((MemberAssignment)binding);
                case MemberBindingType.MemberBinding:
                    return this.VisitMemberMemberBinding((MemberMemberBinding)binding);
                case MemberBindingType.ListBinding:
                    return this.VisitMemberListBinding((MemberListBinding)binding);
                default:
                    throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            ReadOnlyCollection<Expression> arguments = initializer.Arguments;
            if (arguments != initializer.Arguments)
            {
                return Expression.ElementInit(initializer.AddMethod, arguments);
            }
            return initializer;
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            Expression operand = this.Visit(u.Operand);
            return this.UpdateUnary(u, operand, u.Type, u.Method);
        }

        protected UnaryExpression UpdateUnary(UnaryExpression u, Expression operand, Type resultType, MethodInfo method)
        {
            if (u.Operand != operand || u.Type != resultType || u.Method != method)
            {
                return Expression.MakeUnary(u.NodeType, operand, resultType, method);
            }
            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            return null;
            //string toReturn = "";
            //Expression left = this.Visit(b.Left);
            //Expression right = this.Visit(b.Right);

            //ConstantExpression ceLeft = left as ConstantExpression;
            //ConstantExpression ceRight = right as ConstantExpression;
            //ColumnExpression sLeft = left as ColumnExpression;
            //ColumnExpression sRight = right as ColumnExpression;

            //if ((ceLeft == null || ceRight == null) && (sLeft == null || sRight == null))
            //    throw new Exception("The type of expression you provided is not supported");

            //object lValue = "";
            //object rValue = "";

            //if (sLeft == null) lValue = _dbStore.Connection.CommandGenerator.ResolveFieldName(ceLeft.Value.ToString(), t);
            //else lValue = sLeft.Value;

            //if (sRight == null)
            //{
            //    rValue = string.Concat("@", command.Parameters.Count);
            //    IDbDataParameter parm = command.CreateParameter();
            //    parm.ParameterName = rValue.ToString();
            //    parm.Value = ceRight.Value;
            //    command.Parameters.Add(parm);
            //}
            //else
            //    rValue = sRight.Value;


            //toReturn = string.Concat(lValue, TranslateType(b.NodeType), " ", rValue, " "); //, ceRight.Value
            //return new SqlExpression() { Value = toReturn };
        }

        protected virtual string TranslateType(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return "OR";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                default:
                    throw new Exception("Oops..");
            }
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            Expression expr = this.Visit(b.Expression);
            return this.UpdateTypeIs(b, expr, b.TypeOperand);
        }

        protected TypeBinaryExpression UpdateTypeIs(TypeBinaryExpression b, Expression expression, Type typeOperand)
        {
            if (expression != b.Expression || typeOperand != b.TypeOperand)
            {
                return Expression.TypeIs(expression, typeOperand);
            }
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            return c;
        }

        protected override Expression VisitConditional(ConditionalExpression c)
        {
            Expression test = this.Visit(c.Test);
            Expression ifTrue = this.Visit(c.IfTrue);
            Expression ifFalse = this.Visit(c.IfFalse);
            return this.UpdateConditional(c, test, ifTrue, ifFalse);
        }

        protected ConditionalExpression UpdateConditional(ConditionalExpression c, Expression test, Expression ifTrue, Expression ifFalse)
        {
            if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
            {
                return Expression.Condition(test, ifTrue, ifFalse);
            }
            return c;
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        protected virtual Expression VisitMemberAccess(MemberExpression m)
        {
            return new ColumnExpression()
                {
                    Column = m.Member.Name,
                    Object = m.Member.DeclaringType
                };



            ////if (m.Member.DeclaringType.IsAssignableFrom(t))
            ////{
            ////    return Expression.Constant(m.Member.Name);
            ////}

            //ConstantExpression ce = Visit(m.Expression) as ConstantExpression;
            //object target = null;
            //if (ce != null)
            //    target = ce.Value;

            //if (m.Member is PropertyInfo)
            //{
            //    return Expression.Constant(m.Member.ReflectedType.GetProperty(m.Member.Name).GetValue(target, null));
            //}
            //else
            //{
            //    return Expression.Constant((m.Member as FieldInfo).GetValue(target));
            //}
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            return ValidateMethodCall(m, m.Method);
        }

        protected Expression ValidateMethodCall(MethodCallExpression m, MethodInfo method)
        {
            ConstantExpression obj = this.Visit(m.Object) as ConstantExpression;

            if (obj == null)
            {
                object toInvoke = m.Object;
                object[] mArgs = getArgumentList(m.Arguments);
                if (toInvoke == null)
                {
                    object value = method.Invoke(null, mArgs);
                    return Expression.Constant(value);
                }
                else
                {
                    object value = method.Invoke(m.Object, mArgs);
                    return Expression.Constant(value);
                }
            }
            else
            {
                if (method.Name.Equals("Equals"))
                {
                    Expression arg = m.Arguments.First();
                    return Visit(Expression.MakeBinary(ExpressionType.Equal, Expression.Constant(obj.Value), arg));
                }
                else
                {
                    object value = method.Invoke(obj.Value, getArgumentList(m.Arguments));
                    return Expression.Constant(value);
                }
            }
        }

        private object[] getArgumentList(IEnumerable<Expression> args)
        {
            List<object> toReturn = new List<object>();
            foreach (Expression e in args)
            {
                ConstantExpression ce = Visit(e) as ConstantExpression;
                if (ce != null)
                    toReturn.Add(ce.Value);
            }
            return toReturn.ToArray();
        }

        protected virtual ReadOnlyCollection<Expression> VisitMemberAndExpressionList(ReadOnlyCollection<MemberInfo> members, ReadOnlyCollection<Expression> original)
        {
            if (original != null)
            {
                List<Expression> list = null;
                for (int i = 0, n = original.Count; i < n; i++)
                {
                    Expression p = this.VisitMemberAndExpression(members != null ? members[i] : null, original[i]);
                    if (list != null)
                    {
                        list.Add(p);
                    }
                    else if (p != original[i])
                    {
                        list = new List<Expression>(n);
                        for (int j = 0; j < i; j++)
                        {
                            list.Add(original[j]);
                        }
                        list.Add(p);
                    }
                }
                if (list != null)
                {
                    return list.AsReadOnly();
                }
            }
            return original;
        }

        protected virtual Expression VisitMemberAndExpression(MemberInfo member, Expression expression)
        {
            return this.Visit(expression);
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            Expression e = this.Visit(assignment.Expression);
            return this.UpdateMemberAssignment(assignment, assignment.Member, e);
        }

        protected MemberAssignment UpdateMemberAssignment(MemberAssignment assignment, MemberInfo member, Expression expression)
        {
            if (expression != assignment.Expression || member != assignment.Member)
            {
                return Expression.Bind(member, expression);
            }
            return assignment;
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);
            return this.UpdateMemberMemberBinding(binding, binding.Member, bindings);
        }

        protected MemberMemberBinding UpdateMemberMemberBinding(MemberMemberBinding binding, MemberInfo member, IEnumerable<MemberBinding> bindings)
        {
            if (bindings != binding.Bindings || member != binding.Member)
            {
                return Expression.MemberBind(member, bindings);
            }
            return binding;
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);
            return this.UpdateMemberListBinding(binding, binding.Member, initializers);
        }

        protected MemberListBinding UpdateMemberListBinding(MemberListBinding binding, MemberInfo member, IEnumerable<ElementInit> initializers)
        {
            if (initializers != binding.Initializers || member != binding.Member)
            {
                return Expression.ListBind(member, initializers);
            }
            return binding;
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                MemberBinding b = this.VisitBinding(original[i]);
                if (list != null)
                {
                    list.Add(b);
                }
                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(b);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                ElementInit init = this.VisitElementInitializer(original[i]);
                if (list != null)
                {
                    list.Add(init);
                }
                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(init);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            return this.Visit(lambda.Body);
        }

        protected LambdaExpression UpdateLambda(LambdaExpression lambda, Type delegateType, Expression body, IEnumerable<ParameterExpression> parameters)
        {
            if (body != lambda.Body || parameters != lambda.Parameters || delegateType != lambda.Type)
            {
                Delegate del = lambda.Compile();
                del.DynamicInvoke(parameters);

                return Expression.Lambda(delegateType, body, parameters);
            }
            return lambda;
        }

        protected virtual NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> args = this.VisitMemberAndExpressionList(nex.Members, nex.Arguments);
            return this.UpdateNew(nex, nex.Constructor, args, nex.Members);
        }

        protected NewExpression UpdateNew(NewExpression nex, ConstructorInfo constructor, IEnumerable<Expression> args, IEnumerable<MemberInfo> members)
        {
            if (args != nex.Arguments || constructor != nex.Constructor || members != nex.Members)
            {
                if (nex.Members != null)
                {
                    return Expression.New(constructor, args, members);
                }
                else
                {
                    return Expression.New(constructor, args);
                }
            }
            return nex;
        }

        protected override Expression VisitMemberInit(MemberInitExpression init)
        {
            NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
            return this.UpdateMemberInit(init, n, bindings);
        }

        protected MemberInitExpression UpdateMemberInit(MemberInitExpression init, NewExpression nex, IEnumerable<MemberBinding> bindings)
        {
            if (nex != init.NewExpression || bindings != init.Bindings)
            {
                return Expression.MemberInit(nex, bindings);
            }
            return init;
        }

        protected override Expression VisitListInit(ListInitExpression init)
        {
            NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);
            return this.UpdateListInit(init, n, initializers);
        }

        protected ListInitExpression UpdateListInit(ListInitExpression init, NewExpression nex, IEnumerable<ElementInit> initializers)
        {
            if (nex != init.NewExpression || initializers != init.Initializers)
            {
                return Expression.ListInit(nex, initializers);
            }
            return init;
        }

        protected override Expression VisitNewArray(NewArrayExpression na)
        {
            IEnumerable<Expression> exprs = na.Expressions;
            return this.UpdateNewArray(na, na.Type, exprs);
        }

        protected NewArrayExpression UpdateNewArray(NewArrayExpression na, Type arrayType, IEnumerable<Expression> expressions)
        {
            if (expressions != na.Expressions || na.Type != arrayType)
            {
                if (na.NodeType == ExpressionType.NewArrayInit)
                {
                    return Expression.NewArrayInit(arrayType.GetElementType(), expressions);
                }
                else
                {
                    return Expression.NewArrayBounds(arrayType.GetElementType(), expressions);
                }
            }
            return na;
        }

        protected override Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> args = iv.Arguments;
            Expression expr = this.Visit(iv.Expression);
            return this.UpdateInvocation(iv, expr, args);
        }

        protected InvocationExpression UpdateInvocation(InvocationExpression iv, Expression expression, IEnumerable<Expression> args)
        {
            if (args != iv.Arguments || expression != iv.Expression)
            {
                return Expression.Invoke(expression, args);
            }
            return iv;
        }
    }
}
