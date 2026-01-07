using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using Vehicles.Infrastructure.Vehicles.Queries.GetVehicleCharacteristics;

namespace WebHostApplication.Modules.vehicles;

[ApiController]
[Route("api/characteristics")]
public sealed class CharacteristicsController
{
    [HttpGet]
    public async Task<Envelope> GetVehicleCharacteristics(
        [FromQuery(Name = "brand")] Guid? brandId,
        [FromQuery(Name = "category")] Guid? categoryId,
        [FromQuery(Name = "model")] Guid? modelId,
        [FromServices] IQueryHandler<GetVehicleCharacteristicsQuery, GetVehicleCharacteristicsQueryResponse> handler,
        CancellationToken ct)
    {
        GetVehicleCharacteristicsQuery query = new GetVehicleCharacteristicsQuery()
            .ForBrand(brandId)
            .ForCategory(categoryId)
            .ForModel(modelId);
        GetVehicleCharacteristicsQueryResponse result = await handler.Handle(query, ct);
        return EnvelopedResultsExtensions.AsEnvelope(result);
    }
}