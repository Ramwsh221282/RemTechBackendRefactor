using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using Spares.Infrastructure.Queries.GetSpares;
using Spares.Infrastructure.Queries.GetSparesLocations;
using Spares.Infrastructure.Queries.GetSpareTypes;

namespace WebHostApplication.Modules.spares;

/// <summary>
/// Контроллер для работы с запчастями.
/// </summary>
[ApiController]
[Route("api/spares")]
public sealed class SparesController
{
	/// <summary>
	/// Получить список запчастей по фильтрам.
	/// </summary>
	/// <param name="regionId">Идентификатор региона.</param>
	/// <param name="priceMin">Минимальная цена.</param>
	/// <param name="priceMax">Максимальная цена.</param>
	/// <param name="textSearch">Текст для поиска.</param>
	/// <param name="page">Номер страницы.</param>
	/// <param name="pageSize">Размер страницы.</param>
	/// <param name="sortMode">Режим сортировки.</param>
	/// <param name="oem">OEM код.</param>
	/// <param name="type">Тип запчасти.</param>
	/// <param name="handler">Обработчик запроса.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[HttpGet]
	public async Task<Envelope> GetSpares(
		[FromQuery(Name = "region-id")] Guid? regionId,
		[FromQuery(Name = "price-min")] long? priceMin,
		[FromQuery(Name = "price-max")] long? priceMax,
		[FromQuery(Name = "text-search")] string? textSearch,
		[FromQuery(Name = "page")] int? page,
		[FromQuery(Name = "page-size")] int? pageSize,
		[FromQuery(Name = "sort-mode")] string? sortMode,
		[FromQuery(Name = "oem")] string? oem,
		[FromQuery(Name = "type")] string? type,
		[FromServices] IQueryHandler<GetSparesQuery, GetSparesQueryResponse> handler,
		CancellationToken ct
	)
	{
		GetSparesQuery query = new GetSparesQuery()
			.ForRegion(regionId)
			.ForOem(oem)
			.WithMinimalPrice(priceMin)
			.WithMaximalPrice(priceMax)
			.WithTextSearch(textSearch)
			.WithPage(page)
			.WithPageSize(pageSize)
			.WithOrderMode(sortMode)
			.ForType(type);
		GetSparesQueryResponse result = await handler.Handle(query, ct);
		return EnvelopedResultsExtensions.AsEnvelope(result);
	}

	/// <summary>
	/// Получить список локаций запчастей по фильтрам.
	/// </summary>
	/// <param name="textSearch">Текст для поиска.</param>
	/// <param name="handler">Обработчик запроса.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[HttpGet("locations")]
	public async Task<Envelope> GetSparesLocations(
		[FromQuery(Name = "text-search")] string? textSearch,
		[FromServices] IQueryHandler<GetSparesLocationsQuery, IEnumerable<SpareLocationResponse>> handler,
		CancellationToken ct
	)
	{
		GetSparesLocationsQuery query = GetSparesLocationsQuery.Create().WithTextSearch(textSearch);
		IEnumerable<SpareLocationResponse> result = await handler.Handle(query, ct);
		return EnvelopedResultsExtensions.AsEnvelope(result);
	}

	/// <summary>
	/// Получить список типов запчастей.
	/// </summary>
	/// <param name="textSearch">Текст для поиска.</param>
	/// <param name="amount">Количество типов запчастей для получения.</param>
	/// <param name="handler">Обработчик запроса.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[HttpGet("types")]
	public async Task<Envelope> GetSpareTypes(
		[FromQuery(Name = "text-search")] string? textSearch,
		[FromQuery(Name = "amount")] int? amount,
		[FromServices] IQueryHandler<GetSpareTypesQuery, IEnumerable<SpareTypeResponse>> handler,
		CancellationToken ct
	)
	{
		GetSpareTypesQuery query = GetSpareTypesQuery.Create().WithTextSearch(textSearch).WithAmount(amount);
		IEnumerable<SpareTypeResponse> result = await handler.Handle(query, ct);
		return EnvelopedResultsExtensions.AsEnvelope(result);
	}
}
