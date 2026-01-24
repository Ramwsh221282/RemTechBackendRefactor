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
	public async Task<Result<Mailer>> Execute(ChangeCredentialsCommand command, CancellationToken ct = default)
	{
		Result<Mailer> mailer = await GetRequiredMailer(command, ct);
		if (mailer.IsFailure)
			return mailer.Error;

		if (mailer.Value.Credentials.Email != command.Email || await CredentialsEmailDuplicated(command, ct))
			return Error.Conflict($"Настройка почтового сервиса с адресом: {command.Email} уже существует.");

		MailerCredentials credentials = await CreateEncryptedCredentials(command, ct);
		mailer.Value.ChangeCredentials(credentials);
		await unitOfWork.Save(mailer.Value, ct);
		return mailer.Value;
	}

	private Task<MailerCredentials> CreateEncryptedCredentials(
		ChangeCredentialsCommand command,
		CancellationToken ct
	) => MailerCredentials.Create(command.SmtpPassword, command.Email).Value.Encrypt(cryptography, ct);

	private Task<Result<Mailer>> GetRequiredMailer(ChangeCredentialsCommand command, CancellationToken ct)
	{
		MailersSpecification specification = new MailersSpecification().WithId(command.Id).WithLockRequired();
		return repository.Get(specification, ct);
	}

	private Task<bool> CredentialsEmailDuplicated(ChangeCredentialsCommand command, CancellationToken ct)
	{
		MailersSpecification specification = new MailersSpecification().WithEmail(command.Email);
		return repository.Exists(specification, ct);
	}
}
