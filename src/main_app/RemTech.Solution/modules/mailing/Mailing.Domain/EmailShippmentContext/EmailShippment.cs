using Mailing.Domain.CommonContext.ValueObjects;
using Mailing.Domain.EmailShippmentContext.ValueObjects;

namespace Mailing.Domain.EmailShippmentContext;

public sealed class EmailShippment
{
    public EmailShippmentId Id { get; }
    public EmailDestinationDetails Destination { get; }
    public EmailMessageDetails Message { get; }
    public EmailShippmentDate Date { get; }

    public EmailShippment(
        EmailShippmentId id,
        EmailDestinationDetails destination,
        EmailMessageDetails message,
        EmailShippmentDate date
    )
    {
        Id = id;
        Destination = destination;
        Message = message;
        Date = date;
    }

    public void Subscribe(EmailShippmentProcess process)
    {
        Destination.Subscribe(process);
        Message.Subscribe(process);
    }
}
