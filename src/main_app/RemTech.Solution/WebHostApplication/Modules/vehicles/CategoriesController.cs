using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using Vehicles.Infrastructure.Categories.Queries.GetCategories;
using WebHostApplication.Common.Envelope;

namespace WebHostApplication.Modules.vehicles;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController
{
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
