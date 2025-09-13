using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using StackExchange.Redis;
using Users.Module.CommonAbstractions;
using Users.Module.Features.CreatingNewAccount.Exceptions;
using Users.Module.Models;

namespace Users.Module.Features.CreatingNewAccount;

internal sealed class UserRegistrationEmailEmptyException : Exception
{
    public UserRegistrationEmailEmptyException()
        : base("Почта пользователя не указана.") { }
}

internal sealed class UserRegistrationLoginEmptyException : Exception
{
    public UserRegistrationLoginEmptyException()
        : base("Логин пользователя не указан.") { }
}

public static class CreateUserAccountEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapPost("sign-up", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromServices] StringHash hash,
        [FromServices] SecurityKeySource securityKey,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromBody] RegisterUserRequest request,
        CancellationToken ct
    )
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new UserRegistrationEmailEmptyException();
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new UserRegistrationRequiresNameException();
            UserRegistrationDetails details = await new User(
                request.Name,
                request.Password,
                request.Email
            )
                .RequireRegistration()
                .FormDetails()
                .SaveIn(dataSource, hash, "USER", ct);
            UserJwt jwt = details.JwtDetails().UserJwt(securityKey);
            await jwt.StoreInCache(multiplexer);
            return jwt.AsResult();
        }
        catch (UserRegistrationEmailEmptyException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UserRegistrationLoginEmptyException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
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
            logger.Fatal("{Entrance}. {Ex}.", nameof(CreateUserAccountEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения" });
        }
    }
}
