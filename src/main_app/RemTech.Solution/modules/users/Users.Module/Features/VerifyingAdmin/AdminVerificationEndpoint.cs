using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using StackExchange.Redis;
using Users.Module.CommonAbstractions;
using Users.Module.Models;

namespace Users.Module.Features.VerifyingAdmin;

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
            bool verified = jwt.IsOfRole("ADMIN") || jwt.IsOfRole("ROOT");
            if (verified)
            {
                logger.Information("Root or admin verification {Id} {Status}", tokenId, verified);
            }
            else
            {
                logger.Warning("Root or admin verification {Id} {Status}", tokenId, verified);
            }
            return verified ? jwt.AsResult() : Results.StatusCode(403);
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
