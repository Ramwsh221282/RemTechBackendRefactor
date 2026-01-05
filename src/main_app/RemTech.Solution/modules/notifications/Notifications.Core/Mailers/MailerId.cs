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
        if (value == Guid.Empty) return Error.Validation("Идентификатор почтового сервиса не может быть пустым.");
        return new MailerId(value);
    }
}