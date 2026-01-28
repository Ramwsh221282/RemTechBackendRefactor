using Notifications.Core.Mailers.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Core.Mailers.Features.AddMailer;

/// <summary>
/// Обработчик команды добавления почтового ящика.
/// </summary>
/// <param name="repository">Репозиторий для работы с почтовыми ящиками.</param>
/// <param name="cryptography">Сервис для шифрования учетных данных почтового ящика.</param>
public sealed class AddMailerHandler(IMailersRepository repository, IMailerCredentialsCryptography cryptography)
	: ICommandHandler<AddMailerCommand, Mailer>
{
	/// <summary>
	/// Выполняет команду добавления почтового ящика.
	/// </summary>
	/// <param name="command">Команда добавления почтового ящика.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды с добавленным почтовым ящиком.</returns>
	public async Task<Result<Mailer>> Execute(AddMailerCommand command, CancellationToken ct = default)
	{
		if (await MailerWithEmailExists(command, ct))
			return Error.Conflict($"Настройка почтового сервиса для адреса: {command.Email} уже существует.");

		MailerCredentials credentials = await CreateEncryptedCredentials(command, ct);
		Mailer mailer = Mailer.CreateNew(credentials);
		await repository.Add(mailer, ct);
		return mailer;
	}

	private Task<bool> MailerWithEmailExists(AddMailerCommand command, CancellationToken ct)
	{
		MailersSpecification specification = new MailersSpecification().WithEmail(command.Email);
		return repository.Exists(specification, ct);
	}

	private Task<MailerCredentials> CreateEncryptedCredentials(AddMailerCommand command, CancellationToken ct) =>
		MailerCredentials.Create(command.SmtpPassword, command.Email).Value.Encrypt(cryptography, ct);
}
