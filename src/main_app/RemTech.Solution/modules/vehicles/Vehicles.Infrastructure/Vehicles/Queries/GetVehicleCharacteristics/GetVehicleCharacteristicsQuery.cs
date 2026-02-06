using System.Text.Json;
using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicleCharacteristics;

/// <summary>
/// Запрос на получение характеристик транспортных средств по фильтрам.
/// </summary>
public sealed class GetVehicleCharacteristicsQuery : IQuery
{
	/// <summary>
	/// Идентификатор бренда транспортного средства.
	/// </summary>
	public Guid? BrandId { get; private set; }

	/// <summary>
	/// Идентификатор категории транспортного средства.
	/// </summary>
	public Guid? CategoryId { get; private set; }

	/// <summary>
	/// Идентификатор модели транспортного средства.
	/// </summary>
	public Guid? ModelId { get; private set; }

	/// <summary>
	/// Фильтр по бренду.
	/// </summary>
	/// <param name="brandId">Идентификатор бренда для фильтрации.</param>
	/// <returns>Объект запроса с примененным фильтром по бренду.</returns>
	public GetVehicleCharacteristicsQuery ForBrand(Guid? brandId)
	{
		if (BrandId.HasValue)
		{
			return this;
		}

		BrandId = brandId;
		return this;
	}

	/// <summary>
	/// Фильтр по категории.
	/// </summary>
	/// <param name="categoryId">Идентификатор категории для фильтрации.</param>
	/// <returns>Объект запроса с примененным фильтром по категории.</returns>
	public GetVehicleCharacteristicsQuery ForCategory(Guid? categoryId)
	{
		if (CategoryId.HasValue)
		{
			return this;
		}

		CategoryId = categoryId;
		return this;
	}

	/// <summary>
	/// Фильтр по модели.
	/// </summary>
	/// <param name="modelId">Идентификатор модели для фильтрации.</param>
	/// <returns>Объект запроса с примененным фильтром по модели.</returns>
	public GetVehicleCharacteristicsQuery ForModel(Guid? modelId)
	{
		if (ModelId.HasValue)
		{
			return this;
		}

		ModelId = modelId;
		return this;
	}

	public override string ToString()
	{
		return JsonSerializer.Serialize(this);
	}
}
