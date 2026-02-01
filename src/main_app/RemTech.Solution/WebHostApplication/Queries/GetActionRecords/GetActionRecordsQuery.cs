using System.Text.Json;
using RemTech.SharedKernel.Core.Handlers;

namespace WebHostApplication.Queries.GetActionRecords;

/// <summary>
/// Запрос на получение записей действий с возможностью фильтрации.
/// </summary>
public sealed class GetActionRecordsQuery : IQuery
{
	private GetActionRecordsQuery() { }

	public Dictionary<string, string> Sort { get; init; } = [];

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
	public string? Status { get; private init; }

	/// <summary>
	/// Идентификаторы разрешений для фильтрации записей действий.
	/// </summary>
	public IEnumerable<Guid>? PermissionIdentifiers { get; private init; }

	/// <summary>
	/// Номер страницы для пагинации. Стандартно 1.
	/// </summary>
	public int Page { get; init; } = 1;

	/// <summary>
	/// Размер страницы для пагинации. Максимум 50.
	/// </summary>
	public int PageSize { get; init; } = 50;

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
	/// Устанавливает номер страницы для пагинации.
	/// </summary>
	/// <param name="page">Номер страницы для пагинации.</param>
	/// <returns>Запрос с установленным номером страницы для пагинации.</returns>
	public GetActionRecordsQuery WithCustomPage(int? page)
	{
		return Copy(this, page: page);
	}

	/// <summary>
	/// Устанавливает размер страницы для пагинации.
	/// </summary>
	/// <param name="pageSize">Размер страницы для пагинации.</param>
	/// <returns>Запрос с установленным размером страницы для пагинации.</returns>
	public GetActionRecordsQuery WithCustomPageSize(int? pageSize)
	{
		return Copy(this, pageSize: pageSize);
	}

	/// <summary>
	/// Устанавливает Email для поиска записей действий.
	/// </summary>
	/// <param name="loginSearch">Логин для поиска записей действий.</param>
	/// <returns>Запрос с установленным логином для поиска записей действий.</returns>
	public GetActionRecordsQuery WithLoginSearch(string? loginSearch)
	{
		return Copy(this, loginSearch: loginSearch);
	}

	/// <summary>
	/// Устанавливает Email для поиска записей действий.
	/// </summary>
	/// <param name="emailSearch">Email для поиска записей действий.</param>
	/// <returns>Запрос с установленным Email для поиска записей действий.</returns>
	public GetActionRecordsQuery WithEmailSearch(string? emailSearch)
	{
		return Copy(this, emailSearch: emailSearch);
	}

	/// <summary>
	/// Устанавливает имя статуса операции для фильтрации записей действий.
	/// </summary>
	/// <param name="status">Имя статуса для фильтрации.</param>
	/// <returns>Запрос с установленными именем статуса для фильтрации записей действий.</returns>
	public GetActionRecordsQuery WithStatusName(string? status)
	{
		return Copy(this, status: status);
	}

	public GetActionRecordsQuery WithSort(Dictionary<string, string>? sort)
	{
		return Copy(this, sort: sort);
	}

	/// <summary>
	/// Устанавливает идентификаторы разрешений для фильтрации записей действий.
	/// </summary>
	/// <param name="permissionIdentifiers">Идентификаторы разрешений для фильтрации записей действий.</param>
	/// <returns>Запрос с установленными идентификаторами разрешений для фильтрации записей действий.</returns>
	public GetActionRecordsQuery WithPermissionIdentifiers(IEnumerable<Guid>? permissionIdentifiers)
	{
		return Copy(this, permissionIdentifiers: permissionIdentifiers);
	}

	/// <summary>
	/// Устанавливает имя действия для поиска записей действий.
	/// </summary>
	/// <param name="actionNameSearch">Имя действия для поиска записей действий.</param>
	/// <returns>Запрос с установленным именем действия для поиска записей действий.</returns>
	public GetActionRecordsQuery WithActionNameSearch(string? actionNameSearch)
	{
		return Copy(this, actionNameSearch: actionNameSearch);
	}

	/// <summary>
	/// Устанавливает дату начала диапазона для фильтрации записей действий.
	/// </summary>
	/// <param name="startDate">Дата начала диапазона для фильтрации записей действий.</param>
	/// <returns>Запрос с установленной датой начала диапазона для фильтрации записей действий.</returns>
	public GetActionRecordsQuery WithStartDate(DateTime? startDate)
	{
		return Copy(this, startDate: startDate);
	}

