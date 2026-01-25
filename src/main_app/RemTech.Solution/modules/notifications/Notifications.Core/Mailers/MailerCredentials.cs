using System.Text.RegularExpressions;
using Notifications.Core.Mailers.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Notifications.Core.Mailers;

public sealed partial record MailerCredentials
{
	private static readonly string[] AllowedSmtpHosts = ["smtp.yandex.ru", "smtp.mail.ru", "smtp.gmail.com"];
	private static readonly Regex EmailRegex = RegexExpression();

	public string SmtpPassword { get; }
	public string SmtpHost { get; }
	public string Email { get; }

	private MailerCredentials(string smtpPassword, string smtpHost, string email)
	{
		SmtpPassword = smtpPassword;
		SmtpHost = smtpHost;
		Email = email;
	}

	public Task<MailerCredentials> Encrypt(
		IMailerCredentialsCryptography cryptography,
		CancellationToken ct = default
	) => cryptography.Encrypt(this, ct);

	public Task<MailerCredentials> Decrypt(
		IMailerCredentialsCryptography cryptography,
		CancellationToken ct = default
	) => cryptography.Decrypt(this, ct);

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

	private static bool HasValidEmailFormat(string email, out Error error)
	{
		error = Error.NoError();
		if (!EmailRegex.IsMatch(email))
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
		string? resolved = AllowedSmtpHosts.FirstOrDefault(h => h.EndsWith(host, StringComparison.OrdinalIgnoreCase));
		return resolved ?? Error.Validation($"Хост почты: {host} не поддерживается для настройки почтового сервиса.");
	}

	[GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "ru-RU")]
	private static partial Regex RegexExpression();
}
