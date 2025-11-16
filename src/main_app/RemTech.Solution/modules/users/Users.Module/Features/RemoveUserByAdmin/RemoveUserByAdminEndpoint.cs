using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using StackExchange.Redis;
using Users.Module.Models;

namespace Users.Module.Features.RemoveUserByAdmin;

public static class RemoveUserByAdminEndpoint
{
    public static readonly Delegate HandleFn = Handle;

    private static async Task<IResult> Handle(
        [FromServices] Serilog.ILogger logger,
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromServices] MailingBusPublisher publisher,
        [FromQuery] Guid userId,
        [FromHeader(Name = "RemTechAccessTokenId")]
        string token,
        CancellationToken ct
    )
    {
        try
        {
            if (!Guid.TryParse(token, out Guid tokenId))
                return Results.BadRequest(
                    new { message = "Ошибка авторизации. Попробуйте авторизоваться снова." }
                );
            UserJwt jwt = new UserJwt(tokenId);
            jwt = await jwt.Provide(multiplexer);
            RemoveUserByAdminCommand command = new(jwt, userId);
            RemoveUserByAdminHandler handler = new(dataSource, publisher);
            await handler.Handle(command, ct);
            return Results.Ok();
        }
        catch (UserNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (UnableToGetUserJwtValueException)
        {
            return Results.BadRequest(
                new { message = "Ошибка авторизации. Попробуйте авторизоваться снова." }
            );
        }
        catch (ForbiddenOperationException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UserJwtTokenComparisonDifferentException)
        {
            return Results.BadRequest(
                new { message = "Ошибка авторизации. Попробуйте авторизоваться снова." }
            );
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}", nameof(RemoveUserByAdminEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}