using RemTech.Result.Pattern;

namespace Mailing.Domain.EmailShipperContext.ValueObjects;

public sealed record EmailShipperKey
{
    public string Value { get; }

    private EmailShipperKey(string value) => Value = value;

    public static Result<EmailShipperKey> Create(string value)
    {
        return new EmailShipperKey(value);
    }
}
