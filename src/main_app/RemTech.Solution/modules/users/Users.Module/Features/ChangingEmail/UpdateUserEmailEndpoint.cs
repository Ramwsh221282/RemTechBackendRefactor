using Mailing.Module.Bus;
using Mailing.Module.Public;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Shared.Infrastructure.Module.Frontend;
using StackExchange.Redis;
using Users.Module.Features.ChangingEmail.Exceptions;
using Users.Module.Features.CreatingNewAccount.Exceptions;
using Users.Module.Features.VerifyingAdmin;
using Users.Module.Models;

namespace Users.Module.Features.ChangingEmail;

public static class UpdateUserEmailEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapPatch("email", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] MailingBusPublisher publisher,
        [FromServices] Serilog.ILogger logger,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromServices] HasSenderApi hasSenderApi,
        [FromServices] FrontendUrl frontendUrl,
        [FromServices] ConfirmationEmailsCache emailsCache,
        [FromHeader(Name = "RemTechAccessTokenId")] string tokenId,
        [FromHeader(Name = "Password")] string password,
        [FromHeader(Name = "Id")] string id,
        [FromHeader(Name = "NewEmail")] string newEmail,
        CancellationToken ct
    )
    {
        try
        {
            if (!Guid.TryParse(tokenId, out Guid guidTokenId))
                return Results.BadRequest(
                    new { message = "Ошибка авторизации. Попробуйте снова авторизоваться." }
                );
            if (!Guid.TryParse(id, out Guid userId))
                return Results.BadRequest(
                    new { message = "Ошибка авторизации. Проблема с идентификацией пользователя." }
                );
            UserJwt jwt = new UserJwt(guidTokenId);
            jwt = await jwt.Provide(multiplexer);
            UserJwtOutput jwtOutput = jwt.MakeOutput();
            if (jwtOutput.UserId != userId)
            {
                await jwt.Deleted(multiplexer);
                return Results.BadRequest(
                    new { message = "Ошибка авторизации. Проблема с идентификацией пользователя." }
                );
            }
            UpdateUserEmailCommand command = new(jwt, newEmail, password, userId);
            UpdateUserEmailHandler handler = new(
                dataSource,
                publisher,
                logger,
                multiplexer,
                hasSenderApi,
                frontendUrl,
                emailsCache
            );
            UpdateUserEmailResponse response = await handler.Handle(command, ct);
            return Results.Ok(response);
        }
        catch (SendersAreNotAvailableYetException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (EmailEmptyException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (InvalidEmailFormatException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (EmailDuplicateException ex)
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
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(UpdateUserEmailEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения" });
        }
    }
}
