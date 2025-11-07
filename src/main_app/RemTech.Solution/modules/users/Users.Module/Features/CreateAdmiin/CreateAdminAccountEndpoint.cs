using Mailing.Moduled.Bus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using StackExchange.Redis;
using Users.Module.CommonAbstractions;
using Users.Module.Features.CreatingNewAccount;
using Users.Module.Features.CreatingNewAccount.Exceptions;
using Users.Module.Features.VerifyingAdmin;
using Users.Module.Models;

namespace Users.Module.Features.CreateAdmiin;

public static class CreateAdminAccountEndpoint
{
    internal sealed record CreateAdminRequest(string Email, string Name);

    public static Delegate HandleFn => Handle;

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromServices] MailingBusPublisher publisher,
        [FromServices] StringHash hash,
        [FromServices] SecurityKeySource securityKey,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromBody] CreateAdminRequest request,
        [FromHeader(Name = "RemTechAccessTokenId")]
        string tokenId,
        CancellationToken ct
    )
    {
        try
        {
            if (!Guid.TryParse(tokenId, out Guid tokenGuid))
                return Results.BadRequest(
                    new { message = "Проблема с авторизацией. Попробуйте авторизоваться заново." }
                );

            UserJwt incomingJwt = new UserJwt(tokenGuid);
            incomingJwt = await incomingJwt.Provide(multiplexer);
            bool isRoot = incomingJwt.IsOfRole("ROOT");
            if (!isRoot)
                return Results.BadRequest(
                    new { message = "Функция доступна пользователю с правами ROOT." }
                );

            string generatedPassword = Guid.NewGuid().ToString();
            UserRegistrationDetails details = await new User(
                    request.Name,
                    generatedPassword,
                    request.Email
                )
                .RequireRegistration()
                .FormDetails()
                .SaveIn(dataSource, hash, "ADMIN", ct);
            UserJwt jwt = details.JwtDetails().UserJwt(securityKey);
            await jwt.StoreInCache(multiplexer);
            return jwt.AsResult();
        }
        catch (UserJwtTokenComparisonDifferentException)
        {
            return Results.BadRequest(
                new { message = "Проблема с авторизацией. Попробуйте авторизоваться заново." }
            );
        }
        catch (UnableToGetUserJwtValueException)
        {
            return Results.BadRequest(
                new { message = "Проблема с авторизацией. Попробуйте авторизоваться заново." }
            );
        }
        catch (InvalidEmailFormatException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (NameExceesLengthException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (PasswordDoesNotContainLowerCharacterException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (PasswordDoesNotContainUpperCharacterException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (PasswordLengthIsNotSatisfiedException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (PasswordShouldContainDigitException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UserRegistrationRequiresEmailException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UserRegistrationRequiresNameException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UserRegistrationRequiresPasswordException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (EmailDuplicateException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (NameDuplicateException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(CreateAdminAccountEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения" });
        }
    }
}