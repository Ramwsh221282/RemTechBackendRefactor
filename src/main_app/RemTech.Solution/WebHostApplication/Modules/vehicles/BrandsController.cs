using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using Vehicles.Infrastructure.Brands.Queries.GetBrand;
using Vehicles.Infrastructure.Brands.Queries.GetBrandByCategory;
using WebHostApplication.Common.Envelope;

namespace WebHostApplication.Modules.vehicles;

[ApiController]
[Route("api/brands")]
public sealed class BrandsController
{
    [HttpGet]
    public async Task<Envelope> GetBrand(
        [FromQuery(Name = "categoryId")] Guid? categoryId,
        [FromQuery(Name = "categoryName")] string? categoryName,
        [FromQuery(Name = "brandId")] Guid? brandId,
        [FromQuery(Name = "brandName")] string? brandName,
        [FromQuery(Name = "modelId")] Guid? modelId,
        [FromQuery(Name = "modelName")] string? modelName,
        [FromServices] IQueryHandler<GetBrandQuery, BrandResponse?> handler,
        CancellationToken ct = default
    )
    {
        GetBrandQuery query = new GetBrandQuery()
            .ForCategoryId(categoryId)
            .ForCategoryName(categoryName)
            .ForBrandId(brandId)
            .ForBrandName(brandName)
            .ForModelId(modelId)
            .ForModelName(modelName);

        BrandResponse? response = await handler.Handle(query, ct);
        return EnvelopeFactory.NotFoundOrOk(response, "Бренд не найден");
    }
}
