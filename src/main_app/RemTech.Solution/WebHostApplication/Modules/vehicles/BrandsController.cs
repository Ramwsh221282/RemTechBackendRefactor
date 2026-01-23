using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using Vehicles.Infrastructure.Brands.Queries.GetBrands;
using WebHostApplication.Common.Envelope;

namespace WebHostApplication.Modules.vehicles;

[ApiController]
[Route("api/brands")]
public sealed class BrandsController
{
	[HttpGet()]
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
			.WithInclude(includes);

		IEnumerable<BrandResponse> response = await handler.Handle(query, ct);
		return EnvelopeFactory.Ok(response);
	}
}
