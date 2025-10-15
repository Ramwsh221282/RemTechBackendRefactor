namespace Mailing.Domain.EmailShippmentContext.ValueObjects;

public readonly record struct EmailShippmentDate
{
    public DateTime ShippedAt { get; }

    public EmailShippmentDate() => ShippedAt = DateTime.Now;

    private EmailShippmentDate(DateTime shippedAt) => ShippedAt = shippedAt;
}
