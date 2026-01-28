using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Notifications.Core.Mailers;

/// <summary>
/// Идентификатор почтовой рассылки.
/// </summary>
public readonly record struct MailerId
{
	/// <summary>
	/// Создает новый идентификатор почтовой рассылки.
	/// </summary>
	public MailerId()
	{
		Value = Guid.NewGuid();
	}

	private MailerId(Guid value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение идентификатора почтовой рассылки.
	/// </summary>
	public Guid Value { get; }

	/// <summary>
	/// Создает новый идентификатор почтовой рассылки.
	/// </summary>
	/// <returns>Новый идентификатор почтовой рассылки.</returns>
	public static MailerId New() => new();

	/// <summary>
	/// Создает идентификатор почтовой рассылки из заданного значения.
	/// </summary>
	/// <param name="value">Значение идентификатора почтовой рассылки.</param>
	/// <returns>Результат создания идентификатора почтовой рассылки.</returns>
	public static Result<MailerId> Create(Guid value)
	{
		return value == Guid.Empty
			? Error.Validation("Идентификатор почтового сервиса не может быть пустым.")
			: new MailerId(value);
	}
}
