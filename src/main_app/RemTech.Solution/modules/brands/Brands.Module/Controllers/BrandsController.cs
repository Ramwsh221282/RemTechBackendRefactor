using System.Net;
using Brands.Module.Features.QueryBrands;
using Brands.Module.Features.QueryPopularBrands;
using Brands.Module.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RemTech.Core.Shared.Cqrs;
using Shared.WebApi;

namespace Brands.Module.Controllers;

[ApiController]
[Route("api/brands")]
public sealed class BrandsController : ControllerBase
{
    [HttpGet]
    private async Task<IResult> GetBrands(
        [FromServices] ICommandHandler<QueryBrandsCommand, QueryBrandsResponse> handler,
        [FromQuery(Name = "name")] string? name,
        [FromQuery(Name = "textSearch")] string? textSearch,
        [FromQuery(Name = "orderMode")] string? orderMode,
        [FromQuery(Name = "page")] int? page,
        [FromQuery(Name = "pageSize")] int? pageSize,
        CancellationToken ct
    )
    {
        var command = new QueryBrandsCommand(name, textSearch, orderMode, page, pageSize);
        var response = await handler.Handle(command, ct);
        return new HttpEnvelope<QueryBrandsResponse>(response, HttpStatusCode.OK);
    }

    [HttpGet("popular")]
    private async Task<IResult> Handle(
        [FromServices] ICommandHandler<QueryPopularBrandsCommand, IEnumerable<BrandDto>> handler,
        CancellationToken ct
    )
    {
        QueryPopularBrandsCommand command = new();
        var result = await handler.Handle(command, ct);
        return new HttpEnvelope<IEnumerable<BrandDto>>(result, HttpStatusCode.OK);
    }
}
