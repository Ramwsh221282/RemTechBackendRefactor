using System.Data;
using Dapper;
using Pgvector;
using RemTech.SharedKernel.NN;
using WebHostApplication.CommonSql;
using WebHostApplication.Queries.GetActionRecords;

namespace WebHostApplication.Queries.GetActionRecordsPage;

internal static class GetActionRecordsQueryBuilder
{
	public static string CreateOrderBySql(GetActionRecordsQuery query)
	{
		return BuildOrderByPart(query);
	}

	public static string CreateFilterSql(
		DynamicParameters parameters,
		GetActionRecordsQuery query,
		EmbeddingsProvider provider
	)
	{
		return BuildMainQuerySqlFilter(parameters, query, provider);
	}

	public static string CreatePaginationSql(DynamicParameters parameters, GetActionRecordsQuery query)
	{
		return BuildPaginationQueryPart(query, parameters);
	}

	private static string UseLoginSearch(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.LoginSearch))
		{
			return string.Empty;
		}

		parameters.Add("@I_LoginSearch", query.LoginSearch, DbType.String);
		return "a.login ILIKE '%' || @I_LoginSearch || '%'";
	}

	private static string UseEmailSearch(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.EmailSearch))
		{
			return string.Empty;
		}

		parameters.Add("@I_EmailSearch", query.EmailSearch, DbType.String);
		return "a.email ILIKE '%' || @I_EmailSearch || '%'";
	}

	private static string UseActionNameSearch(
		GetActionRecordsQuery query,
		DynamicParameters parameters,
		EmbeddingsProvider provider
	)
	{
		if (string.IsNullOrWhiteSpace(query.ActionNameSearch))
		{
			return string.Empty;
		}

		Vector vector = new(provider.Generate(query.ActionNameSearch));
		parameters.Add("@I_ActionNameSearchEmbedding", vector);
		parameters.Add("@I_ActionNameSearch", query.ActionNameSearch, DbType.String);

		return """
			(
				ar.name ILIKE '%' || @I_ActionNameSearch || '%' OR
				ts_rank_cd(ts_vector_field, to_tsquery('russian', @I_ActionNameSearch))) > 0 OR
				(embedding_vector <=> @I_ActionNameSearchEmbedding) < 0.5
			)
			""";
	}

	private static string UseDateRangeFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (query.StartDate == null || query.EndDate == null)
		{
			return string.Empty;
		}

		parameters.Add("@I_StartDate", query.StartDate.Value, DbType.DateTime);
		parameters.Add("@I_EndDate", query.EndDate.Value, DbType.DateTime);
		return "ar.created_at BETWEEN @I_StartDate AND @I_EndDate";
	}

	private static string UseSubQueryPermissionsFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		List<string> filters = ["ap.account_id = a.id"];
		if (query.PermissionIdentifiers?.Any() != true)
		{
			return string.Empty;
		}

		Guid[] ids = [.. query.PermissionIdentifiers];
		filters.Add("ap.permission_id = ANY(@I_PermissionIds)");
		parameters.Add("@I_PermissionIds", ids);
		return string.Join(" AND ", filters);
	}

	private static string UseOperationStatusesFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.Status))
		{
			return string.Empty;
		}

		parameters.Add("@I_StatusName", query.Status, DbType.String);
		return "ar.severity = @I_StatusName";
	}

	private static string UseIgnoreSelfFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (query.IdOfRequestInvoker == null)
		{
			return string.Empty;
		}

		parameters.Add("@I_InvokerId", query.IdOfRequestInvoker.Value, DbType.Guid);
		return "ar.invoker_id <> @I_InvokerId";
	}

	private static string UseIgnoreErrorsFilter(GetActionRecordsQuery query)
	{
		return !query.IgnoreErrors ? string.Empty : "ar.error IS NULL";
	}

	private static string BuildMainQuerySqlFilter(
		DynamicParameters parameters,
		GetActionRecordsQuery query,
		EmbeddingsProvider provider
	)
	{
		return SqlBuilderDelegate.CombineWhereClauses(
			query,
			parameters,
			[
				UseLoginSearch,
				UseEmailSearch,
				(quer, param) => UseActionNameSearch(quer, param, provider),
				UseDateRangeFilter,
				UseOperationStatusesFilter,
				(quer, param) => UseIgnoreErrorsFilter(quer),
				UseIgnoreSelfFilter,
				UseSubQueryPermissionsFilter,
			]
		);
	}

	private static string BuildPaginationQueryPart(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		return SqlBuilderDelegate.BuildPaginationClause(query, parameters, q => q.Page, q => q.PageSize);
	}

	private static string BuildOrderByPart(GetActionRecordsQuery query)
	{
		List<string> orderByClauses = [];
		foreach (KeyValuePair<string, string> sortEntry in query.EnumerateSort())
		{
			string field = sortEntry.Key;
			string mode = sortEntry.Value;
			if (mode == "NONE")
			{
				continue;
			}

			if (!(mode == "ASC" || mode == "DESC"))
			{
				continue;
			}

			string clause = field switch
			{
				"date" => $"ar.created_at {mode}",
				"login" => $"a.login {mode}",
				"email" => $"a.email {mode}",
				"name" => $"ar.name {mode}",
				_ => string.Empty,
			};

			orderByClauses.Add(clause);
		}

		return SqlBuilderDelegate.BuildOrderByClause(() => orderByClauses);
	}
}
