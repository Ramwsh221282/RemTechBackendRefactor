using Notifications.Core.Mailers.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Core.Mailers.Features.AddMailer;

public sealed class AddMailerHandler(
    IMailersRepository repository, 
    IMailerCredentialsCryptography cryptography)
    : ICommandHandler<AddMailerCommand, Mailer>
{
    public async Task<Result<Mailer>> Execute(AddMailerCommand command, CancellationToken ct = default)
    {
        if (await MailerWithEmailExists(command, ct))
            return Error.Conflict($"Настройка почтового сервиса для адреса: {command.Email} уже существует.");

        MailerCredentials credentials = await CreateEncryptedCredentials(command, ct);
        Mailer mailer = Mailer.CreateNew(credentials);
        await repository.Add(mailer, ct);
        return mailer;
    }

    private async Task<bool> MailerWithEmailExists(AddMailerCommand command, CancellationToken ct)
    {
        MailersSpecification specification = new MailersSpecification().WithEmail(command.Email);
        return await repository.Exists(specification, ct);
    }

    private async Task<MailerCredentials> CreateEncryptedCredentials(AddMailerCommand command, CancellationToken ct) =>
        await MailerCredentials.Create(command.SmtpPassword, command.Email).Value.Encrypt(cryptography, ct);
}