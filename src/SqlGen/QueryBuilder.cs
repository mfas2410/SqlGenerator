using SqlGen.SqlGenerators.QueryBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SqlGen
{
    public sealed class QueryBuilder
    {
        private readonly ISqlBuilder sqlGenerator;
        internal bool distinct;
        internal List<Join> joins;
        internal List<(string TableName, string PropertyName)> orderby;
        internal Query query;
        internal int? skip;
        internal int? take;

        public QueryBuilder(Dialect dialect)
        {
            if (!Enum.IsDefined(typeof(Dialect), dialect)) throw new ArgumentOutOfRangeException(((int)dialect).ToString(), nameof(dialect));
            sqlGenerator = (ISqlBuilder)Activator.CreateInstance(Type.GetType($"{typeof(ISqlBuilder).Namespace}.{Enum.GetName(typeof(Dialect), dialect)}"), this);
        }

        public Query<T> From<T>(Expression<Func<T>> alias = null) where T : class
        {
            distinct = false;
            joins = new List<Join>();
            orderby = new List<(string, string)>();
            skip = null;
            take = null;
            Query<T> query = new Query<T>(this, alias);
            this.query = query;
            return query;
        }

        public override string ToString()
        {
            if (query == null) throw new InvalidOperationException("No query");
            return sqlGenerator.ToString();
        }

        internal void Distinct()
        {
            distinct = true;
        }

        internal void OrderBy<T>(Expression<Func<T, object>> expression) where T : class
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            orderby.Clear();

            var memberExpression = expression.Body as MemberExpression;
            var newExpression = expression.Body as NewExpression;
            var unaryExpression = expression.Body as UnaryExpression;
            if (memberExpression == null && newExpression == null && (unaryExpression == null || unaryExpression.Operand.NodeType != ExpressionType.MemberAccess))
            {
                throw new ArgumentException("Unsupported expression type", nameof(expression));
            }

            var tableNames = joins.ToDictionary(x => x.Table2.TableName, x => x.Table2.TableAlias ?? x.Table2.TableName);
            tableNames.Add(query.TableName, query.TableAlias ?? query.TableName);

            if (memberExpression != null)
            {
                var tableName = typeof(T).Name;
                if (!tableNames.ContainsKey(tableName))
                {
                    throw new InvalidOperationException("Unknown table alias or name");
                }

                orderby.Add((tableNames[tableName], memberExpression.Member.Name));
                return;
            }

            if (newExpression != null)
            {
                var tableName = typeof(T).Name;
                if (!tableNames.ContainsKey(tableName))
                {
                    throw new InvalidOperationException("Unknown table alias or name");
                }

                orderby.AddRange(newExpression.Members.Select(x => (tableName, x.Name)));
                return;
            }

            if (unaryExpression != null)
            {
                var tableName = typeof(T).Name;
                if (!tableNames.ContainsKey(tableName))
                {
                    throw new InvalidOperationException("Unknown table alias or name");
                }

                var expressionName = ((MemberExpression)unaryExpression.Operand).Expression.ToString();
                orderby.Add((tableNames[tableName], unaryExpression.Operand.ToString().Replace($"{expressionName}.", string.Empty)));
                return;
            }

            throw new InvalidOperationException("Unsupported expression type");
        }

        internal void OrderBy(Expression<Func<object>> expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            orderby.Clear();

            var newExpression = expression.Body as NewExpression;
            if (newExpression == null)
            {
                throw new ArgumentException("Unsupported expression type", nameof(expression));
            }

            var tableNames = joins.ToDictionary(x => x.Table2.TableName, x => x.Table2.TableAlias ?? x.Table2.TableName);
            tableNames.Add(query.TableName, query.TableAlias ?? query.TableName);

            if (newExpression != null)
            {
                for (int index = 0; index < newExpression.Arguments.Count(); index++)
                {
                    var tableName = ((MemberExpression)newExpression.Arguments[index]).Expression.Type.Name;
                    if (!tableNames.ContainsKey(tableName))
                    {
                        throw new InvalidOperationException("Unknown table alias or name");
                    }

                    var propertyName = newExpression.Members[index].Name;
                    orderby.Add((tableNames[tableName], propertyName));
                }

                return;
            }

            throw new InvalidOperationException("Unsupported expression type");
        }

        internal void Skip(int skip)
        {
            if (skip < 1)
            {
                throw new ArgumentException("Must be > 0", nameof(skip));
            }

            this.skip = skip;
        }

        internal void Take(int take)
        {
            if (take < 1)
            {
                throw new ArgumentException("Must be > 0", nameof(take));
            }

            this.take = take;
        }
    }
}
