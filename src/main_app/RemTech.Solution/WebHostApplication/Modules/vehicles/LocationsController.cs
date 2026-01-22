using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using Vehicles.Infrastructure.Locations.Queries;

namespace WebHostApplication.Modules.vehicles;

[ApiController]
[Route("api/locations")]
public sealed class LocationsController
{
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
            .WithAmount(amount);

        IEnumerable<LocationsResponse> result = await handler.Handle(query, ct);
        return EnvelopedResultsExtensions.AsEnvelope(result);
    }
}
