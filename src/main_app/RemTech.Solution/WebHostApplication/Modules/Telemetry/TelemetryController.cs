using System.ComponentModel;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using WebHostApplication.ActionFilters.Attributes;
using WebHostApplication.Queries.GetActionRecords;
using WebHostApplication.Queries.Responses;

namespace WebHostApplication.Modules.Telemetry;

[TypeConverter(typeof(SortDictionaryQueryParameterConverter))]
public sealed class SortDictionary : Dictionary<string, string>
{
	private const char SORT_RELATION_SEPARATOR = ':';
	private const char ARGUMENTS_SEPARATOR = ',';

	public static SortDictionary FromString(string? input)
	{
		SortDictionary dictionary = [];
		if (string.IsNullOrWhiteSpace(input))
		{
			return dictionary;
		}

		string[] parts = SplitArguments(input);
		AddSortParts(dictionary, parts);
		return dictionary;
	}

	private static void AddSortParts(SortDictionary dictionary, string[] parts)
	{
		foreach (string part in parts)
		{
			Optional<KeyValuePair<string, string>> sortClause = ParseArgument(part);
			if (!sortClause.HasValue)
			{
				continue;
			}

			if (dictionary.ContainsKey(sortClause.Value.Key))
			{
				continue;
			}

			dictionary.Add(sortClause.Value.Key, sortClause.Value.Value);
		}
	}

	private static Optional<KeyValuePair<string, string>> ParseArgument(string argument)
	{
		string[] field_sortMode = argument.Split(SORT_RELATION_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
		if (field_sortMode.Length != 2)
		{
			return Optional.None<KeyValuePair<string, string>>();
		}

		string field = field_sortMode[0];
		string sortMode = field_sortMode[1];
		if (sortMode == "ASC" || sortMode == "DESC" || sortMode == "NONE")
		{
			return new KeyValuePair<string, string>(field, sortMode);
		}

		return Optional.None<KeyValuePair<string, string>>();
	}

	private static string[] SplitArguments(string input)
	{
		return input.Split(ARGUMENTS_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
	}
}

public sealed class SortDictionaryQueryParameterConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
	{
		return sourceType == typeof(string);
	}

	public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
	{
		if (value is string stringValue)
		{
			return SortDictionary.FromString(stringValue);
		}

		return base.ConvertFrom(context, culture, value);
	}
}

/// <summary>
/// Контроллер для работы с телеметрией.
/// </summary>
[ApiController]
[Route("api/telemetry")]
public sealed class TelemetryController : ControllerBase
{
	[HttpGet]
	public async Task<Envelope> GetData(
		[FromQuery(Name = "page")] int? page,
		[FromQuery(Name = "page-size")] int? pageSize,
		[FromQuery(Name = "permissions")] IEnumerable<Guid>? permissions,
		[FromQuery(Name = "sort")] SortDictionary? sort,
		[FromQuery(Name = "login")] string? login,
		[FromQuery(Name = "email")] string? email,
		[FromQuery(Name = "status")] string? status,
		[FromQuery(Name = "text-search")] string? textSearch,
		[FromServices] IQueryHandler<GetActionRecordsQuery, ActionRecordsPageResponse> handler,
		CancellationToken ct
	)
	{
		GetActionRecordsQuery query = GetActionRecordsQuery
			.Create()
			.WithCustomPage(page)
			.WithCustomPageSize(pageSize)
			.WithPermissionIdentifiers(permissions)
			.WithSort(sort)
			.WithLoginSearch(login)
			.WithEmailSearch(email)
			.WithStatusName(status)
			.WithActionNameSearch(textSearch);

		ActionRecordsPageResponse response = await handler.Handle(query, ct);
		return EnvelopedResultsExtensions.AsEnvelope(response);
	}

	/// <summary>
	/// Получить записи действий пользователя.
	/// </summary>
	/// <param name="page">Номер страницы для пагинации.</param>
	/// <param name="pageSize">Размер страницы для пагинации.</param>
	/// <param name="handler">Обработчик запроса для получения записей действий пользователя.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатами запроса.</returns>
	[VerifyToken]
	[HttpGet("records")]
	public async Task<Envelope> GetRecords(
		[FromQuery(Name = "page")] int? page,
		[FromQuery(Name = "page-size")] int? pageSize,
		[FromQuery(Name = "permissions")] IEnumerable<Guid>? permissions,
		[FromServices] IQueryHandler<GetActionRecordsQuery, GetActionRecordQueryResponse> handler,
		CancellationToken ct
	)
	{
		GetActionRecordsQuery query = GetActionRecordsQuery
			.Create()
			.WithCustomPage(page)
			.WithCustomPageSize(pageSize)
			.WithPermissionIdentifiers(permissions);

		GetActionRecordQueryResponse response = await handler.Handle(query, ct);
		return EnvelopedResultsExtensions.AsEnvelope(response);
	}

	/// <summary>
	/// Получить статистику записей действий пользователя.
	/// </summary>
	/// <param name="handler">Обработчик запроса для получения статистики записей действий пользователя.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатами запроса.</returns>
	[VerifyToken]
	[HttpGet("records/statistics")]
	public async Task<Envelope> GetRecordsStatistics(
		[FromServices] IQueryHandler<GetActionRecordsQuery, IReadOnlyList<ActionRecordsStatisticsResponse>> handler,
		CancellationToken ct
	)
	{
		GetActionRecordsQuery query = GetActionRecordsQuery.Create();
		IReadOnlyList<ActionRecordsStatisticsResponse> response = await handler.Handle(query, ct);
		return EnvelopedResultsExtensions.AsEnvelope(response);
	}
}
