using System.Text.Json;
using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Infrastructure.Models.Queries.GetModels;

/// <summary>
/// Запрос на получение моделей транспортных средств по различным фильтрам.
/// </summary>
public class GetModelsQuery : IQuery
{
	/// <summary>
	/// Идентификатор бренда транспортного средства.
	/// </summary>
	public Guid? BrandId { get; private init; }

	/// <summary>
	/// Название бренда транспортного средства.
	/// </summary>
	public string? BrandName { get; private init; }

	/// <summary>
	/// Идентификатор категории транспортного средства.
	/// </summary>
	public Guid? CategoryId { get; private init; }

	/// <summary>
	/// Название категории транспортного средства.
	/// </summary>
	public string? CategoryName { get; private init; }

	/// <summary>
	/// Идентификатор модели транспортного средства.
	/// </summary>
	public Guid? Id { get; private init; }

	/// <summary>
	/// Название модели транспортного средства.
	/// </summary>
	public string? Name { get; private init; }

	/// <summary>
	/// Фильтр по идентификатору модели транспортного средства.
	/// </summary>
	/// <param name="id">Идентификатор модели транспортного средства.</param>
	/// <returns>Обновленный запрос с фильтром по идентификатору модели транспортного средства.</returns>
	public GetModelsQuery ForId(Guid? id)
	{
		return id == null || id == Guid.Empty ? this : Copy(this, id: id);
	}

	/// <summary>
	/// Фильтр по названию модели транспортного средства.
	/// </summary>
	/// <param name="name">Название модели транспортного средства.</param>
	/// <returns>Обновленный запрос с фильтром по названию модели транспортного средства.</returns>
	public GetModelsQuery ForName(string? name)
	{
		return string.IsNullOrWhiteSpace(name) ? this : Copy(this, name: name);
	}

	/// <summary>
	/// Фильтр по идентификатору бренда транспортного средства.
	/// </summary>
	/// <param name="brandId">Идентификатор бренда транспортного средства.</param>
	/// <returns>Обновленный запрос с фильтром по идентификатору бренда транспортного средства.</returns>
	public GetModelsQuery ForBrandId(Guid? brandId)
	{
		return brandId == null || brandId == Guid.Empty ? this : Copy(this, brandId: brandId);
	}

	/// <summary>
	/// Фильтр по названию бренда транспортного средства.
	/// </summary>
	/// <param name="brandName">Название бренда транспортного средства.</param>
	/// <returns>Обновленный запрос с фильтром по названию бренда транспортного средства.</returns>
	public GetModelsQuery ForBrandName(string? brandName)
	{
		return string.IsNullOrWhiteSpace(brandName) ? this : Copy(this, brandName: brandName);
	}

	/// <summary>
	/// Фильтр по идентификатору категории транспортного средства.
	/// </summary>
	/// <param name="categoryId">Идентификатор категории транспортного средства.</param>
	/// <returns>Обновленный запрос с фильтром по идентификатору категории транспортного средства.</returns>
	public GetModelsQuery ForCategoryId(Guid? categoryId)
	{
		return categoryId == null || categoryId == Guid.Empty ? this : Copy(this, categoryId: categoryId);
	}

	/// <summary>
	/// Фильтр по названию категории транспортного средства.
	/// </summary>
	/// <param name="categoryName" >Название категории транспортного средства.</param>
	/// <returns>Обновленный запрос с фильтром по названию категории транспортного средства.</returns>
	public GetModelsQuery ForCategoryName(string? categoryName)
	{
		return string.IsNullOrWhiteSpace(categoryName) ? this : Copy(this, categoryName: categoryName);
	}

	/// <summary>
	/// Преобразует запрос в строковое представление в формате JSON.
	/// </summary>
	/// <returns>Строковое представление запроса в формате JSON.</returns>
	public override string ToString()
	{
		return JsonSerializer.Serialize(this);
	}

	private static GetModelsQuery Copy(
		GetModelsQuery origin,
		Guid? brandId = null,
		string? brandName = null,
		Guid? categoryId = null,
		string? categoryName = null,
		Guid? id = null,
		string? name = null
	)
	{
		return new()
		{
			BrandId = brandId ?? origin.BrandId,
			BrandName = brandName ?? origin.BrandName,
			CategoryId = categoryId ?? origin.CategoryId,
			CategoryName = categoryName ?? origin.CategoryName,
			Id = id ?? origin.Id,
			Name = name ?? origin.Name,
		};
	}
}
