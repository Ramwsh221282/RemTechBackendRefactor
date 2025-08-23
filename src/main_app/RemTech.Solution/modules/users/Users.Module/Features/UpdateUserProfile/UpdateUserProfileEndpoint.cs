using Mailing.Module.Bus;
using Mailing.Module.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;
using StackExchange.Redis;
using Users.Module.CommonAbstractions;
using Users.Module.Features.AddUserByAdmin;
using Users.Module.Features.ChangingEmail.Exceptions;
using Users.Module.Features.CreateEmailConfirmation;
using Users.Module.Features.CreatingNewAccount.Exceptions;
using Users.Module.Features.VerifyingAdmin;
using Users.Module.Models;

namespace Users.Module.Features.UpdateUserProfile;

public static class UpdateUserProfileEndpoint
{
    internal sealed record UpdateUserProfileRequest(
        PreviousUserDetails PreviousDetails,
        UpdateUserDetails UpdateUserDetails
    );

    public static readonly Delegate HandleFn = Handle;

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromServices] MailingBusPublisher publisher,
        [FromServices] StringHash hash,
        [FromServices] HasSenderApi senderApi,
        [FromBody] UpdateUserProfileRequest request,
        [FromHeader(Name = "RemTechAccessTokenId")] string tokenId,
        CancellationToken ct
    )
    {
        try
        {
            if (!Guid.TryParse(tokenId, out Guid identifier))
                return Results.BadRequest(
                    new { message = "Ошибка авторизации. Попробуйте снова авторизоваться." }
                );
            UserJwt jwt = new UserJwt(identifier);
            jwt = await jwt.Provide(multiplexer);
            UpdateUserProfileCommand command = new(
                jwt,
                request.PreviousDetails,
                request.UpdateUserDetails
            );
            ICommandHandler<UpdateUserProfileCommand, UpdateUserProfileResult> handler =
                new UpdateUserProfileCheckIfChangedWrapper(
                    new UpdateUserProfileValidationWrapper(
                        new UpdateUserRolePermissionWrapper(
                            dataSource,
                            new UpdateUserRoleHandler(dataSource, hash, publisher, senderApi)
                        )
                    )
                );
            UpdateUserProfileResult result = await handler.Handle(command, ct);
            return Results.Ok(result);
        }
        catch (EmailEmptyException ex)
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
        catch (OnlyRootUserCanPromoteRootException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (SendersAreNotAvailableYetException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
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
        catch (RoleNotFoundException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UserNotFoundException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(UpdateUserProfileEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения" });
        }
    }
}
