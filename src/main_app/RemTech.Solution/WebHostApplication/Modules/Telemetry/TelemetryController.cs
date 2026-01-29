using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using WebHostApplication.ActionFilters.Attributes;
using WebHostApplication.Queries.GetActionRecords;

namespace WebHostApplication.Modules.Telemetry;

/// <summary>
/// Контроллер для работы с телеметрией.
/// </summary>
[ApiController]
[Route("api/telemetry")]
public sealed class TelemetryController : ControllerBase
{
	/// <summary>
	/// Получить записи действий пользователя.
	/// </summary>
	/// <param name="handler">Обработчик запроса для получения записей действий пользователя.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатами запроса.</returns>
	[VerifyToken]
	[HttpGet("records")]
	public async Task<Envelope> GetRecords(
		[FromServices] IQueryHandler<GetActionRecordsQuery, GetActionRecordQueryResponse> handler,
		CancellationToken ct
	)
	{
		GetActionRecordsQuery query = GetActionRecordsQuery.Create();
		GetActionRecordQueryResponse response = await handler.Handle(query, ct);
		return EnvelopedResultsExtensions.AsEnvelope(response);
	}
}
