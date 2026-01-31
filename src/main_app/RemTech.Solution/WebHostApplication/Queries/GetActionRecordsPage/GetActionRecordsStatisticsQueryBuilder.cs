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

	private static string UseLoginSearch(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.LoginSearch))
		{
			return string.Empty;
		}

		parameters.Add("@ST_LoginSearch", query.LoginSearch, DbType.String);
		return "a.login ILIKE '%' || @ST_LoginSearch || '%'";
	}

	private static string UseEmailSearch(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.EmailSearch))
		{
			return string.Empty;
		}

		parameters.Add("@ST_EmailSearch", query.EmailSearch, DbType.String);
		return "a.email ILIKE '%' || @ST_EmailSearch || '%'";
	}

	private static string UseActionNameSearch(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.ActionNameSearch))
		{
			return string.Empty;
		}

		parameters.Add("@ST_ActionNameSearch", query.ActionNameSearch, DbType.String);
		return """
			(
				ar.name ILIKE '%' || @ST_ActionNameSearch || '%' OR
				ts_rank_cd(ts_vector_field, to_tsquery('russian', @ST_ActionNameSearch))) > 0 OR
				(embedding_vector <=> @ST_ActionNameSearchEmbedding) < 0.5
			)
			""";
	}

	private static string UseDateRangeFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (query.StartDate == null || query.EndDate == null)
		{
			return string.Empty;
		}

		parameters.Add("@ST_StartDate", query.StartDate.Value, DbType.DateTime);
		parameters.Add("@ST_EndDate", query.EndDate.Value, DbType.DateTime);
		return "ar.created_at BETWEEN @ST_StartDate AND @ST_EndDate";
	}

	private static string UseSubQueryPermissionsFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		List<string> filters = ["ap.account_id = a.id"];
		if (query.PermissionIdentifiers?.Any() != true)
		{
			return string.Empty;
		}

		Guid[] ids = [.. query.PermissionIdentifiers];
		filters.Add("ap.permission_id = ANY(@ST_PermissionIds)");
		parameters.Add("@ST_PermissionIds", ids);
		return string.Join(" AND ", filters);
	}

	private static string UseOperationStatusesFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (query.StatusNames?.Any() != true)
		{
			return string.Empty;
		}

		string[] statusNames = [.. query.StatusNames];
		parameters.Add("@ST_StatusNames", statusNames);
		return "ar.severity = ANY(@ST_StatusNames)";
	}

	private static string UseIgnoreSelfFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (query.IdOfRequestInvoker == null)
		{
			return string.Empty;
		}

		parameters.Add("@ST_InvokerId", query.IdOfRequestInvoker.Value, DbType.Guid);
		return "ar.invoker_id <> @ST_InvokerId";
	}

	private static string UseIgnoreErrorsFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (!query.IgnoreErrors)
		{
			return string.Empty;
		}

		return "ar.error IS NULL";
	}

	private static string BuildMainQuerySqlFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		return SqlBuilderDelegate.CombineWhereClauses(
			query,
			parameters,
			[
				UseLoginSearch,
				UseEmailSearch,
				UseActionNameSearch,
				UseDateRangeFilter,
				UseOperationStatusesFilter,
				UseIgnoreErrorsFilter,
				UseIgnoreSelfFilter,
			]
		);
	}
}
