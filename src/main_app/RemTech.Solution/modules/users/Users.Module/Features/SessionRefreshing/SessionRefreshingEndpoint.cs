using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using StackExchange.Redis;
using Users.Module.CommonAbstractions;
using Users.Module.Models;

namespace Users.Module.Features.SessionRefreshing;

public class SessionRefreshingEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("refresh-session", Handle);

    private static async Task<IResult> Handle(
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromServices] Serilog.ILogger logger,
        [FromServices] SecurityKeySource key,
        [FromHeader(Name = "RemTechAccessTokenId")] string tokenId,
        CancellationToken ct
    )
    {
        try
        {
            if (!Guid.TryParse(tokenId, out Guid guid))
                return Results.Unauthorized();
            UserJwt jwt = new UserJwt(guid);
            jwt = await jwt.Provide(multiplexer);
            jwt = jwt.Recreate(key);
            logger.Information("Token {id} recreated.", guid);
            return jwt.AsResult();
        }
        catch (UnableToGetUserJwtValueException)
        {
            return Results.Unauthorized();
        }
        catch (UserJwtTokenComparisonDifferentException)
        {
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(SessionRefreshingEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}
