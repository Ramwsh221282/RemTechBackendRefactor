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

    public Func<Task<Status<Unit>>> Save(IEmailSendersStorage storage) =>
        async () => await storage.Accept((command) =>
        {
            command.CommandText = """
                                  INSERT INTO mailing_module.senders
                                  (id, email, service, password, send_limit, current_sent)
                                  VALUES
                                  (@id, @email, @service, @password, @send_limit, @current_sent)
                                  """;

            command = _id.AppendParameter(command);
            command = _email.AppendParameter(command);
            command = _service.AddParameter(command);
            command = _statistics.AddParameter(command);
        });

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