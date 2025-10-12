using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telemetry.Gateways.Filters;

namespace Telemetry.Gateways.Controllers;

[ApiController]
[Route("api/telemetry")]
public sealed class TelemetryTestController
{
    [HttpGet]
    [TypeFilter<TestFilter>(Arguments = ["Тестовый"])]
    public async Task<IResult> Test()
    {
        await Task.CompletedTask;
        return Results.Ok();
    }
}
