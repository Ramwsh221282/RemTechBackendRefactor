using System.Data;
using Dapper;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using WebHostApplication.Queries.GetActionRecords;

namespace WebHostApplication.Queries.GetActionRecords;

/// <summary>
/// Запрос на получение записей действий с возможностью фильтрации.
/// </summary>
public sealed class GetActionRecordsQuery : IQuery
{
	/// <summary>
	/// Инициализирует новый экземпляр класса <see cref="GetActionRecordsQuery"/>.
	/// </summary>
	public string? LoginSearch { get; private init; }

	/// <summary>
	/// Email для поиска записей действий.
	/// </summary>
	public string? EmailSearch { get; private init; }

	/// <summary>
	/// Имена статусов для фильтрации записей действий.
	/// </summary>
	public IEnumerable<string>? StatusNames { get; private init; }

	/// <summary>
	/// Идентификаторы разрешений для фильтрации записей действий.
	/// </summary>
	public IEnumerable<Guid>? PermissionIdentifiers { get; private init; }

	/// <summary>
	/// Имя действия для поиска записей действий.
	/// </summary>
	public string? ActionNameSearch { get; private init; }

	/// <summary>
	/// Дата начала диапазона для фильтрации записей действий.
	/// </summary>
	public DateTime? StartDate { get; private init; }

	/// <summary>
	/// Дата окончания диапазона для фильтрации записей действий.
	/// </summary>
	public DateTime? EndDate { get; private init; }

	/// <summary>
	/// Идентификатор вызывающего запроса. Нужен чтобы не включать действия пользователя с этим ID.
	/// Короче говоря, не видеть свои же действия в системе.
	/// </summary>
	public Guid? IdOfRequestInvoker { get; private init; }

	/// <summary>
	/// Игнорировать ошибки при получении записей действий.
	/// </summary>
	public bool IgnoreErrors { get; init; }

	/// <summary>
	/// Устанавливает Email для поиска записей действий.
	/// </summary>
	/// <param name="loginSearch">Логин для поиска записей действий.</param>
	/// <returns>Запрос с установленным логином для поиска записей действий.</returns>
	public GetActionRecordsQuery WithLoginSearch(string? loginSearch) => Copy(this, loginSearch: loginSearch);

	/// <summary>
	/// Устанавливает Email для поиска записей действий.
	/// </summary>
	/// <param name="emailSearch">Email для поиска записей действий.</param>
	/// <returns>Запрос с установленным Email для поиска записей действий.</returns>
	public GetActionRecordsQuery WithEmailSearch(string? emailSearch) => Copy(this, emailSearch: emailSearch);

	/// <summary>
	/// Устанавливает имена статусов для фильтрации записей действий.
	/// </summary>
	/// <param name="statusNames">Имена статусов для фильтрации записей действий.</param>
	/// <returns>Запрос с установленными именами статусов для фильтрации записей действий.</returns>
	public GetActionRecordsQuery WithStatusNames(IEnumerable<string>? statusNames) =>
		Copy(this, statusNames: statusNames);

	/// <summary>
	/// Устанавливает идентификаторы разрешений для фильтрации записей действий.
	/// </summary>
	/// <param name="permissionIdentifiers">Идентификаторы разрешений для фильтрации записей действий.</param>
	/// <returns>Запрос с установленными идентификаторами разрешений для фильтрации записей действий.</returns>
	public GetActionRecordsQuery WithPermissionIdentifiers(IEnumerable<Guid>? permissionIdentifiers) =>
		Copy(this, permissionIdentifiers: permissionIdentifiers);

	/// <summary>
	/// Устанавливает имя действия для поиска записей действий.
	/// </summary>
	/// <param name="actionNameSearch">Имя действия для поиска записей действий.</param>
	/// <returns>Запрос с установленным именем действия для поиска записей действий.</returns>
	public GetActionRecordsQuery WithActionNameSearch(string? actionNameSearch) =>
		Copy(this, actionNameSearch: actionNameSearch);

