using System.Data;
using Mailing.Module.Bus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;
using Shared.Infrastructure.Module.Frontend;
using StackExchange.Redis;
using Users.Module.CommonAbstractions;
using Users.Module.Features.ChangingEmail.Exceptions;
using Users.Module.Features.CreateEmailConfirmation;
using Users.Module.Features.CreatingNewAccount.Exceptions;
using Users.Module.Features.VerifyingAdmin;
using Users.Module.Models;

namespace Users.Module.Features.AddUserByAdmin;

public static class AddUserByAdminEndpoint
{
    public static readonly Delegate HandleFn = Handle;

    internal sealed record AddUserByAdminRequest(string Email, string Name, string Role);

    internal sealed record AddUserByAdminResponse(Guid Id, string Name, string Email, string Role)
    {
        public static AddUserByAdminResponse Create(AddUserByAdminResult result)
        {
            return new AddUserByAdminResponse(result.Id, result.Name, result.Email, result.Role);
        }
    }

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] StringHash hash,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromServices] MailingBusPublisher publisher,
        [FromServices] FrontendUrl frontendUrl,
        [FromServices] ConfirmationEmailsCache cache,
        [FromServices] Serilog.ILogger logger,
        [FromBody] AddUserByAdminRequest request,
        [FromHeader(Name = "RemTechAccessTokenId")] string tokenId,
        CancellationToken ct
    )
    {
        try
        {
            if (!Guid.TryParse(tokenId, out Guid guidTokenId))
                return Results.BadRequest(
                    new { message = "Ошибка авторизации. Попробуйте снова авторизоваться." }
                );

            UserJwt jwt = new UserJwt(guidTokenId);
            jwt = await jwt.Provide(multiplexer);
            UserJwtOutput output = jwt.MakeOutput();
            AddUserByAdminCommand command = new AddUserByAdminCommand(
                output.UserId,
                request.Email,
                request.Name,
                request.Role
            );
            ICommandHandler<AddUserByAdminCommand, AddUserByAdminResult> handler =
                new AddUserSendEmailWrapper(
                    publisher,
                    frontendUrl,
                    cache,
                    new AddUserByAdminValidatorWrapper(
                        new AddUserByAdminRoleWrapper(
                            dataSource,
                            new AddUserByAdminCommandHandler(dataSource, hash)
                        )
                    )
                );
            AddUserByAdminResult result = await handler.Handle(command, ct);
            AddUserByAdminResponse response = AddUserByAdminResponse.Create(result);
            return Results.Ok(response);
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
        catch (UserNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (RoleNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (DuplicateNameException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (EmailDuplicateException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UserRegistrationRequiresNameException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UserRegistrationRequiresEmailException ex)
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
        catch (RootUserCanBeAddedOnlyByRootException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(AddUserByAdminEndpoint), ex.Message);
            return Results.InternalServerError(new { message = ex.Message });
        }
    }
}
