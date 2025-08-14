using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using StackExchange.Redis;

namespace Users.Module.Models.Features.SignOut;

public static class SignOutEndpoint
{
    public static void Map(RouteGroupBuilder group) => group.MapGet("sign-out", Handle);

    private static async Task<IResult> Handle(
        [FromServices] Serilog.ILogger logger,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromHeader(Name = "RemTechAccessTokenId")] string tokenId
    )
    {
        try
        {
            if (!Guid.TryParse(tokenId, out var guid))
                return Results.Ok();
            UserJwt jwt = new UserJwt(guid);
            await jwt.Deleted(multiplexer);
            logger.Information("Token session removed: {Id}", guid);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            logger.Error("{Entrance}. {Ex}.", nameof(SignOutEndpoint), ex.Message);
            return Results.Ok();
        }
    }
}
