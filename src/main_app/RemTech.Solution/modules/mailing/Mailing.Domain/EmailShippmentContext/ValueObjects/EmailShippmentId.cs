namespace Mailing.Domain.EmailShippmentContext.ValueObjects;

public readonly record struct EmailShippmentId
{
    public Guid Id { get; }

    public EmailShippmentId() => Id = Guid.NewGuid();

    private EmailShippmentId(Guid id) => Id = id;
}
