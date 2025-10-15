using RemTech.Result.Pattern;

namespace Mailing.Domain.EmailShipperContext.ValueObjects;

public readonly record struct EmailShipperId
{
    public Guid Value { get; }

    public EmailShipperId() => Value = Guid.NewGuid();

    private EmailShipperId(Guid value) => Value = value;

    public static Result<EmailShipperId> Create(Guid value)
    {
        return new EmailShipperId(value);
    }
}
