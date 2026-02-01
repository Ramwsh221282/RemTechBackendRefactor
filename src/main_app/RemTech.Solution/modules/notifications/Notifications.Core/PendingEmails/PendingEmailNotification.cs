using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Notifications.Core.PendingEmails;

/// <summary>
/// Ожидающее email-уведомление.
/// </summary>
/// <param name="id">Уникальный идентификатор уведомления.</param>
/// <param name="recipient">Адресат email-уведомления.</param>
/// <param name="subject">Тема email-уведомления.</param>
/// <param name="body">Содержимое email-уведомления.</param>
/// <param name="wasSent">Флаг, указывающий, было ли уведомление отправлено.</param>
public sealed class PendingEmailNotification(Guid id, string recipient, string subject, string body, bool wasSent)
{
	private PendingEmailNotification(PendingEmailNotification origin)
		: this(origin.Id, origin.Recipient, origin.Subject, origin.Body, origin.WasSent) { }

	/// <summary>
	/// Уникальный идентификатор уведомления.
	/// </summary>
	public Guid Id { get; } = id;

	/// <summary>
	/// Адресат email-уведомления.
	/// </summary>
	public string Recipient { get; } = recipient;

	/// <summary>
	/// Тема email-уведомления.
	/// </summary>
	public string Subject { get; } = subject;

	/// <summary>
	/// Содержимое email-уведомления.
	/// </summary>
	public string Body { get; } = body;

	/// <summary>
	/// Флаг, указывающий, было ли уведомление отправлено.
	/// </summary>
	public bool WasSent { get; private set; } = wasSent;

	/// <summary>
	/// Создает новое ожидающее email-уведомление.
	/// </summary>
	/// <param name="recipient">Адресат email-уведомления.</param>
	/// <param name="subject">Тема email-уведомления.</param>
	/// <param name="body">Содержимое email-уведомления.</param>
	/// <returns>Уведомление для отправки.</returns>
	public static Result<PendingEmailNotification> CreateNew(string recipient, string subject, string body)
	{
		List<string> errors = [];
		if (string.IsNullOrWhiteSpace(recipient))
		{
			errors.Add("Почта поле не может быть пустой");
		}

		if (string.IsNullOrWhiteSpace(subject))
		{
			errors.Add("Тема письма не может быть пустой");
		}

		if (string.IsNullOrWhiteSpace(body))
		{
			errors.Add("Текст письма не может быть пустым");
		}

		if (errors.Count > 0)
		{
			string message = string.Join(", ", errors);
			return Error.Validation(message);
		}

		return Result.Success(new PendingEmailNotification(Guid.NewGuid(), recipient, subject, body, false));
	}

	/// <summary>
	/// Отмечает уведомление как отправленное.
	/// </summary>
	public void MarkSent()
	{
		WasSent = true;
	}

	/// <summary>
	/// Создает копию уведомления.
	/// </summary>
	/// <returns>Копия уведомления.</returns>
	public PendingEmailNotification Copy()
	{
		return new(this);
	}
}
