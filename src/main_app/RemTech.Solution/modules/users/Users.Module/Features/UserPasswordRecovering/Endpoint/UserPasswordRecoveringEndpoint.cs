using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using StackExchange.Redis;
using Users.Module.Features.UserPasswordRecovering.Interactor;

namespace Users.Module.Features.UserPasswordRecovering.Endpoint;

public static class UserPasswordRecoveringEndpoint
{
    private const string Entrance = nameof(UserPasswordRecoveringEndpoint);

    public static void Map(RouteGroupBuilder builder) => builder.MapGet("password-reset", Handle);

    private static async Task<IResult> Handle(
        [FromServices] Serilog.ILogger logger,
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromServices] FrontendUrl frontendUrl,
        [FromServices] MailingBusPublisher publisher,
        [FromServices] HasSenderApi hasSender,
        [FromHeader(Name = "Email")] string? email,
        [FromHeader(Name = "Login")] string? login,
        CancellationToken ct
    )
    {
        try
        {
            await hasSender.HasSenderAndThrowIfNotExists();
            UserPasswordRecoverInteractor interactor = new(
                dataSource,
                multiplexer,
                frontendUrl,
                publisher,
                logger
            );
            await interactor.Handle(new UserPasswordRecoverRequest(email, login), ct);
            return Results.Ok();
        }
        catch (SendersAreNotAvailableYetException ex)
        {
            logger.Error("{Entrance}. {Error}", Entrance, ex);
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UnableToDetermineHowToResetPasswordException ex)
        {
            logger.Error("{Entrance}. {Error}", Entrance, ex);
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UserRecoveringPasswordByEmailInvalidException ex)
        {
            logger.Error("{Entrance}. {Error}", Entrance, ex);
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UserRecoveringPasswordByLoginEmptyException ex)
        {
            logger.Error("{Entrance}. {Error}", Entrance, ex);
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UserRecoveringPasswordKeyAlreadyExistsException ex)
        {
            logger.Error("{Entrance}. {Error}", Entrance, ex);
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UserToRecoverNotFoundException ex)
        {
            logger.Error("{Entrance}. {Error}", Entrance, ex);
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Error}", Entrance, ex);
            return Results.InternalServerError(new { message = ex.Message });
        }
    }
}