	/// <summary>
	/// Устанавливает дату начала диапазона для фильтрации записей действий.
	/// </summary>
	/// <param name="startDate">Дата начала диапазона для фильтрации записей действий.</param>
	/// <returns>Запрос с установленной датой начала диапазона для фильтрации записей действий.</returns>
	public GetActionRecordsQuery WithStartDate(DateTime? startDate) => Copy(this, startDate: startDate);

	/// <summary>
	/// Устанавливает дату окончания диапазона для фильтрации записей действий.
	/// </summary>
	/// <param name="endDate">Дата окончания диапазона для фильтрации записей действий.</param>
	/// <returns>Запрос с установленной датой окончания диапазона для фильтрации записей действий.</returns>
	public GetActionRecordsQuery WithEndDate(DateTime? endDate) => Copy(this, endDate: endDate);

	/// <summary>
	/// Устанавливает идентификатор вызывающего запроса.
	/// </summary>
	/// <param name="guidOfRequestInvoker">Идентификатор вызывающего запроса.</param>
	/// <returns>Запрос с установленным идентификатором вызывающего запроса.</returns>
	public GetActionRecordsQuery WithIdOfRequestInvoker(Guid? guidOfRequestInvoker) =>
		Copy(this, guidOfRequestInvoker: guidOfRequestInvoker);

	/// <summary>
	/// Устанавливает флаг игнорирования ошибок при получении записей действий.
	/// </summary>
	/// <param name="ignoreErrors">Флаг игнорирования ошибок при получении записей действий.</param>
	/// <returns>Запрос с установленным флагом игнорирования ошибок при получении записей действий.</returns>
	public GetActionRecordsQuery WithIgnoreErrors(bool ignoreErrors) => Copy(this, ignoreErrors: ignoreErrors);

	private static GetActionRecordsQuery Copy(
		GetActionRecordsQuery origin,
		string? loginSearch = null,
		string? emailSearch = null,
		IEnumerable<string>? statusNames = null,
		IEnumerable<Guid>? permissionIdentifiers = null,
		string? actionNameSearch = null,
		DateTime? startDate = null,
		DateTime? endDate = null,
		Guid? guidOfRequestInvoker = null,
		bool? ignoreErrors = null
	) =>
		new()
		{
			LoginSearch = loginSearch ?? origin.LoginSearch,
			EmailSearch = emailSearch ?? origin.EmailSearch,
			StatusNames = statusNames ?? origin.StatusNames,
			PermissionIdentifiers = permissionIdentifiers ?? origin.PermissionIdentifiers,
			ActionNameSearch = actionNameSearch ?? origin.ActionNameSearch,
			StartDate = startDate ?? origin.StartDate,
			EndDate = endDate ?? origin.EndDate,
			IdOfRequestInvoker = guidOfRequestInvoker ?? origin.IdOfRequestInvoker,
			IgnoreErrors = ignoreErrors ?? origin.IgnoreErrors,
		};
}

/// <summary>
/// Результат запроса <see cref="GetActionRecordsQuery"/> .
/// </summary>
public sealed class GetActionRecordsQueryResponse
{
	/// <summary>
	/// Идентификатор записи действия.
	/// </summary>
	public required string UserLogin { get; init; }

	/// <summary>
	/// Email пользователя, выполнившего действие.
	/// </summary>
	public required string UserEmail { get; init; }

	/// <summary>
	/// Разрешения пользователя, выполнившего действие.
	/// </summary>
	public required IReadOnlyList<string> UserPermissions { get; init; }

	/// <summary>
	/// Имя действия.
	/// </summary>
	public required string ActionName { get; init; }

	/// <summary>
	/// Уровень серьезности действия.
	/// </summary>
	public required string ActionSeverity { get; init; }

	/// <summary>
	/// Сообщение об ошибке, если таковая имелась.
	/// </summary>
	public required string? ErrorMessage { get; init; }

	/// <summary>
	/// Временная метка действия.
	/// </summary>
	public required DateTime ActionTimestamp { get; init; }
}

