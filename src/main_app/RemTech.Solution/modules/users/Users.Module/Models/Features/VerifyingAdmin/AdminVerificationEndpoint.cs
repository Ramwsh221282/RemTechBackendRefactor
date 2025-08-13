using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using StackExchange.Redis;
using Users.Module.CommonAbstractions;

namespace Users.Module.Models.Features.VerifyingAdmin;

public static class AdminVerificationEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("verify-admin", Handle);

    private static async Task<IResult> Handle(
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromServices] Serilog.ILogger logger,
        [FromServices] SecurityKeySource key,
        [FromHeader(Name = "RemTechAccessTokenId")] string tokenId
    )
    {
        try
        {
            UserJwt jwt = new UserJwt(Guid.Parse(tokenId));
            jwt = await jwt.Provide(multiplexer);
            jwt = await jwt.CheckSubscription(key, multiplexer);
            return jwt.IsOfRole("ADMIN") || jwt.IsOfRole("ROOT")
                ? jwt.AsResult()
                : Results.StatusCode(403);
        }
        catch (TokensExpiredException)
        {
            return Results.Unauthorized();
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
            logger.Error("{Entrance}. {Ex}.", nameof(AdminVerificationEndpoint), ex.Message);
            return Results.Unauthorized();
        }
    }
}
