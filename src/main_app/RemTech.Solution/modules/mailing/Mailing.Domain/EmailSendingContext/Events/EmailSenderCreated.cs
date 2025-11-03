using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.EmailSendingContext.Events;

public sealed class EmailSenderCreated : IDomainEvent
{
    internal EmailSenderCreated(
        Guid id,
        string email,
        string service,
        string password,
        int limit,
        int sent,
        int port
    ) => (_id, _email, _service, _password, _limit, _sent, _port) =
        (id, email, service, password, limit, sent, port);

    internal EmailSenderCreated(EmailSender sender) : this(FromSender(sender))
    {
    }

    internal EmailSenderCreated(EmailSenderCreated sender) : this(
        sender._id,
        sender._email,
        sender._service,
        sender._password,
        sender._limit,
        sender._sent,
        sender._port
    )
    {
    }

    private readonly Guid _id;
    private readonly string _email;
    private readonly string _service;
    private readonly string _password;
    private readonly int _limit;
    private readonly int _sent;
    private readonly int _port;

    private static EmailSenderCreated FromSender(EmailSender sender) =>
        from tuple in sender
            .ProjectTo((id, email, service, password, limit, sent, port) =>
                (id, email, service, password, limit, sent, port))
            .AsSuccessStatus()
        select (FromTuple(tuple));

    private static EmailSenderCreated FromTuple(
        (
            Guid id,
            string email,
            string service,
            string password,
            int limit,
            int sent,
            int port
            )
            tuple
    ) =>
        from id in tuple.id.AsSuccessStatus()
        from email in tuple.email.AsSuccessStatus()
        from service in tuple.service.AsSuccessStatus()
        from password in tuple.password.AsSuccessStatus()
        from limit in tuple.limit.AsSuccessStatus()
        from sent in tuple.sent.AsSuccessStatus()
        from port in tuple.port.AsSuccessStatus()
        let @event = new EmailSenderCreated(
            id,
            email,
            service,
            password,
            limit,
            sent,
            port
        )
        select (@event);
}