/// <summary>
/// Обработчик запроса <see cref="GetActionRecordsQuery"/> .
/// </summary>
public sealed class GetActionRecordsQueryHandler(NpgSqlSession session, EmbeddingsProvider embeddings)
	: IQueryHandler<GetActionRecordsQuery, IReadOnlyList<GetActionRecordsQueryResponse>>
{
	private NpgSqlSession Session { get; } = session;
	private EmbeddingsProvider Embeddings { get; } = embeddings;

	public async Task<IReadOnlyList<GetActionRecordsQueryResponse>> Handle(
		GetActionRecordsQuery query,
		CancellationToken ct = default
	) { }

	private (DynamicParameters parameters, string sql) CreateSql(GetActionRecordsQuery query)
	{
		string sql = $"""
	SELECT
	ar.id as record_id,
	a.login as user_login,
	a.email as user_email,
	ar.name as action_name,
	ar.severity as action_severity,
	ar.error as error_message,
	ar.created_at as action_timestamp,
	(
		SELECT
			COALESCE(
				jsonb_agg (
					jsonb_build_object ('pName', p.name, 'pDescription', p.description)
				),
				'[]'
			)
		FROM
			identity_module.account_permissions ap
			LEFT JOIN identity_module.permissions p ON p.id = ap.permission_id
		WHERE
			ap.account_id = a.id
	) as user_permissions
	FROM
telemetry_module.action_records ar
	LEFT JOIN identity_module.accounts a ON ar.invoker_id = a.id
WHERE
	1 = 1
""";
	}

	private static string UseLoginSearch(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.LoginSearch))
			return string.Empty;

		parameters.Add("@LoginSearch", query.LoginSearch, DbType.String);
		return "a.login ILIKE '%' || @LoginSearch || '%'";
	}

	private static string UseEmailSearch(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.EmailSearch))
			return string.Empty;

		parameters.Add("@EmailSearch", query.EmailSearch, DbType.String);
		return "a.email ILIKE '%' || @EmailSearch || '%'";
	}

	private string UseActionNameSearch(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (string.IsNullOrWhiteSpace(query.ActionNameSearch))
			return string.Empty;

		parameters.Add("@ActionNameSearch", query.ActionNameSearch, DbType.String);
		return """
			(
				ar.name ILIKE '%' || @ActionNameSearch || '%' OR
				ts_rank_cd(ts_vector_field, to_tsquery('russian', @ActionNameSearch))) > 0 OR
				(embedding_vector <=> @ActionNameSearchEmbedding) < 0.5
			)
			""";
	}

	private string UseDateRangeFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (query.StartDate == null || query.EndDate == null)
			return string.Empty;

		parameters.Add("@StartDate", query.StartDate.Value, DbType.DateTime);
		parameters.Add("@EndDate", query.EndDate.Value, DbType.DateTime);
		return "ar.created_at BETWEEN @StartDate AND @EndDate";
	}

	private string UsePermissionsFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (query.PermissionIdentifiers?.Any() != true)
			return string.Empty;

		// TODO: implement
	}

	private string UseOperationStatusesFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (query.StatusNames?.Any() != true)
			return string.Empty;

		// TODO: implement
	}

	private string UseIgnoreSelfFilter(GetActionRecordsQuery query, DynamicParameters parameters)
	{
		if (query.IdOfRequestInvoker == null)
			return string.Empty;

		parameters.Add("@InvokerId", query.IdOfRequestInvoker.Value, DbType.Guid);
		return "ar.invoker_id <> @InvokerId";
	}

	private string UseIgnoreErrorsFilter(GetActionRecordsQuery query)
	{
		if (!query.IgnoreErrors)
			return string.Empty;

		return "ar.error IS NULL";
	}

	private static string CombineFilters()
}


// SELECT
// ar.id as record_id,
// a.login as user_login,
// a.email as user_email,
// ar.name as action_name,
// ar.severity as action_severity,
// ar.error as error_message,
// ar.created_at as action_timestamp,
// (SELECT COALESCE(
//     jsonb_agg(
//     jsonb_build_object(
//         'pName', p.name,
//         'pDescription', p.description
//     )
// ), '[]'
// ) FROM identity_module.account_permissions ap
//   LEFT JOIN identity_module.permissions p ON p.id = ap.permission_id
//   WHERE ap.account_id = a.id) as user_permissions
// FROM telemetry_module.action_records ar
// LEFT JOIN identity_module.accounts a ON ar.invoker_id = a.id
