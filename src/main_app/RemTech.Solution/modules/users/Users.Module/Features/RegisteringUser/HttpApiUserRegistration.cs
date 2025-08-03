using Microsoft.AspNetCore.Http;
using Serilog;
using Users.Module.Features.RegisteringUser.Exceptions;
using Users.Module.Features.RegisteringUser.Storage;

namespace Users.Module.Features.RegisteringUser;

internal sealed class HttpApiUserRegistration(
    IUserToRegister user,
    INewUsersStorage storage,
    ILogger logger
)
{
    public async Task<IResult> Process(RegisterUserRequest request, CancellationToken ct = default)
    {
        try
        {
            await user.Register(storage, ct);
            return Results.Ok(new { message = "Регистрация успешна." });
        }
        catch (UserRegistrationNickNameConflictException ex)
        {
            return Results.Conflict(new { message = ex.Message });
        }
        catch (UserRegistrationEmailConflictException ex)
        {
            return Results.Conflict(new { message = ex.Message });
        }
        catch (UserRegistrationValidationFailedException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "Fatal error. Feature: {Name}. Exception: {Ex}.",
                nameof(HttpApiUserRegistration),
                ex.Message
            );
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}
