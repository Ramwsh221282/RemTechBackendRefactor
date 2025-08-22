using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using StackExchange.Redis;
using Users.Module.CommonAbstractions;
using Users.Module.Models;

namespace Users.Module.Features.VerifyingToken;

public static class VerifyTokenEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("verify", Handle);

    private static async Task<IResult> Handle(
        [FromServices] SecurityKeySource securityKey,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromServices] Serilog.ILogger logger,
        [FromHeader(Name = "RemTechAccessTokenId")] string tokenId,
        CancellationToken ct
    )
    {
        try
        {
            if (!Guid.TryParse(tokenId, out Guid guid))
                return Results.Ok(false);
            UserJwt jwt = new UserJwt(guid);
            jwt = await jwt.Provide(multiplexer);
            jwt = await jwt.CheckSubscription(securityKey, multiplexer);
            logger.Information("Token session verified {Id}", guid);
            return Results.Ok(jwt.AsResult());
        }
        catch (Exception)
        {
            return Results.Ok(false);
        }
    }
}
