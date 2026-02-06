using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

namespace WebHostApplication.Modules.vehicles;

/// <summary>
/// Контроллер для работы с транспортными средствами.
/// </summary>
[ApiController]
[Route("api/vehicles")]
public sealed class VehiclesController
{
	/// <summary>
	/// Получить список транспортных средств с фильтрацией и пагинацией.
	/// </summary>
	/// <param name="brandId">Идентификатор бренда</param>
	/// <param name="categoryId">Идентификатор категории</param>
	/// <param name="regionId">Идентификатор региона</param>
	/// <param name="modelId">Идентификатор модели</param>
	/// <param name="isNds">Признак НДС</param>
	/// <param name="minimalPrice">Минимальная цена</param>
	/// <param name="maximalPrice">Максимальная цена</param>
	/// <param name="sort">Поле сортировки</param>
	/// <param name="sortFields">Поля сортировки</param>
	/// <param name="page">Номер страницы</param>
	/// <param name="pageSize">Размер страницы</param>
	/// <param name="textSearch">Текстовый поиск</param>
	/// <param name="characteristics">Характеристики</param>
	/// <param name="handler">Обработчик запроса</param>
	/// <param name="ct">Токен отмены</param>
	/// <returns>Список транспортных средств</returns>
	[HttpGet]
	public async Task<Envelope> GetVehicles(
		[FromQuery(Name = "brand")] Guid? brandId,
		[FromQuery(Name = "category")] Guid? categoryId,
		[FromQuery(Name = "region")] Guid? regionId,
		[FromQuery(Name = "model")] Guid? modelId,
		[FromQuery(Name = "nds")] bool? isNds,
		[FromQuery(Name = "price-min")] long? minimalPrice,
		[FromQuery(Name = "price-max")] long? maximalPrice,
		[FromQuery(Name = "sort")] string? sort,
		[FromQuery(Name = "sort-fields")] IEnumerable<string>? sortFields,
		[FromQuery(Name = "page")] int page,
		[FromQuery(Name = "page-size")] int pageSize,
		[FromQuery(Name = "text-search")] string? textSearch,
		[FromQuery(Name = "characteristics")] IEnumerable<CharacteristicQueryParameter>? characteristics,
		[FromServices] IQueryHandler<GetVehiclesQuery, GetVehiclesQueryResponse> handler,
		CancellationToken ct
	)
	{
		GetVehiclesQuery query = new(
			new GetVehiclesQueryParameters()
				.ForBrand(brandId)
				.ForCategory(categoryId)
				.ForRegion(regionId)
				.ForModel(modelId)
				.ForNds(isNds)
				.ForMinimalPrice(minimalPrice)
				.ForMaximalPrice(maximalPrice)
				.ForSort(sort)
				.ForSortFields(sortFields)
				.ForPage(page)
				.ForPageSize(pageSize)
				.ForTextSearch(textSearch)
				.ForCharacteristics(characteristics, c => (c.Id, c.Value))
		);

		GetVehiclesQueryResponse result = await handler.Handle(query, ct);
		return EnvelopedResultsExtensions.AsEnvelope(result);
	}
}
