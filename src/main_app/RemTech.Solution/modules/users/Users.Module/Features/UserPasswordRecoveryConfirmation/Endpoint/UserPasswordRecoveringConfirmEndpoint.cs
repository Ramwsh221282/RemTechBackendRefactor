using Mailing.Module.Bus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using StackExchange.Redis;
using Users.Module.CommonAbstractions;
using Users.Module.Features.CreateEmailConfirmation;
using Users.Module.Features.UserPasswordRecovering.Infrastructure;
using Users.Module.Features.UserPasswordRecoveryConfirmation.Exceptions;
using Users.Module.Features.UserPasswordRecoveryConfirmation.Interactor;

namespace Users.Module.Features.UserPasswordRecoveryConfirmation.Endpoint;

public static class UserPasswordRecoveryConfirmationEndpoint
{
    public static void Map(RouteGroupBuilder builder) =>
        builder.MapGet("password-reset-confirm", Handle);

    private static async Task<IResult> Handle(
        [FromServices] MailingBusPublisher publisher,
        [FromServices] StringHash hash,
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] string confirmationKey,
        CancellationToken ct
    )
    {
        try
        {
            UserPasswordRecoveryTicketCommand command = new(confirmationKey);
            UserPasswordRecoveryTicketCommandHandler handler = new(
                multiplexer,
                hash,
                dataSource,
                publisher
            );
            await handler.Handle(command, ct);
            return Results.Ok();
        }
        catch (UserNotFoundException ex)
        {
            logger.LogError(ex);
            return Results.NotFound(new { message = ex.Message });
        }
        catch (UserRecoveryPasswordTicketEmptyException ex)
        {
            logger.LogError(ex);
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UserRecoveryPasswordTicketNotValidException ex)
        {
            logger.LogError(ex);
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (RecoveryPasswordKeyValueNotFoundException ex)
        {
            logger.LogError(ex);
            return Results.NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogFatal(ex);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения" });
        }
    }

    private static void LogError(this Serilog.ILogger logger, Exception ex)
    {
        logger.Error("{Entrance}. {Error}.", nameof(UserPasswordRecoveryConfirmationEndpoint), ex);
    }

    private static void LogFatal(this Serilog.ILogger logger, Exception ex)
    {
        logger.Error("{Entrance}. {Error}.", nameof(UserPasswordRecoveryConfirmationEndpoint), ex);
    }
}
