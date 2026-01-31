using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using WebHostApplication.ActionFilters.Attributes;
using WebHostApplication.Queries.GetActionRecords;
using WebHostApplication.Queries.Responses;

namespace WebHostApplication.Modules.Telemetry;

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
		[FromServices] IQueryHandler<GetActionRecordsQuery, ActionRecordsPageResponse> handler,
		CancellationToken ct
	)
	{
		GetActionRecordsQuery query = GetActionRecordsQuery
			.Create()
			.WithCustomPage(page)
			.WithCustomPageSize(pageSize)
			.WithPermissionIdentifiers(permissions);
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
