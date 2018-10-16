using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SqlGen.SqlGenerators.QueryBuilders
{
    internal class AnsiSql : ISqlBuilder
    {
        protected const string SELECT = "SELECT";
        protected const string FROM = "FROM";
        protected const string WHERE = "WHERE";
        protected const string ORDERBY = "ORDER BY";
        protected readonly QueryBuilder queryBuilder;

        public AnsiSql(QueryBuilder queryBuilder)
        {
            this.queryBuilder = queryBuilder;
        }

        protected string SelectPart { get; private set; }
        protected string TopPart { get; private set; }
        protected string DistinctPart { get; private set; }
        protected string FromPart { get; private set; }
        protected string WherePart { get; private set; }
        protected string OrderByPart { get; private set; }

        public override string ToString()
        {
            // Top
            TopPart = queryBuilder.take.HasValue ? $"TOP({queryBuilder.take.Value})" : string.Empty;

            // Distinct
            DistinctPart = queryBuilder.distinct ? "DISTINCT" : string.Empty;

            // Select
            SelectPart = $"{string.Join(",", queryBuilder.query.Columns.Select(column => $"[{queryBuilder.query.TableAlias ?? queryBuilder.query.TableName}].[{column}]"))}";
            queryBuilder.joins.ForEach(join => SelectPart += $",{string.Join(",", join.Table2.Columns.Select(column => $"[{join.Table2.TableAlias ?? join.Table2.TableName}].[{column}]"))}");

            // From
            FromPart = $"[{queryBuilder.query.TableName}]";
            if (!string.IsNullOrWhiteSpace(queryBuilder.query.TableAlias))
            {
                FromPart += $" AS [{queryBuilder.query.TableAlias}]";
            }

            queryBuilder.joins.ForEach(join =>
            {
                string table1;
                string table2;
                string expression;
                Match match = Regex.Match(join.Expression, $@"^\((?<{nameof(table1)}>.+)\, (?<{nameof(table2)}>.+)\) => \((?<{nameof(expression)}>.+)\)$", RegexOptions.Singleline);
                table1 = match.Groups[nameof(table1)].Value;
                table2 = match.Groups[nameof(table2)].Value;
                FromPart += $" {Enum.GetName(join.JoinType.GetType(), join.JoinType).ToUpper()} JOIN [{join.Table2.TableName}]";
                if (!string.IsNullOrWhiteSpace(join.Table2.TableAlias))
                {
                    FromPart += $" AS [{join.Table2.TableAlias}]";
                }

                FromPart += $" ON {ToSql(match.Groups[nameof(expression)].Value, (table1, join.Table1.TableAlias ?? join.Table1.TableName), (table2, join.Table2.TableAlias ?? join.Table2.TableName))}";
            });

            // Where
            if (!string.IsNullOrWhiteSpace(queryBuilder.query.WhereExpression))
            {
                string table;
                string expression;
                Match match = Regex.Match(queryBuilder.query.WhereExpression, $@"^(?<{nameof(table)}>.+) => (?<{nameof(expression)}>.+)$", RegexOptions.Singleline);
                table = match.Groups[nameof(table)].Value;
                WherePart = $"{ToSql(match.Groups[nameof(expression)].Value, (table, queryBuilder.query.TableAlias ?? queryBuilder.query.TableName))}";
            }

            // OrderBy
            if (queryBuilder.orderby.Any())
            {
                OrderByPart = $"{string.Join(",", queryBuilder.orderby.Select(x => $"[{x.TableName}].[{x.PropertyName}]"))}";
            }

            return $"{SELECT}{(!string.IsNullOrWhiteSpace(TopPart) ? " " + TopPart : string.Empty)}{(!string.IsNullOrWhiteSpace(DistinctPart) ? " " + DistinctPart : string.Empty)} {SelectPart} {FROM} {FromPart}{(!string.IsNullOrWhiteSpace(WherePart) ? " " + WHERE + " " + WherePart : string.Empty)}{(!string.IsNullOrWhiteSpace(OrderByPart) ? " " + ORDERBY + " " + OrderByPart : string.Empty)}";
        }

        private static string ToSql(string expression, params (string, string)[] tableNames)
        {
            foreach ((string expressionName, string tableName) in tableNames)
            {
                expression = expression.Replace($"{expressionName}.", $"[{tableName}].");
            }

            foreach (Match match in Regex.Matches(expression, @"\.(\w+)"))
            {
                expression = expression.Replace(match.Value, $".[{match.Groups[1].Value}]");
            }

            expression = expression
                .Replace("Not(", "NOT (")
                .Replace(" And ", " AND ")
                .Replace(" AndAlso ", " AND ")
                .Replace(" Or ", " OR ")
                .Replace(" OrElse ", " OR ")
                .Replace(" == null", " IS NULL")
                .Replace(" != null", " IS NOT NULL")
                .Replace(" > ", ">")
                .Replace(" >= ", ">=")
                .Replace(" < ", "<")
                .Replace(" <= ", "<=")
                .Replace(" != ", "<>")
                .Replace(" == ", "=")
                .Replace("\"", "'");
            return expression;
        }
    }
}
