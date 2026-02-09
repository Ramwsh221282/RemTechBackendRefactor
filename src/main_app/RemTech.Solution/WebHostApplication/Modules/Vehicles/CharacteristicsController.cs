using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using Vehicles.Infrastructure.Vehicles.Queries.GetVehicleCharacteristics;

namespace WebHostApplication.Modules.Vehicles;

/// <summary>
/// Контроллер для работы с характеристиками транспортных средств.
/// </summary>
[ApiController]
[Route("api/characteristics")]
public sealed class CharacteristicsController
{
	/// <summary>
	/// Получить характеристики транспортных средств по фильтрам.
	/// </summary>
	/// <param name="brandId">Идентификатор бренда</param>
	/// <param name="categoryId">Идентификатор категории</param>
	/// <param name="modelId">Идентификатор модели</param>
	/// <param name="handler">Обработчик запроса</param>
	/// <param name="ct">Токен отмены</param>
	/// <returns>Характеристики транспортных средств</returns>
	[HttpGet]
	public async Task<Envelope> GetVehicleCharacteristics(
		[FromQuery(Name = "brand")] Guid? brandId,
		[FromQuery(Name = "category")] Guid? categoryId,
		[FromQuery(Name = "model")] Guid? modelId,
		[FromServices] IQueryHandler<GetVehicleCharacteristicsQuery, GetVehicleCharacteristicsQueryResponse> handler,
		CancellationToken ct
	)
	{
		GetVehicleCharacteristicsQuery query = new GetVehicleCharacteristicsQuery()
			.ForBrand(brandId)
			.ForCategory(categoryId)
			.ForModel(modelId);
		GetVehicleCharacteristicsQueryResponse result = await handler.Handle(query, ct);
		return EnvelopedResultsExtensions.AsEnvelope(result);
	}
}
