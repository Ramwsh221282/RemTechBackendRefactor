using Mailing.Moduled.Bus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using StackExchange.Redis;
using Users.Module.CommonAbstractions;
using Users.Module.Features.CreateEmailConfirmation;
using Users.Module.Features.VerifyingAdmin;
using Users.Module.Models;

namespace Users.Module.Features.UpdateUserPassword;

public static class UpdateUserPasswordEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapPatch("password", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] MailingBusPublisher publisher,
        [FromServices] StringHash hash,
        [FromServices] Serilog.ILogger logger,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromHeader(Name = "RemTechAccessTokenId")]
        string tokenId,
        [FromHeader(Name = "Password")] string password,
        [FromHeader(Name = "NewPassword")] string newPassword,
        CancellationToken ct
    )
    {
        try
        {
            if (!Guid.TryParse(tokenId, out Guid tokenGuid))
                return Results.BadRequest(
                    new { message = "Ошибка авторизации. Попробуйте снова авторизоваться." }
                );
            UserJwt jwt = new(tokenGuid);
            jwt = await jwt.Provide(multiplexer);
            UserJwtOutput jwtOutput = jwt.MakeOutput();
            UpdateUserPasswordCommand command = new(password, newPassword, jwtOutput.UserId);
            UpdateUserPasswordHandler handler = new UpdateUserPasswordHandler(
                dataSource,
                publisher,
                hash
            );
            await handler.Handle(command, ct);
            return Results.Ok();
        }
        catch (UnableToGetUserJwtValueException)
        {
            return Results.BadRequest(
                new { message = "Ошибка авторизации. Попробуйте снова авторизоваться" }
            );
        }
        catch (UserJwtTokenComparisonDifferentException)
        {
            return Results.BadRequest(
                new { message = "Ошибка авторизации. Попробуйте снова авторизоваться" }
            );
        }
        catch (UserNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (PasswordInvalidException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance} {Ex}", nameof(UpdateUserPasswordEndpoint), ex.Message);
            return Results.BadRequest(new { message = "Ошибка на стороне приложения" });
        }
    }
}