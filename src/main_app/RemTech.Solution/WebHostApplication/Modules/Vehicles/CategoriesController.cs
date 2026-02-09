using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using Vehicles.Infrastructure.Categories.Queries.GetCategories;
using WebHostApplication.Common.Envelope;

namespace WebHostApplication.Modules.Vehicles;

/// <summary>
/// Контроллер для работы с категориями транспортных средств.
/// </summary>
[ApiController]
[Route("api/categories")]
public sealed class CategoriesController
{
	/// <summary>
	/// Получить список категорий по фильтрам.
	/// </summary>
	/// <param name="id">Идентификатор категории</param>
	/// <param name="name">Название категории</param>
	/// <param name="brandId">Идентификатор бренда</param>
	/// <param name="brandName">Название бренда</param>
	/// <param name="modelId">Идентификатор модели</param>
	/// <param name="modelName">Название модели</param>
	/// <param name="includedInformation">Включаемая информация</param>
	/// <param name="textSearch">Текстовый поиск</param>
	/// <param name="page">Номер страницы</param>
	/// <param name="pageSize">Размер страницы</param>
	/// <param name="orderByFields">Поля сортировки</param>
	/// <param name="orderByMode">Режим сортировки</param>
	/// <param name="handler">Обработчик запроса</param>
	/// <param name="ct">Токен отмены</param>
	/// <returns>Список категорий</returns>
	[HttpGet]
	public async Task<Envelope> GetCategories(
		[FromQuery(Name = "id")] Guid? id,
		[FromQuery(Name = "name")] string? name,
		[FromQuery(Name = "brandId")] Guid? brandId,
		[FromQuery(Name = "brandName")] string? brandName,
		[FromQuery(Name = "modelId")] Guid? modelId,
		[FromQuery(Name = "modelName")] string? modelName,
		[FromQuery(Name = "include")] IEnumerable<string>? includedInformation,
		[FromQuery(Name = "text-search")] string? textSearch,
		[FromQuery(Name = "page")] int? page,
		[FromQuery(Name = "pageSize")] int? pageSize,
		[FromQuery(Name = "sort-fields")] IEnumerable<string>? orderByFields,
		[FromQuery(Name = "sort-mode")] string? orderByMode,
		[FromServices] IQueryHandler<GetCategoriesQuery, IEnumerable<CategoryResponse>> handler,
		CancellationToken ct = default
	)
	{
		GetCategoriesQuery query = new GetCategoriesQuery()
			.ForBrandId(brandId)
			.ForBrandName(brandName)
			.ForModelId(modelId)
			.ForModelName(modelName)
			.ForId(id)
			.ForName(name)
			.WithIncludedInformation(includedInformation)
			.WithPage(page)
			.WithPageSize(pageSize)
			.WithTextSearch(textSearch)
			.WithOrderByFields(orderByFields)
			.WithOrderMode(orderByMode);

		IEnumerable<CategoryResponse> response = await handler.Handle(query, ct);
		return EnvelopeFactory.Ok(response);
	}
}
