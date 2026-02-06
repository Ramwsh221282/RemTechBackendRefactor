using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using Vehicles.Infrastructure.Brands.Queries.GetBrands;
using WebHostApplication.Common.Envelope;

namespace WebHostApplication.Modules.vehicles;

/// <summary>
/// Контроллер для работы с брендами автомобилей.
/// </summary>
[ApiController]
[Route("api/brands")]
public sealed class BrandsController
{
	/// <summary>
	/// Получает список брендов автомобилей с возможностью фильтрации, сортировки и пагинации.
	/// </summary>
	/// <param name="id">Идентификатор бренда.</param>
	/// <param name="name">Название бренда.</param>
	/// <param name="categoryId">Идентификатор категории.</param>
	/// <param name="categoryName">Название категории.</param>
	/// <param name="modelId">Идентификатор модели.</param>
	/// <param name="modelName">Название модели.</param>
	/// <param name="page">Номер страницы.</param>
	/// <param name="pageSize">Размер страницы.</param>
	/// <param name="includes">Список связанных сущностей для включения.</param>
	/// <param name="textSearch">Текстовый поиск.</param>
	/// <param name="sortFields">Поля для сортировки.</param>
	/// <param name="sortMode">Режим сортировки.</param>
	/// <param name="handler">Обработчик запроса.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Объект-обёртка с результатом запроса.</returns>
	[HttpGet]
	public async Task<Envelope> GetBrands(
		[FromQuery(Name = "id")] Guid? id,
		[FromQuery(Name = "name")] string? name,
		[FromQuery(Name = "categoryId")] Guid? categoryId,
		[FromQuery(Name = "categoryName")] string? categoryName,
		[FromQuery(Name = "modelId")] Guid? modelId,
		[FromQuery(Name = "modelName")] string? modelName,
		[FromQuery(Name = "page")] int? page,
		[FromQuery(Name = "pageSize")] int? pageSize,
		[FromQuery(Name = "include")] IEnumerable<string>? includes,
		[FromQuery(Name = "text-search")] string? textSearch,
		[FromQuery(Name = "sort-fields")] IEnumerable<string>? sortFields,
		[FromQuery(Name = "sort-mode")] string? sortMode,
		[FromServices] IQueryHandler<GetBrandsQuery, IEnumerable<BrandResponse>> handler,
		CancellationToken ct = default
	)
	{
		GetBrandsQuery query = new GetBrandsQuery()
			.ForId(id)
			.ForName(name)
			.ForCategoryId(categoryId)
			.ForCategoryName(categoryName)
			.ForModelId(modelId)
			.ForModelName(modelName)
			.WithPageSize(pageSize)
			.WithPagination(page)
			.WithTextSearch(textSearch)
			.WithInclude(includes)
			.WithSortFields(sortFields)
			.WithSortDirection(sortMode);

		IEnumerable<BrandResponse> response = await handler.Handle(query, ct);
		return response.Ok();
	}
}
