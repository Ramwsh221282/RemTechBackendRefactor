using Notifications.Core.Mailers;

namespace Notifications.Infrastructure.Mailers.Queries.GetMailers;

/// <summary>
/// Ответ с данными почтового ящика.
/// </summary>
/// <param name="Id">Идентификатор почтового ящика.</param>
/// <param name="Email">Электронная почта почтового ящика.</param>
/// <param name="SmtpHost">SMTP хост почтового ящика.</param>
public sealed record MailerResponse(Guid Id, string Email, string SmtpHost)
{
	/// <summary>
	/// Создает ответ с данными почтового ящика из сущности почтового ящика.
	/// </summary>
	/// <param name="mailer">Сущность почтового ящика.</param>
	/// <returns>Ответ с данными почтового ящика.</returns>
	public static MailerResponse Create(Mailer mailer)
	{
		return new(mailer.Id.Value, mailer.Credentials.Email, mailer.Credentials.SmtpHost);
	}
}
