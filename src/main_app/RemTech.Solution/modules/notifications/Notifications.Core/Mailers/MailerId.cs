using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Notifications.Core.Mailers;

public readonly record struct MailerId
{
    public Guid Value { get; }

    public MailerId() => Value = Guid.NewGuid();

    private MailerId(Guid value) => Value = value;

    public static MailerId New() => new();

    public static Result<MailerId> Create(Guid value)
    {
        return value == Guid.Empty ? (Result<MailerId>)Error.Validation("Идентификатор почтового сервиса не может быть пустым.") : (Result<MailerId>)new MailerId(value);
    }
}
