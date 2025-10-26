using System.Net;
using Categories.Module.Features.QueryCategories;
using Categories.Module.Features.QueryPopularCategories;
using Categories.Module.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RemTech.Core.Shared.Cqrs;
using Shared.WebApi;

namespace Categories.Module.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController : ControllerBase
{
    [HttpGet]
    public async Task<IResult> GetCategories(
        [FromServices] ICommandHandler<QueryCategoriesCommand, QueryCategoriesResponse> handler,
        [FromQuery(Name = "name")] string? name,
        [FromQuery(Name = "textSearch")] string? textSearch,
        [FromQuery(Name = "orderMode")] string? orderMode,
        [FromQuery(Name = "page")] int? page,
        [FromQuery(Name = "pageSize")] int? pageSize,
        CancellationToken ct
    )
    {
        var command = new QueryCategoriesCommand(name, textSearch, orderMode, page, pageSize);
        var result = await handler.Handle(command, ct);
        return new HttpEnvelope<QueryCategoriesResponse>(result);
    }

    [HttpGet]
    public async Task<IResult> GetPopularCategories(
        [FromServices] ICommandHandler<PopularCategoriesCommand, IEnumerable<CategoryDto>> handler,
        CancellationToken ct
    )
    {
        var command = new PopularCategoriesCommand();
        var result = await handler.Handle(command, ct);
        return new HttpEnvelope<IEnumerable<CategoryDto>>(result, HttpStatusCode.OK);
    }
}
