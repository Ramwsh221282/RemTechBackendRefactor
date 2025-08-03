using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Users.Module.CommonAbstractions;
using Users.Module.Features.AuthenticateUser.Exceptions;
using Users.Module.Features.AuthenticateUser.Jwt;
using Users.Module.Features.AuthenticateUser.Storage;

namespace Users.Module.Features.AuthenticateUser.Endpoint;

public static class AuthenticateUserFeatureEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapPost("auth", Handle);

    private static async Task<IResult> Handle(
        HttpContext httpContext,
        [FromServices] PgConnectionSource source,
        [FromServices] Serilog.ILogger logger,
        [FromServices] SecurityKeySource securityKey,
        [FromServices] IJsonWebTokensStorage tokensStorage,
        [FromHeader(Name = "password")] string password,
        [FromQuery] string? email = null,
        [FromQuery] string? name = null,
        CancellationToken ct = default
    )
    {
        try
        {
            IExistingUser authenticated = await new NpgAuthenticationService(
                new LoggingUsersStorage(logger, new PgExistingUsersStorage(source)),
                new UserAuthenticationPassword(password)
            )
                .Method(new ByNameUserReceivingMethod(name))
                .Method(new ByEmailUserReceivingMethod(email))
                .Authenticated(ct);
            UserJsonWebToken token = new(securityKey);
            token = authenticated.PrintToToken(token);
            await token.Save(tokensStorage, ct);
            string tokenString = token.TokenString();
            return new SignedInResult(tokenString);
        }
        catch (UnableToDetermineUserGetMethodException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UserDoesNotExistsException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (UserPasswordNotProivdedException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UserIsNotAuthenticatedException ex)
        {
            return Results.BadRequest(new { message = "Неверный пароль." });
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}

internal sealed class SignedInResult : IResult
{
    private readonly string _token;

    public SignedInResult(string token)
    {
        _token = token;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.Headers.Remove("Authorization");
        httpContext.Response.Headers.Add("Authorization", _token);
        httpContext.Response.StatusCode = 200;
        await httpContext.Response.WriteAsync("{}");
    }
}
