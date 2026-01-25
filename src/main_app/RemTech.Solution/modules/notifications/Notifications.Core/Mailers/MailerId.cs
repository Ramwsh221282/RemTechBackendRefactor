using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Notifications.Core.Mailers;

public readonly record struct MailerId
{
	public MailerId() => Value = Guid.NewGuid();

	private MailerId(Guid value) => Value = value;

	public Guid Value { get; }

	public static MailerId New() => new();

	public static Result<MailerId> Create(Guid value)
	{
		return value == Guid.Empty
			? Error.Validation("Идентификатор почтового сервиса не может быть пустым.")
			: new MailerId(value);
	}
}
