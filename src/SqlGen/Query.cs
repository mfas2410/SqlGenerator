using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SqlGen
{
    public abstract class Query
    {
        internal abstract IEnumerable<string> Columns { get; }

        internal abstract string TableAlias { get; }

        internal abstract string TableName { get; }

        internal abstract string WhereExpression { get; }
    }

    public sealed class Query<T> : Query where T : class
    {
        private readonly PropertyInfo[] properties;
        private readonly QueryBuilder queryBuilder;
        private Expression<Func<T, object>> selectExpression;
        private string tableAlias;
        private string tableName;
        private string whereExpression;

        internal Query(QueryBuilder queryBuilder, Expression<Func<T>> alias)
        {
            this.queryBuilder = queryBuilder ?? throw new ArgumentNullException(nameof(queryBuilder));
            if (alias != null)
            {
                Match match = Regex.Match(alias.Body.ToString(), $@"^value\(.+\)\.(?'{nameof(tableAlias)}'.+)$", RegexOptions.Singleline);
                tableAlias = match.Groups[nameof(tableAlias)].Value;
            }

            Type type = typeof(T);
            tableName = type.Name;
            properties = type.GetProperties();
        }

        public override string ToString()
        {
            return queryBuilder.ToString();
        }

        public Query<T> Distinct()
        {
            queryBuilder.Distinct();
            return this;
        }

        public Query<TU> Join<TU>(Expression<Func<TU>> alias, Expression<Func<T, TU, bool>> expression, JoinType joinType = JoinType.Inner) where TU : class
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            Query<TU> query = new Query<TU>(queryBuilder, alias);
            Join join = new Join { Expression = expression.ToString(), JoinType = joinType, Table1 = this, Table2 = query };
            queryBuilder.joins.Add(join);

            return query;
        }

        public Query<TU> Join<TU>(Expression<Func<T, TU, bool>> expression, JoinType joinType = JoinType.Inner) where TU : class
        {
            return Join(null, expression, joinType);
        }

        public Query<T> OrderBy(Expression<Func<T, object>> expression)
        {
            queryBuilder.OrderBy(expression);
            return this;
        }

        public Query<T> OrderBy(Expression<Func<object>> expression)
        {
            queryBuilder.OrderBy(expression);
            return this;
        }

        public Query<T> Select(Expression<Func<T, object>> expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            var memberExpression = expression.Body as MemberExpression;
            var newExpression = expression.Body as NewExpression;
            var unaryExpression = expression.Body as UnaryExpression;
            if (memberExpression == null && newExpression == null && (unaryExpression == null || unaryExpression.Operand.NodeType != ExpressionType.MemberAccess))
            {
                throw new ArgumentException("Unsupported expression type", nameof(expression));
            }

            selectExpression = expression;
            return this;
        }

        public Query<T> Skip(int skip)
        {
            queryBuilder.Skip(skip);
            return this;
        }

        public Query<T> Take(int take)
        {
            queryBuilder.Take(take);
            return this;
        }

        public Query<T> Where(Expression<Func<T, bool>> expression)
        {
            // TODO: missing operators: BETWEEN, LIKE, IN
            whereExpression = expression?.ToString() ?? throw new ArgumentNullException(nameof(expression));
            return this;
        }

        internal override IEnumerable<string> Columns
        {
            get
            {
                if (selectExpression == null)
                {
                    return properties.Select(property => property.Name);
                }

                var memberExpression = selectExpression.Body as MemberExpression;
                if (memberExpression != null)
                {
                    return new[] { memberExpression.Member.Name };
                }

                var newExpression = selectExpression.Body as NewExpression;
                if (newExpression != null)
                {
                    return newExpression.Members.Select(member => member.Name);
                }

                var unaryExpression = selectExpression.Body as UnaryExpression;
                if (unaryExpression != null)
                {
                    return new[] { ((MemberExpression)unaryExpression.Operand).Member.Name };
                }

                throw new InvalidOperationException("Unsupported expression type");
            }
        }

        internal override string TableAlias => tableAlias;

        internal override string TableName => tableName;

        internal override string WhereExpression => whereExpression;
    }
}
