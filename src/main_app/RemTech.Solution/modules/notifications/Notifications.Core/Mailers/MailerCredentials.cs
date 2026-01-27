using System.Text.RegularExpressions;
using Notifications.Core.Mailers.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Notifications.Core.Mailers;

/// <summary>
/// Учетные данные почтовой рассылки.
/// </summary>
public sealed partial record MailerCredentials
{
	private static readonly string[] _allowedSmtpHosts = ["smtp.yandex.ru", "smtp.mail.ru", "smtp.gmail.com"];
	private static readonly Regex _emailRegex = RegexExpression();

	private MailerCredentials(string smtpPassword, string smtpHost, string email)
	{
		SmtpPassword = smtpPassword;
		SmtpHost = smtpHost;
		Email = email;
	}

	/// <summary>
	///  Пароль SMTP-сервиса.
	/// </summary>
	public string SmtpPassword { get; }

	/// <summary>
	/// Хост SMTP-сервиса.
	/// </summary>
	public string SmtpHost { get; }

	/// <summary>
	/// Email почтовой рассылки.
	/// </summary>
	public string Email { get; }

	/// <summary>
	/// Создает учетные данные почтовой рассылки.
	/// </summary>
	/// <param name="smtpPassword">Пароль SMTP-сервиса.</param>
	/// <param name="email">Email почтовой рассылки.</param>
	/// <returns>Результат создания учетных данных почтовой рассылки.</returns>
	public static Result<MailerCredentials> Create(string smtpPassword, string email)
	{
		if (string.IsNullOrWhiteSpace(smtpPassword))
			return Error.Validation("Пароль SMTP-сервиса не может быть пустым.");
		if (string.IsNullOrWhiteSpace(email))
			return Error.Validation("Адрес почты не может быть пустым.");
		if (!HasValidEmailFormat(email, out Error error))
			return error;
		Result<string> resolvedSmtpHost = ResolveSmtpHost(email);
		return resolvedSmtpHost.IsFailure
			? resolvedSmtpHost.Error
			: new MailerCredentials(smtpPassword, resolvedSmtpHost.Value, email);
	}

	/// <summary>
	/// Шифрует учетные данные почтовой рассылки.
	/// </summary>
	/// <param name="cryptography">Объект для выполнения криптографических операций.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию шифрования.</returns>
	public Task<MailerCredentials> Encrypt(
		IMailerCredentialsCryptography cryptography,
		CancellationToken ct = default
	) => cryptography.Encrypt(this, ct);

	/// <summary>
	/// Дешифрует учетные данные почтовой рассылки.
	/// </summary>
	/// <param name="cryptography">Объект для выполнения криптографических операций.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию дешифрования.</returns>
	public Task<MailerCredentials> Decrypt(
		IMailerCredentialsCryptography cryptography,
		CancellationToken ct = default
	) => cryptography.Decrypt(this, ct);

	[GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "ru-RU")]
	private static partial Regex RegexExpression();

	private static bool HasValidEmailFormat(string email, out Error error)
	{
		error = Error.NoError();
		if (!_emailRegex.IsMatch(email))
		{
			error = Error.InvalidFormat("Некорректный формат почты.");
			return false;
		}

		return true;
	}

	private static Result<string> ResolveSmtpHost(string email)
	{
		string[] parts = email.Split('@');
		string host = parts[1];
		string? resolved = _allowedSmtpHosts.FirstOrDefault(h => h.EndsWith(host, StringComparison.OrdinalIgnoreCase));
		return resolved ?? Error.Validation($"Хост почты: {host} не поддерживается для настройки почтового сервиса.");
	}
}
