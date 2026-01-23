using System.Text.Json;
using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Infrastructure.Models.Queries.GetModels;

public class GetModelsQuery : IQuery
{
	public Guid? BrandId { get; private init; }
	public string? BrandName { get; private init; }
	public Guid? CategoryId { get; private init; }
	public string? CategoryName { get; private init; }
	public Guid? Id { get; private init; }
	public string? Name { get; private init; }

	public GetModelsQuery ForId(Guid? id) => id == null || id == Guid.Empty ? this : Copy(this, id: id);

	public GetModelsQuery ForName(string? name) => string.IsNullOrWhiteSpace(name) ? this : Copy(this, name: name);

	public GetModelsQuery ForBrandId(Guid? brandId) =>
		brandId == null || brandId == Guid.Empty ? this : Copy(this, brandId: brandId);

	public GetModelsQuery ForBrandName(string? brandName) =>
		string.IsNullOrWhiteSpace(brandName) ? this : Copy(this, brandName: brandName);

	public GetModelsQuery ForCategoryId(Guid? categoryId) =>
		categoryId == null || categoryId == Guid.Empty ? this : Copy(this, categoryId: categoryId);

	public GetModelsQuery ForCategoryName(string? categoryName) =>
		string.IsNullOrWhiteSpace(categoryName) ? this : Copy(this, categoryName: categoryName);

	private static GetModelsQuery Copy(
		GetModelsQuery origin,
		Guid? brandId = null,
		string? brandName = null,
		Guid? categoryId = null,
		string? categoryName = null,
		Guid? id = null,
		string? name = null
	) =>
		new()
		{
			BrandId = brandId ?? origin.BrandId,
			BrandName = brandName ?? origin.BrandName,
			CategoryId = categoryId ?? origin.CategoryId,
			CategoryName = categoryName ?? origin.CategoryName,
			Id = id ?? origin.Id,
			Name = name ?? origin.Name,
		};

	public override string ToString() => JsonSerializer.Serialize(this);
}
