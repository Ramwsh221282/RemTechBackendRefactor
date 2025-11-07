using Mailing.Moduled.Bus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Shared.Infrastructure.Module.Frontend;
using StackExchange.Redis;
using Users.Module.Features.VerifyingAdmin;
using Users.Module.Models;

namespace Users.Module.Features.CreateEmailConfirmation;

public static class CreateEmailConfirmationEndpoint
{
    public static void Map(RouteGroupBuilder builder) =>
        builder.MapPatch("email-confirmation", Handle);

    private static async Task<IResult> Handle(
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] FrontendUrl frontendUrl,
        [FromServices] MailingBusPublisher publisher,
        [FromServices] Serilog.ILogger logger,
        [FromHeader(Name = "RemTechAccessTokenId")]
        string tokenId,
        [FromHeader(Name = "Password")] string password,
        CancellationToken ct
    )
    {
        try
        {
            if (!Guid.TryParse(tokenId, out Guid tokenGuid))
                return Results.BadRequest(
                    new { message = "Ошибка авторизации. Попробуйте снова авторизоваться." }
                );
            UserJwt jwt = new UserJwt(tokenGuid);
            jwt = await jwt.Provide(multiplexer);
            UserJwtOutput output = jwt.MakeOutput();
            CreateEmailConfirmationCommand command = new(output.UserId, password);
            CreateEmailConfirmationHandler handler = new(
                multiplexer,
                dataSource,
                frontendUrl,
                publisher
            );
            Guid result = await handler.Handle(command, ct);
            logger.Information(
                "{Entrance}. Created email confirmation key {Key}.",
                nameof(CreateEmailConfirmationEndpoint),
                result
            );
            return Results.Ok(result);
        }
        catch (PasswordInvalidException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UserNotFoundException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (EmailIsAlreadyConfirmedException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
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
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(CreateEmailConfirmationEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}