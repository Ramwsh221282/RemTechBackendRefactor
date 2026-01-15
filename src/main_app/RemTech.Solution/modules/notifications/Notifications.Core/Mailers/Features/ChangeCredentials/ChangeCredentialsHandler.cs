using Notifications.Core.Common.Contracts;
using Notifications.Core.Mailers.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace Notifications.Core.Mailers.Features.ChangeCredentials;

[TransactionalHandler]
public sealed class ChangeCredentialsHandler(
    IMailersRepository repository,
    IMailerCredentialsCryptography cryptography,
    INotificationsModuleUnitOfWork unitOfWork
) : ICommandHandler<ChangeCredentialsCommand, Mailer>
{
    public async Task<Result<Mailer>> Execute(
        ChangeCredentialsCommand command,
        CancellationToken ct = new CancellationToken()
    )
    {
        Result<Mailer> mailer = await GetRequiredMailer(command, ct);
        if (mailer.IsFailure)
            return mailer.Error;

        if (mailer.Value.Credentials.Email != command.Email)
        {
            if (await CredentialsEmailDuplicated(command, ct))
                return Error.Conflict(
                    $"Настройка почтового сервиса с адресом: {command.Email} уже существует."
                );
        }

        MailerCredentials credentials = await CreateEncryptedCredentials(command, ct);
        mailer.Value.ChangeCredentials(credentials);
        await unitOfWork.Save(mailer.Value);
        return mailer.Value;
    }

    private async Task<MailerCredentials> CreateEncryptedCredentials(
        ChangeCredentialsCommand command,
        CancellationToken ct
    ) =>
        await MailerCredentials
            .Create(command.SmtpPassword, command.Email)
            .Value.Encrypt(cryptography, ct);

    private async Task<Result<Mailer>> GetRequiredMailer(
        ChangeCredentialsCommand command,
        CancellationToken ct
    )
    {
        MailersSpecification specification = new MailersSpecification()
            .WithId(command.Id)
            .WithLockRequired();
        return await repository.Get(specification, ct);
    }

    private async Task<bool> CredentialsEmailDuplicated(
        ChangeCredentialsCommand command,
        CancellationToken ct
    )
    {
        MailersSpecification specification = new MailersSpecification().WithEmail(command.Email);
        return await repository.Exists(specification, ct);
    }
}
