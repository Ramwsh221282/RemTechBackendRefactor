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

namespace Users.Module.Features.ChangingEmail.UpdateUserEmailByAdmin;

public static class UpdateUserEmailByAdminEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapPatch("email-admin", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] MailingBusPublisher publisher,
        [FromServices] Serilog.ILogger logger,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromServices] HasSenderApi hasSenderApi,
        [FromServices] FrontendUrl frontendUrl,
        [FromServices] ConfirmationEmailsCache emailsCache,
        [FromBody] UpdateUserEmailByAdminCommand byAdminCommand,
        [FromHeader(Name = "RemTechAccessTokenId")] string tokenId,
        CancellationToken ct
    )
    {
        try
        {
            if (!Guid.TryParse(tokenId, out Guid guidTokenId))
                return Results.BadRequest(
                    new { message = "Ошибка авторизации. Попробуйте снова авторизоваться" }
                );
            UserJwt jwt = new UserJwt(guidTokenId);
            jwt = await jwt.Provide(multiplexer);
            if (jwt.IsOfRole("ROOT") || jwt.IsOfRole("ADMIN") == false)
                return Results.BadRequest(
                    new { message = "Функция доступна юзеру с правами ROOT или ADMIN." }
                );
            UpdateUserEmailByAdminHandler byAdminHandler = new UpdateUserEmailByAdminHandler(
                dataSource,
                publisher,
                logger,
                hasSenderApi,
                frontendUrl,
                emailsCache
            );
            UpdateUserEmailByAdminResponse byAdminResponse = await byAdminHandler.Handle(
                byAdminCommand,
                ct
            );
            return Results.Ok(byAdminResponse);
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
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(UpdateUserEmailByAdminEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения" });
        }
    }
}
