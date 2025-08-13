using Mailing.Module.Bus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using StackExchange.Redis;
using Users.Module.CommonAbstractions;
using Users.Module.Models.Features.CreatingNewAccount;
using Users.Module.Models.Features.CreatingNewAccount.Exceptions;

namespace Users.Module.Models.Features.CreateAdmiin;

public static class CreateAdminAccountEndpoint
{
    // public static void Map(RouteGroupBuilder builder) => builder.MapPost("admin-up", Handle);

    public static Delegate HandleFn => Handle;

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromServices] MailingBusPublisher publisher,
        [FromServices] StringHash hash,
        [FromServices] SecurityKeySource securityKey,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromBody] RegisterUserRequest request,
        CancellationToken ct
    )
    {
        try
        {
            UserRegistrationDetails details = await new User(
                request.Name,
                request.Password,
                request.Email
            )
                .RequireRegistration()
                .FormDetails()
                .SaveIn(dataSource, hash, "ADMIN", ct);
            UserJwt jwt = details.JwtDetails().UserJwt(securityKey);
            await jwt.StoreInCache(multiplexer);
            return jwt.AsResult();
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
