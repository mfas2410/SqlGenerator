namespace SqlGen.SqlGenerators.QueryBuilders
{
    internal sealed class TSql2017 : AnsiSql, ISqlBuilder
    {
        public TSql2017(QueryBuilder queryBuilder) : base(queryBuilder)
        {
        }

        public override string ToString()
        {
            base.ToString();

            // AnsiSql without Top
            string sql = $"{SELECT}{(!string.IsNullOrWhiteSpace(DistinctPart) ? " " + DistinctPart : string.Empty)} {SelectPart} {FROM} {FromPart}{(!string.IsNullOrWhiteSpace(WherePart) ? " " + WHERE + " " + WherePart : string.Empty)}{(!string.IsNullOrWhiteSpace(OrderByPart) ? " " + ORDERBY + " " + OrderByPart : string.Empty)}";

            // Top (Offset/fetch)
            if (queryBuilder.skip.HasValue || queryBuilder.take.HasValue)
            {
                sql += $" OFFSET {queryBuilder.skip ?? 0} ROWS FETCH NEXT {queryBuilder.take ?? int.MaxValue} ROWS ONLY";
            }

            return sql;
        }
    }
}
