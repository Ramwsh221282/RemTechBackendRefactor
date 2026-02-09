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

    private static WhereClauseBuilderDelegate<GetActionRecordsQuery> LoginSearch()
    {
        return (query, parameters) =>
        {
            if (string.IsNullOrWhiteSpace(query.LoginSearch))
            {
                return string.Empty;
            }

            parameters.Add("@I_LoginSearch", query.LoginSearch, DbType.String);
            return "a.login ILIKE '%' || @I_LoginSearch || '%'";
        };
    }

    private static WhereClauseBuilderDelegate<GetActionRecordsQuery> EmailSearch()
    {
        return (query, parameters) =>
        {
            if (string.IsNullOrWhiteSpace(query.EmailSearch))
            {
                return string.Empty;
            }

            parameters.Add("@I_EmailSearch", query.EmailSearch, DbType.String);
            return "a.email ILIKE '%' || @I_EmailSearch || '%'";
        };
    }

    private static WhereClauseBuilderDelegate<GetActionRecordsQuery> ActionNameSearch(EmbeddingsProvider provider)
    {
        return (source, parameters) =>
        {
            if (string.IsNullOrWhiteSpace(source.ActionNameSearch))
            {
                return string.Empty;
            }
            
            Vector vector = new(provider.Generate(source.ActionNameSearch));
            parameters.Add("@I_ActionNameSearchEmbedding", vector);
            parameters.Add("@I_ActionNameSearch", source.ActionNameSearch, DbType.String);

            return """
                   (
                   	ar.name ILIKE '%' || @I_ActionNameSearch || '%' OR
                   	ts_rank_cd(ts_vector_field, plainto_tsquery('russian', @I_ActionNameSearch)) > 0 OR
                   	(ar.embedding <=> @I_ActionNameSearchEmbedding) < 0.5
                   )
                   """;
        };
    }

    private static WhereClauseBuilderDelegate<GetActionRecordsQuery> DateRangeFilter()
    {
        return (query, parameters) =>
        {
            if (query.StartDate == null || query.EndDate == null)
            {
                return string.Empty;
            }

            parameters.Add("@I_StartDate", query.StartDate.Value, DbType.DateTime);
            parameters.Add("@I_EndDate", query.EndDate.Value, DbType.DateTime);
            return "ar.created_at BETWEEN @I_StartDate AND @I_EndDate";
        };
    }
    

    private static WhereClauseBuilderDelegate<GetActionRecordsQuery> OperationStatusesFilter()
    {
        return (query, parameters) =>
        {
            if (string.IsNullOrWhiteSpace(query.Status))
            {
                return string.Empty;
            }

            parameters.Add("@I_StatusName", query.Status, DbType.String);
            return "ar.severity = @I_StatusName";
        };
    }

    private static WhereClauseBuilderDelegate<GetActionRecordsQuery> IgnoreSelfFilter()
    {
        return (query, parameters) =>
        {
            if (query.IdOfRequestInvoker == null)
            {
                return string.Empty;
            }

            parameters.Add("@I_InvokerId", query.IdOfRequestInvoker.Value, DbType.Guid);
            return "ar.invoker_id <> @I_InvokerId";
        };
    }

    private static WhereClauseBuilderDelegate<GetActionRecordsQuery> IgnoreAnonymousFilter()
    {
        return (source, _) => source.IgnoreAnonymous is null ? string.Empty
            : source.IgnoreAnonymous.Value ? "a.id IS NOT NULL"
            : string.Empty; 
    }

    private static WhereClauseBuilderDelegate<GetActionRecordsQuery> IgnoreErrorsFilter()
    {
        return (query, _) => !query.IgnoreErrors ? string.Empty : "ar.error IS NULL";
    }

    private static WhereClauseBuilderDelegate<GetActionRecordsQuery> PermissionsFilter()
    {
        return (query, parameters) =>
        {
            Guid[] ids = query.PermissionIdentifiers is null ? [] : [.. query.PermissionIdentifiers];
            if (ids.Length == 0)
            {
                return string.Empty;
            }

            parameters.Add("@I_PermissionIds", ids);
            return """
                   EXISTS 
                   	(
                   		SELECT 1 FROM jsonb_array_elements(up.permissions) AS perm
                   	 	WHERE (perm->>'Id')::uuid = ANY(@I_PermissionIds)
                   	)
                   """;
        };
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
                ActionNameSearch(provider), 
                IgnoreErrorsFilter(),
                IgnoreAnonymousFilter(),
                LoginSearch(),
                EmailSearch(),
                DateRangeFilter(),
                OperationStatusesFilter(),
                PermissionsFilter(),
                IgnoreSelfFilter(),
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

			if (mode is not ("ASC" or "DESC"))
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

		if (!string.IsNullOrWhiteSpace(query.ActionNameSearch))
		{
			orderByClauses.Add("(ar.embedding <=> @I_ActionNameSearchEmbedding) ASC");
		}

		return SqlBuilderDelegate.BuildOrderByClause(() => orderByClauses);
	}
}
