using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using Vehicles.Infrastructure.Models.Queries.GetModels;
using WebHostApplication.Common.Envelope;

namespace WebHostApplication.Modules.vehicles;

/// <summary>
/// Контроллер для работы с моделями транспортных средств.
/// </summary>
[ApiController]
[Route("api/models")]
public sealed class ModelsController
{
	/// <summary>
	/// Получить список моделей по фильтрам.
	/// </summary>
	/// <param name="brandId">Идентификатор бренда</param>
	/// <param name="brandName">Название бренда</param>
	/// <param name="categoryId">Идентификатор категории</param>
	/// <param name="categoryName">Название категории</param>
	/// <param name="handler">Обработчик запроса</param>
	/// <param name="ct">Токен отмены</param>
	/// <returns>Список моделей</returns>
	[HttpGet]
	public async Task<Envelope> GetModels(
		[FromQuery(Name = "brandId")] Guid? brandId,
		[FromQuery(Name = "brandName")] string? brandName,
		[FromQuery(Name = "categoryId")] Guid? categoryId,
		[FromQuery(Name = "categoryName")] string? categoryName,
		[FromServices] IQueryHandler<GetModelsQuery, IEnumerable<ModelResponse>> handler,
		CancellationToken ct = default
	)
	{
		GetModelsQuery query = new GetModelsQuery()
			.ForBrandId(brandId)
			.ForBrandName(brandName)
			.ForCategoryId(categoryId)
			.ForCategoryName(categoryName);

		IEnumerable<ModelResponse> response = await handler.Handle(query, ct);
		return EnvelopeFactory.Ok(response);
	}
}
