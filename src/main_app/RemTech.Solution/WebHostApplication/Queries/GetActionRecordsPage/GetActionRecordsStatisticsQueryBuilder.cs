using Dapper;
using WebHostApplication.CommonSql;
using WebHostApplication.Queries.GetActionRecords;

namespace WebHostApplication.Queries.GetActionRecordsPage;

//TODO: сделать кастомную фильтрацию по срезу дат: за 7 дней, за месяц.
internal static class GetActionRecordsStatisticsQueryBuilder
{
	public static string CreateFilterSql(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		return BuildMainQuerySqlFilter(query, parameters);
	}

	private static string BuildMainQuerySqlFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		return SqlBuilderDelegate.CombineWhereClauses(query, parameters, []);
	}
}
