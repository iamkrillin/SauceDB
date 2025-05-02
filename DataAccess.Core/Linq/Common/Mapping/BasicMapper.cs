using DataAccess.Core.Linq.Common.Expressions;
using DataAccess.Core.Linq.Common.Translation;
using DataAccess.Core.Linq.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
#pragma warning disable 1591

namespace DataAccess.Core.Linq.Common.Mapping
{
    public class BasicMapper : QueryMapper
    {
        private SauceMapping mapping;
        private QueryTranslator translator;

        public override SauceMapping Mapping { get { return this.mapping; } }
        public override QueryTranslator Translator { get { return this.translator; } }

        public BasicMapper(SauceMapping mapping, QueryTranslator translator)
        {
            this.mapping = mapping;
            this.translator = translator;
        }

        public override ProjectionExpression GetQueryExpression(MappingEntity entity)
        {
            var tableAlias = new TableAlias();
            var selectAlias = new TableAlias();
            var tableName = this.mapping.GetTableName(entity);
            tableName.Wait();

            var table = new TableExpression(tableAlias, entity, tableName.Result);
            Expression projector = this.GetEntityExpression(table, entity);
            var pc = ColumnProjector.ProjectColumns(this.translator.Linguist.Language, projector, null, selectAlias, tableAlias);
            var proj = new ProjectionExpression(new SelectExpression(selectAlias, pc.Columns, table, null), pc.Projector);

            return (ProjectionExpression)this.Translator.Police.ApplyPolicy(proj, entity.ElementType);
        }

        public override EntityExpression GetEntityExpression(Expression root, MappingEntity entity)
        {
            // must be some complex type constructed from multiple columns
            var assignments = new List<EntityAssignment>();
            foreach (MemberInfo mi in this.mapping.GetMappedMembers(entity))
            {
                Expression me = this.GetMemberExpression(root, entity, mi);
                if (me != null)
                    assignments.Add(new EntityAssignment(mi, me));
            }

            return new EntityExpression(entity, BuildEntityExpression(entity, assignments));
        }

        protected virtual Expression BuildEntityExpression(MappingEntity entity, IList<EntityAssignment> assignments)
        {
            NewExpression newExpression;

            // handle cases where members are not directly assignable
            EntityAssignment[] readonlyMembers = assignments.Where(b => TypeHelper.IsReadOnly(b.Member)).ToArray();
            ConstructorInfo[] cons = entity.EntityType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            bool hasNoArgConstructor = cons.Any(c => c.GetParameters().Length == 0);

            if (readonlyMembers.Length > 0 || !hasNoArgConstructor)
            {
                // find all the constructors that bind all the read-only members
                var consThatApply = cons.Select(c => this.BindConstructor(c, readonlyMembers)).Where(cbr => cbr != null && cbr.Remaining.Count == 0).ToList();
                if (consThatApply.Count == 0)
                {
                    throw new InvalidOperationException(string.Format("Cannot construct type '{0}' with all mapped includedMembers.", entity.ElementType));
                }
                // just use the first one... (Note: need better algorithm. :-)
                if (readonlyMembers.Length == assignments.Count)
                {
                    return consThatApply[0].Expression;
                }
                var r = this.BindConstructor(consThatApply[0].Expression.Constructor, assignments);

                newExpression = r.Expression;
                assignments = r.Remaining;
            }
            else
            {
                newExpression = Expression.New(entity.EntityType);
            }

            Expression result;
            if (assignments.Count > 0)
            {
                if (entity.ElementType.IsInterface)
                {
                    assignments = this.MapAssignments(assignments, entity.EntityType).ToList();
                }
                result = Expression.MemberInit(newExpression, (MemberBinding[])assignments.Select(a => Expression.Bind(a.Member, a.Expression)).ToArray());
            }
            else
            {
                result = newExpression;
            }

            if (entity.ElementType != entity.EntityType)
            {
                result = Expression.Convert(result, entity.ElementType);
            }

            return result;
        }

        private IEnumerable<EntityAssignment> MapAssignments(IEnumerable<EntityAssignment> assignments, Type entityType)
        {
            foreach (var assign in assignments)
            {
                MemberInfo[] members = entityType.GetMember(assign.Member.Name, BindingFlags.Instance | BindingFlags.Public);
                if (members != null && members.Length > 0)
                {
                    yield return new EntityAssignment(members[0], assign.Expression);
                }
                else
                {
                    yield return assign;
                }
            }
        }

