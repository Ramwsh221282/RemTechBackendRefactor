using Mailing.Module.Bus;
using Mailing.Module.Public;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;
using Shared.Infrastructure.Module.Frontend;
using Users.Module.CommonAbstractions;
using Users.Module.Features.ChangingEmail.Exceptions;
using Users.Module.Features.ChangingEmail.Shared;

namespace Users.Module.Features.ChangingEmail.UpdateUserEmailByAdmin;

internal sealed class UpdateUserEmailByAdminHandler(
    NpgsqlDataSource dataSource,
    MailingBusPublisher publisher,
    Serilog.ILogger logger,
    HasSenderApi hasSenderApi,
    FrontendUrl frontendUrl,
    ConfirmationEmailsCache cache
) : ICommandHandler<UpdateUserEmailByAdminCommand, UpdateUserEmailByAdminResponse>
{
    public async Task<UpdateUserEmailByAdminResponse> Handle(
        UpdateUserEmailByAdminCommand command,
        CancellationToken ct = default
    )
    {
        new EmailValidation().ValidateEmail(command.NewEmail);
        if (!await hasSenderApi.HasSender())
            throw new SendersAreNotAvailableYetException();
        await new ChangeEmailTransaction(dataSource).Execute(command.NewEmail, command.UserId, ct);
        Guid confirmationKey = await new EmailConfirmationKeyGeneration(cache).Generate(
            command.UserId
        );
        await new EmailChangeMailingMessage(frontendUrl, confirmationKey, publisher).Send(
            command.NewEmail,
            ct
        );
        logger.Information(
            "User {Guid} has updated email by admin {Email}.",
            command.UserId,
            command.NewEmail
        );
        return new UpdateUserEmailByAdminResponse(command.UserId, command.NewEmail);
    }
}
