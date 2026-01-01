using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

namespace Vehicles.WebApi.Controllers;

[ApiController]
[Route("api/vehicles")]
public sealed class VehiclesController
{
    [HttpGet]
    public async Task<Envelope> GetVehicles(
        [FromQuery(Name = "brand")] Guid? brandId,
        [FromQuery(Name = "category")] Guid? categoryId,
        [FromQuery(Name = "region")] Guid? regionId,
        [FromQuery(Name = "model")] Guid? modelId,
        [FromQuery(Name = "nds")] bool? isNds,
        [FromQuery(Name = "price-min")] long? minimalPrice,
        [FromQuery(Name = "price-max")] long? maximalPrice,
        [FromQuery(Name = "sort")] string? sort,
        [FromQuery(Name = "sort-fields")] IEnumerable<string>? sortFields,
        [FromQuery(Name = "page")] int page,
        [FromQuery(Name = "page-size")] int pageSize,
        [FromQuery(Name = "text-search")] string? textSearch,
        [FromServices] IQueryHandler<GetVehiclesQuery, GetVehiclesQueryResponse> handler,
        CancellationToken ct
        )
    {
        GetVehiclesQuery query = new(new GetVehiclesQueryParameters()
            .ForBrand(brandId)
            .ForCategory(categoryId)
            .ForRegion(regionId)
            .ForModel(modelId)
            .ForNds(isNds)
            .ForMinimalPrice(minimalPrice)
            .ForMaximalPrice(maximalPrice)
            .ForSort(sort)
            .ForSortFields(sortFields)
            .ForPage(page)
            .ForPageSize(pageSize)
            .ForTextSearch(textSearch));
        GetVehiclesQueryResponse result = await handler.Handle(query, ct);
        return EnvelopedResultsExtensions.AsEnvelope(result);
    }
}