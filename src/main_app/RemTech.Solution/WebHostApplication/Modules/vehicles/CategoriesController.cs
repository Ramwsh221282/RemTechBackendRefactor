using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using Vehicles.Infrastructure.Categories.Queries.GetCategories;
using Vehicles.Infrastructure.Categories.Queries.GetCategory;
using WebHostApplication.Common.Envelope;

namespace WebHostApplication.Modules.vehicles;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController
{
    [HttpGet]
    public async Task<Envelope> GetCategory(
        [FromQuery(Name = "id")] Guid? categoryId,
        [FromQuery(Name = "name")] string? categoryName,
        [FromQuery(Name = "brandId")] Guid? brandId,
        [FromQuery(Name = "brandName")] string? brandName,
        [FromQuery(Name = "modelId")] Guid? modelId,
        [FromQuery(Name = "modelName")] string? modelName,
        [FromServices] IQueryHandler<GetCategoryQuery, CategoryResponse?> handler,
        CancellationToken ct = default
    )
    {
        GetCategoryQuery query = new GetCategoryQuery()
            .ForId(categoryId)
            .ForName(categoryName)
            .ForBrandId(brandId)
            .ForBrandName(brandName)
            .ForModelId(modelId)
            .ForModelName(modelName);

        CategoryResponse? response = await handler.Handle(query, ct);
        return EnvelopeFactory.NotFoundOrOk(response, "Категория не найдена");
    }

    [HttpGet("all")]
    public async Task<Envelope> GetCategories(
        [FromServices] IQueryHandler<GetCategoriesQuery, IEnumerable<CategoryResponse>> handler,
        CancellationToken ct = default
    )
    {
        GetCategoriesQuery query = new();
        IEnumerable<CategoryResponse> response = await handler.Handle(query, ct);
        return EnvelopeFactory.Ok(response);
    }
}
