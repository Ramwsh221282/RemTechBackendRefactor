namespace Mailing.Domain.EmailShipperContext.ValueObjects;

public readonly record struct EmailShipperId
{
    public Guid Value { get; }

    public EmailShipperId() => Value = Guid.NewGuid();

    private EmailShipperId(Guid value) => Value = value;
}
