using System.Text;

namespace RemTech.ParsedAdvertisements.Core.Database.SqlStringGeneration;

public static class SqlGenerator
{
    public static SqlGeneration SourceSql(string sourceSql) => () => new SqlGenerated(sourceSql);

    public static SqlGeneration ApplyFilters(this SqlGeneration generation, SqlFilters filters)
    {
        return () =>
        {
            StringBuilder builder = new(generation().Sql);
            if (filters.Filters.Any() == false)
                return new SqlGenerated(builder.ToString());
            string joined = string.Join(" AND ", filters.Filters);
            builder.AppendLine(" WHERE 1 = 1 AND ");
            builder.AppendLine(joined);
            return new SqlGenerated(builder.ToString());
        };
    }

    public static SqlGeneration ApplyOrdering(this SqlGeneration generation, SqlOrdering ordering)
    {
        return () =>
        {
            StringBuilder builder = new(generation().Sql);
            if (!string.IsNullOrWhiteSpace(ordering.Ordering))
                builder.AppendLine(ordering.Ordering);
            return new SqlGenerated(builder.ToString());
        };
    }

    public static SqlGeneration ApplyPagination(
        this SqlGeneration generation,
        SqlPagination pagination
    )
    {
        return () =>
        {
            StringBuilder builder = new(generation().Sql);
            if (!string.IsNullOrWhiteSpace(pagination.Pagination))
                builder.AppendLine(pagination.Pagination);
            return new SqlGenerated(builder.ToString());
        };
    }
}