        protected virtual ConstructorBindResult BindConstructor(ConstructorInfo cons, IList<EntityAssignment> assignments)
        {
            var ps = cons.GetParameters();
            var args = new Expression[ps.Length];
            var mis = new MemberInfo[ps.Length];
            HashSet<EntityAssignment> members = new HashSet<EntityAssignment>(assignments);
            HashSet<EntityAssignment> used = new HashSet<EntityAssignment>();

            for (int i = 0, n = ps.Length; i < n; i++)
            {
                ParameterInfo p = ps[i];
                var assignment = members.FirstOrDefault(a => p.Name == a.Member.Name && p.ParameterType.IsAssignableFrom(a.Expression.Type));
                if (assignment == null)
                    assignment = members.FirstOrDefault(a => string.Compare(p.Name, a.Member.Name, true) == 0 && p.ParameterType.IsAssignableFrom(a.Expression.Type));

                if (assignment != null)
                {
                    args[i] = assignment.Expression;
                    if (mis != null)
                        mis[i] = assignment.Member;
                    used.Add(assignment);
                }
                else
                {
                    MemberInfo[] mems = cons.DeclaringType.GetMember(p.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                    if (mems != null && mems.Length > 0)
                    {
                        args[i] = Expression.Constant(TypeHelper.GetDefault(p.ParameterType), p.ParameterType);
                        mis[i] = mems[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            members.ExceptWith(used);
            return new ConstructorBindResult(Expression.New(cons, args, mis), members);
        }

        public override bool HasIncludedMembers(EntityExpression entity)
        {
            var policy = this.translator.Police.Policy;
            foreach (var mi in this.mapping.GetMappedMembers(entity.Entity))
            {
                if (policy.IsIncluded(mi))
                    return true;
            }
            return false;
        }

        public override EntityExpression IncludeMembers(EntityExpression entity, Func<MemberInfo, bool> fnIsIncluded)
        {
            var assignments = this.GetAssignments(entity.Expression).ToDictionary(ma => ma.Member.Name);
            bool anyAdded = false;
            foreach (var mi in this.mapping.GetMappedMembers(entity.Entity))
            {
                EntityAssignment ea;
                bool okayToInclude = !assignments.TryGetValue(mi.Name, out ea);
                if (okayToInclude && fnIsIncluded(mi))
                {
                    ea = new EntityAssignment(mi, this.GetMemberExpression(entity.Expression, entity.Entity, mi));
                    assignments[mi.Name] = ea;
                    anyAdded = true;
                }
            }
            if (anyAdded)
            {
                return new EntityExpression(entity.Entity, this.BuildEntityExpression(entity.Entity, assignments.Values.ToList()));
            }
            return entity;
        }

        private IEnumerable<EntityAssignment> GetAssignments(Expression newOrMemberInit)
        {
            var assignments = new List<EntityAssignment>();
            var minit = newOrMemberInit as MemberInitExpression;
            if (minit != null)
            {
                assignments.AddRange(minit.Bindings.OfType<MemberAssignment>().Select(a => new EntityAssignment(a.Member, a.Expression)));
                newOrMemberInit = minit.NewExpression;
            }
            var nex = newOrMemberInit as NewExpression;
            if (nex != null && nex.Members != null)
            {
                assignments.AddRange(Enumerable.Range(0, nex.Arguments.Count).Where(i => nex.Members[i] != null).Select(i => new EntityAssignment(nex.Members[i], nex.Arguments[i])));
            }
            return assignments;
        }

        public override Expression GetMemberExpression(Expression root, MappingEntity entity, MemberInfo member)
        {
            AliasedExpression aliasedRoot = root as AliasedExpression;
            var isCol = this.mapping.IsColumn(entity, member);
            isCol.Wait();

            if (aliasedRoot != null && isCol.Result)
            {
                var colName = this.mapping.GetColumnName(entity, member);
                colName.Wait();

                return new ColumnExpression(TypeHelper.GetMemberType(member), aliasedRoot.Alias, colName.Result);
            }

            return QueryBinder.BindMember(root, member);
        }

        private IEnumerable<ColumnAssignment> GetColumnAssignments(Expression table, Expression instance, MappingEntity entity, Func<MappingEntity, MemberInfo, bool> fnIncludeColumn)
        {
            foreach (var m in this.mapping.GetMappedMembers(entity))
            {
                var isCol = this.mapping.IsColumn(entity, m);
                isCol.Wait();

                if (isCol.Result && fnIncludeColumn(entity, m))
                {
                    yield return new ColumnAssignment((ColumnExpression)this.GetMemberExpression(table, entity, m), Expression.MakeMemberAccess(instance, m));
                }
            }
        }
    }
}
