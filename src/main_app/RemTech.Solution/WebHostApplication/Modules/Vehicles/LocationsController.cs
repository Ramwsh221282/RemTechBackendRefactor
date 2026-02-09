using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using Vehicles.Infrastructure.Locations.Queries;

namespace WebHostApplication.Modules.Vehicles;

/// <summary>
/// Контроллер для работы с локациями транспортных средств.
/// </summary>
[ApiController]
[Route("api/locations")]
public sealed class LocationsController
{
	/// <summary>
	/// Получить список локаций по фильтрам.
	/// </summary>
	/// <param name="textSearch">Текстовый поиск</param>
	/// <param name="id">Идентификатор локации</param>
	/// <param name="categoryId">Идентификатор категории</param>
	/// <param name="brandId">Идентификатор бренда</param>
	/// <param name="modelId">Идентификатор модели</param>
	/// <param name="categoryName">Название категории</param>
	/// <param name="brandName">Название бренда</param>
	/// <param name="modelName">Название модели</param>
	/// <param name="amount">Количество</param>
	/// <param name="orderByName">Сортировка по имени</param>
	/// <param name="includes">Включаемые поля</param>
	/// <param name="handler">Обработчик запроса</param>
	/// <param name="ct">Токен отмены</param>
	/// <returns>Список локаций</returns>
	[HttpGet]
	public async Task<Envelope> GetLocations(
		[FromQuery(Name = "text-search")] string? textSearch,
		[FromQuery(Name = "id")] Guid? id,
		[FromQuery(Name = "category-id")] Guid? categoryId,
		[FromQuery(Name = "brand-id")] Guid? brandId,
		[FromQuery(Name = "model-id")] Guid? modelId,
		[FromQuery(Name = "category-name")] string? categoryName,
		[FromQuery(Name = "brand-name")] string? brandName,
		[FromQuery(Name = "model-name")] string? modelName,
		[FromQuery(Name = "amount")] int? amount,
		[FromQuery(Name = "order-by-name")] bool? orderByName,
		[FromQuery(Name = "include")] IEnumerable<string>? includes,
		[FromServices] IQueryHandler<GetLocationsQuery, IEnumerable<LocationsResponse>> handler,
		CancellationToken ct = default
	)
	{
		GetLocationsQuery query = GetLocationsQuery
			.Create()
			.WithTextSearch(textSearch)
			.WithId(id)
			.WithCategoryId(categoryId)
			.WithBrandId(brandId)
			.WithModelId(modelId)
			.WithCategoryName(categoryName)
			.WithBrandName(brandName)
			.WithModelName(modelName)
			.WithIncludes(includes)
			.WithAmount(amount)
			.WithOrderByName(orderByName);

		IEnumerable<LocationsResponse> result = await handler.Handle(query, ct);
		return EnvelopedResultsExtensions.AsEnvelope(result);
	}
}
