using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Infrastructure.Module.Postgres;
using Shared.Infrastructure.Module.Redis;
using Users.Module.CommonAbstractions;
using Users.Module.Models;

namespace Users.Module.Features.AuthenticatingUserAccount;

public static class UserAuthenticationEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapPost("sign-in", Handle);

    private static async Task<IResult> Handle(
        HttpContext httpContext,
        [FromServices] PostgresDatabase dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromServices] SecurityKeySource securityKey,
        [FromServices] RedisCache cache,
        [FromHeader(Name = "password")] string password,
        [FromQuery] string? email = null,
        [FromQuery] string? name = null,
        CancellationToken ct = default
    )
    {
        User user = new(name, password, email);

        try
        {
            IUserAuthentication authentication = user.RequireAuthentication();
            AuthenticatedUser authenticated = await authentication.Authenticate(dataSource, ct);

            UserJwtSource jwtSource = authenticated.AsJwtSource();
            UserJwt jwt = jwtSource.Provide(securityKey);

            await jwt.StoreInCache(cache);
            return jwt.AsResult();
        }
        catch (AuthenticationEmailNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (AuthenticationPasswordFailedException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (AuthenticationPasswordNotProvidedException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (AuthenticationUserNameNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (UnableToResolveAuthenticationMethodException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(UserAuthenticationEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}
