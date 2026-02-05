using System.Data;
using Dapper;
using WebHostApplication.CommonSql;
using WebHostApplication.Queries.GetActionRecords;

namespace WebHostApplication.Queries.GetActionRecordsPage;

internal static class GetActionRecordsStatisticsQueryBuilder
{
	public static string CreateFilterSql(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		return BuildMainQuerySqlFilter(query, parameters);
	}

	private static string BuildMainQuerySqlFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		return SqlBuilderDelegate.CombineWhereClauses(
			query,
			parameters,
			[WeekDateRangeFilter(), DateRangeFilter()]
		);
	}

    private static WhereClauseBuilderDelegate<GetActionRecordsQuery> WeekDateRangeFilter()
    {
        return (query, _) => query.UsingWeek.HasValue && query.UsingWeek.Value
            ? "Date (ar.created_at) >= Date (CURRENT_DATE - INTERVAL '7 days')"
            : string.Empty;
    }

    private static WhereClauseBuilderDelegate<GetActionRecordsQuery> DateRangeFilter()
    {
        return (query, parameters) =>
        {
            List<string> clauses = [];

            if (query.UsingWeek.HasValue && query.UsingWeek.Value)
            {
                return string.Empty;
            }

            if (query.ChartStartDate.HasValue)
            {
                clauses.Add("ar.created_at >= @StartDate");
                parameters.Add("StartDate", query.ChartStartDate.Value, DbType.DateTime);
            }

            if (query.ChartEndDate.HasValue)
            {
                clauses.Add("ar.created_at <= @EndDate");
                parameters.Add("EndDate", query.ChartEndDate.Value, DbType.DateTime);
            }

            return clauses.Count == 0 ? string.Empty : string.Join(" AND ", clauses);
        };
    }
}
