using RemTech.SharedKernel.Core.Handlers;

namespace WebHostApplication.Queries.GetActionRecords;

/// <summary>
/// Запрос на получение записей действий с возможностью фильтрации.
/// </summary>
public sealed class GetActionRecordsQuery : IQuery
{
	private GetActionRecordsQuery() { }

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

	/// <summary>
	/// Создает новый экземпляр без фильтров.
	/// </summary>
	/// <returns>Новый экземпляр <see cref="GetActionRecordsQuery"/>.</returns>
	public static GetActionRecordsQuery Create() => new();

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
