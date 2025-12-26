using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.Postgres;
using Shared.Infrastructure.Module.Redis;
using Users.Module.Models;

namespace Users.Module.Features.ChangingEmail;

public static class UpdateUserEmailEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapPatch("email", Handle);

    private static async Task<IResult> Handle(
        [FromServices] PostgresDatabase dataSource,
        [FromServices] MailingBusPublisher publisher,
        [FromServices] Serilog.ILogger logger,
        [FromServices] RedisCache multiplexer,
        [FromServices] HasSenderApi hasSenderApi,
        [FromServices] IOptions<FrontendOptions> frontendUrl,
        [FromHeader(Name = "RemTechAccessTokenId")]
        string tokenId,
        [FromHeader(Name = "Password")] string password,
        [FromHeader(Name = "Id")] string id,
        [FromHeader(Name = "NewEmail")] string newEmail,
        CancellationToken ct
    )
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
        ConfirmationEmailsCache emailsCache = new ConfirmationEmailsCache(multiplexer);
        UpdateUserEmailHandler handler = new(
            dataSource,
            publisher,
            logger,
            multiplexer,
            hasSenderApi,
            frontendUrl,
            emailsCache
        );

        try
        {
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