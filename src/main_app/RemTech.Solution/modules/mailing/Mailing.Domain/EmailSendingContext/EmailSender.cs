using Mailing.Domain.EmailSendingContext.Events;
using Mailing.Domain.EmailSendingContext.Ports;
using Mailing.Domain.EmailSendingContext.ValueObjects;
using Mailing.Domain.SendedMessageContext;
using Mailing.Domain.SendedMessageContext.ValueObjects;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.EmailSendingContext;

public sealed class EmailSender
{
    private const int Port = 587;
    private readonly List<IDomainEvent> _events = [];
    private readonly EmailSenderId _id;
    private readonly EmailString _email;
    private readonly SenderServiceInformation _service;
    private SenderServiceStatistics _statistics;

    private EmailSender(
        EmailString email,
        SenderServiceInformation service,
        SenderServiceStatistics statistics,
        EmailSenderId? id = null
    )
    {
        _email = email;
        _service = service;
        _statistics = statistics;
        _id = id ?? new EmailSenderId();
    }

    public Status<SendedMessage> SendMessage(SendedMessageContent content) =>
        from limit_checking in EnsureLimitNotReached()
        from sent_message in SentMessage(content)
        let updated_statistics = UpdateStatistics(_statistics.OnMessageSent())
        let created_event = AddDomainEvent(new EmailMessageSent(this, sent_message))
        select sent_message;

    public Status<SendedMessage> SendMessage(
        string recipient,
        string subject,
        string body
    ) =>
        from limit_checking in EnsureLimitNotReached()
        from email in EmailString.Create(recipient)
        from content in SendedMessageContent.Create(email, subject, body)
        select SendMessage(content);

    public T ProjectTo<T>(EmailSenderDataSink<T> sinkMethod) =>
        from id in _id.Fold(x => x).AsSuccessStatus()
        from email in _email.Fold(x => x).AsSuccessStatus()
        from service_fold in _service.Fold((service, password) => (service, password)).AsSuccessStatus()
        from stats_fold in _statistics.Fold((limit, current) => (limit, current)).AsSuccessStatus()
        let service_name = service_fold.service
        let service_password = service_fold.password
        let limit = stats_fold.limit
        let current = stats_fold.current
        select sinkMethod(id, email, service_name, service_password, limit, current);


    private Status<Unit> EnsureLimitNotReached() =>
        _statistics.IsLimitReached()
            ? _statistics.StatisticsReachedError(_service)
            : Unit.Value;

    private Status<Unit> UpdateStatistics(Func<SenderServiceStatistics> updateFn) =>
        Unit.Return(() => _statistics = updateFn());

    private Status<Unit> AddDomainEvent(IDomainEvent @event) =>
        Unit.Return(() => _events.Add(@event));

    private Status<SendedMessage> SentMessage(SendedMessageContent content) =>
        new SendedMessage(
            SendedMessageSenderId.Create(_id.Fold(id => id)),
            content,
            new SendedMessageDate()
        );

    public async Task<Status> PublishEvents(IDomainEventsDispatcher dispatcher, CancellationToken ct = default) =>
        await dispatcher.Dispatch(_events, ct);

    public static EmailSender Create(EmailString email,
        SenderServiceInformation service,
        SenderServiceStatistics statistics,
        EmailSenderId? id = null
    )
    {
        var sender = new EmailSender(email, service, statistics, id);
        if (id == null)
            sender._events.Add(new EmailSenderCreated(sender));
        return sender;
    }
}