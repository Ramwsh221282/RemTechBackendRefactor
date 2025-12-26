using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.Postgres;
using Shared.Infrastructure.Module.Redis;
using Users.Module.CommonAbstractions;
using Users.Module.Features.ChangingEmail;
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
        [FromServices] PostgresDatabase dataSource,
        [FromServices] StringHash hash,
        [FromServices] RedisCache multiplexer,
        [FromServices] MailingBusPublisher publisher,
        [FromServices] IOptions<FrontendOptions> frontendUrl,
        [FromServices] Serilog.ILogger logger,
        [FromBody] AddUserByAdminRequest request,
        [FromHeader(Name = "RemTechAccessTokenId")]
        string tokenId,
        CancellationToken ct
    )
    {
        if (!Guid.TryParse(tokenId, out Guid guidTokenId))
            return Results.BadRequest(
                new { message = "Ошибка авторизации. Попробуйте снова авторизоваться." }
            );

        ConfirmationEmailsCache cache = new(multiplexer);
        UserJwt jwt = new UserJwt(guidTokenId);
        ICommandHandler<AddUserByAdminCommand, Status<AddUserByAdminResult>> handler =
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

        try
        {
            jwt = await jwt.Provide(multiplexer);
            UserJwtOutput output = jwt.MakeOutput();
            AddUserByAdminCommand command = new(
                output.UserId,
                request.Email,
                request.Name,
                request.Role
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