	/// <summary>
	/// Устанавливает дату окончания диапазона для фильтрации записей действий.
	/// </summary>
	/// <param name="endDate">Дата окончания диапазона для фильтрации записей действий.</param>
	/// <returns>Запрос с установленной датой окончания диапазона для фильтрации записей действий.</returns>
	public GetActionRecordsQuery WithEndDate(DateTime? endDate)
	{
		return Copy(this, endDate: endDate);
	}

	/// <summary>
	/// Устанавливает идентификатор вызывающего запроса.
	/// </summary>
	/// <param name="guidOfRequestInvoker">Идентификатор вызывающего запроса.</param>
	/// <returns>Запрос с установленным идентификатором вызывающего запроса.</returns>
	public GetActionRecordsQuery WithIdOfRequestInvoker(Guid? guidOfRequestInvoker)
	{
		return Copy(this, guidOfRequestInvoker: guidOfRequestInvoker);
	}

	/// <summary>
	/// Устанавливает флаг игнорирования ошибок при получении записей действий.
	/// </summary>
	/// <param name="ignoreErrors">Флаг игнорирования ошибок при получении записей действий.</param>
	/// <returns>Запрос с установленным флагом игнорирования ошибок при получении записей действий.</returns>
	public GetActionRecordsQuery WithIgnoreErrors(bool ignoreErrors)
	{
		return Copy(this, ignoreErrors: ignoreErrors);
	}

	/// <summary>
	/// Создает новый экземпляр без фильтров.
	/// </summary>
	/// <returns>Новый экземпляр <see cref="GetActionRecordsQuery"/>.</returns>
	public static GetActionRecordsQuery Create()
	{
		return new();
	}

	/// <summary>
	/// Перечисляет пары ключ-значение для сортировки.
	/// </summary>
	/// <returns>Ключ значения пары для сортировки.</returns>
	public IEnumerable<KeyValuePair<string, string>> EnumerateSort()
	{
		foreach (KeyValuePair<string, string> pair in Sort)
		{
			yield return pair;
		}
	}

	private static GetActionRecordsQuery Copy(
		GetActionRecordsQuery origin,
		string? loginSearch = null,
		string? emailSearch = null,
		string? status = null,
		IEnumerable<Guid>? permissionIdentifiers = null,
		string? actionNameSearch = null,
		DateTime? startDate = null,
		DateTime? endDate = null,
		Guid? guidOfRequestInvoker = null,
		bool? ignoreErrors = null,
		int? page = null,
		int? pageSize = null,
		Dictionary<string, string>? sort = null
	)
	{
		return new()
		{
			LoginSearch = loginSearch ?? origin.LoginSearch,
			EmailSearch = emailSearch ?? origin.EmailSearch,
			Status = status ?? origin.Status,
			PermissionIdentifiers = permissionIdentifiers ?? origin.PermissionIdentifiers,
			ActionNameSearch = actionNameSearch ?? origin.ActionNameSearch,
			StartDate = startDate ?? origin.StartDate,
			EndDate = endDate ?? origin.EndDate,
			IdOfRequestInvoker = guidOfRequestInvoker ?? origin.IdOfRequestInvoker,
			IgnoreErrors = ignoreErrors ?? origin.IgnoreErrors,
			Page = CanUsePageFromArgument(page, out int validPage) ? validPage : origin.Page,
			PageSize = CanUsePageSizeFromArgument(pageSize, out int validPageSize) ? validPageSize : origin.PageSize,
			Sort = sort ?? origin.Sort,
		};
	}

	private static bool CanUsePageFromArgument(int? page, out int validPage)
	{
		if (page == null || page.Value < 1)
		{
			validPage = 1;
			return false;
		}

		validPage = page.Value;
		return true;
	}

	private static bool CanUsePageSizeFromArgument(int? pageSize, out int validPageSize)
	{
		if (pageSize is null || pageSize.Value > 50)
		{
			validPageSize = 50;
			return false;
		}

		validPageSize = pageSize.Value;
		return true;
	}

	/// <summary>
	/// Сериализует объект в строку JSON.
	/// </summary>
	/// <returns>JSON строка - запрос.</returns>
	public override string ToString()
	{
		return JsonSerializer.Serialize(this);
	}
}
