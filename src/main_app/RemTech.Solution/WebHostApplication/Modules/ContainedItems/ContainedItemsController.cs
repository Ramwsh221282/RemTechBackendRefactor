using ContainedItems.Infrastructure.Queries.GetMainPageItemStats;
using ContainedItems.Infrastructure.Queries.GetMainPageLastAddedItems;
using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;

namespace WebHostApplication.Modules.ContainedItems;

/// <summary>
/// Контроллер для работы с элементами, содержащимися на главной странице.
/// </summary>
[ApiController]
[Route("api/contained-items")]
public sealed class ContainedItemsController
{
	/// <summary>
	/// Получить статистику для главной страницы.
	/// </summary>
	/// <param name="handler">Обработчик запроса для получения статистики.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом запроса.</returns>
	[HttpGet("main-page/statistics")]
	public async Task<Envelope> GetStatistics(
		[FromServices] IQueryHandler<GetMainPageItemStatsQuery, MainPageItemStatsResponse> handler,
		CancellationToken ct = default
	)
	{
		GetMainPageItemStatsQuery query = new();
		MainPageItemStatsResponse response = await handler.Handle(query, ct);
		return EnvelopedResultsExtensions.AsEnvelope(response);
	}

	/// <summary>
	/// Получить последние добавленные элементы для главной страницы.
	/// </summary>
	/// <param name="handler">Обработчик запроса для получения последних добавленных элементов. </param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом запроса.</returns>
	[HttpGet("main-page/last-added")]
	public async Task<Envelope> GetLastAddedItems(
		[FromServices] IQueryHandler<GetMainPageLastAddedItemsQuery, MainPageLastAddedItemsResponse> handler,
		CancellationToken ct = default
	)
	{
		GetMainPageLastAddedItemsQuery query = new();
		MainPageLastAddedItemsResponse response = await handler.Handle(query, ct);
		return EnvelopedResultsExtensions.AsEnvelope(response);
	}
}
