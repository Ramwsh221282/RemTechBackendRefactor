using Mailing.Moduled.Bus;
using Mailing.Moduled.Public;
using Microsoft.Extensions.Options;
using RemTech.Core.Shared.Cqrs;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.Postgres;
using Shared.Infrastructure.Module.Redis;
using Users.Module.CommonAbstractions;
using Users.Module.Features.ChangingEmail.Shared;

namespace Users.Module.Features.ChangingEmail;

internal sealed class UpdateUserEmailHandler(
    PostgresDatabase dataSource,
    MailingBusPublisher publisher,
    Serilog.ILogger logger,
    RedisCache multiplexer,
    HasSenderApi hasSenderApi,
    IOptions<FrontendOptions> frontendUrl,
    ConfirmationEmailsCache cache
) : ICommandHandler<UpdateUserEmailCommand, UpdateUserEmailResponse>
{
    public async Task<UpdateUserEmailResponse> Handle(
        UpdateUserEmailCommand command,
        CancellationToken ct = default
    )
    {
        new EmailValidation().ValidateEmail(command.NewEmail);
        if (!await hasSenderApi.HasSender())
            throw new SendersAreNotAvailableYetException();

        if (!await IsAuthorized(command))
            throw new UnauthorizedAccessException();

        await new ChangeEmailTransaction(dataSource).Execute(command.NewEmail, command.UserId, ct);

        Guid confirmationKey = await new EmailConfirmationKeyGeneration(cache).Generate(
            command.UserId
        );

        await new EmailChangeMailingMessage(frontendUrl, confirmationKey, publisher).Send(
            command.NewEmail,
            ct
        );

        logger.Information(
            "User {Guid} has updated email by self {Email}.",
            command.UserId,
            command.NewEmail
        );

        return new UpdateUserEmailResponse(command.UserId, command.NewEmail);
    }

    private async Task<bool> IsAuthorized(UpdateUserEmailCommand command)
    {
        string inputPassword = command.InputPassword;
        string realPassword = command.UserJwt.MakeOutput().Password;
        if (BCrypt.Net.BCrypt.Verify(inputPassword, realPassword))
            return true;
        await command.UserJwt.Deleted(multiplexer);
        return false;
    }
}