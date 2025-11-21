using Mailing.Core.Abstractions;
using RemTech.Primitives.Extensions.Exceptions;

namespace Mailing.Core.Inbox.InboxMessages;

public sealed class InboxMessage(
    string targetEmail, 
    string subject, 
    string body,
    SyncMessagesChoreography? choreography = null)
{
    public void Register()
    {
        choreography?.SendMessage(new InboxMessageRegistrationCalled(
            TargetEmail: targetEmail,
            Subject: subject,
            Body: body
            ));
    }
}

public sealed class InboxMessageRegistrationOffice  
    : IMessageListener<InboxMessageRegistrationCalled>
{
    private readonly bool _isClosed;
    private readonly SyncMessagesChoreography? _choreography;
    
    public void React(InboxMessageRegistrationCalled message)
    {
        if (_isClosed) MessageException.Validation("Регистрация inbox сообщений закрыта.");
        DateTime registrationDate = DateTime.Now;
        _choreography?.SendMessage(new InboxMessageRegistered(
            TargetEmail: message.TargetEmail,
            Subject: message.Subject,
            Body: message.Body,
            RegistrationDate: registrationDate
            ));
    }

    public InboxMessageRegistrationOffice(bool isClosed, SyncMessagesChoreography? choreography = null)
    {
        _isClosed = isClosed;
        choreography?.RegisterListener(this);
        _choreography = choreography;
        
    }
}

public sealed class InboxMessageStore : IMessageListener<InboxMessageRegistered>
{
    private int _count;
    
    public void React(InboxMessageRegistered message)
    {
        _count++;
        Console.WriteLine($"""
                           Registered message:
                           Target email: {message.TargetEmail}
                           Subject: {message.Subject}
                           Body: {message.Body}
                           RegistrationDate: {message.RegistrationDate}
                           Current storage count: {_count}
                           """);
    }

    public InboxMessageStore(SyncMessagesChoreography? choreography = null)
    {
        choreography?.RegisterListener(this);
    }
}

public sealed record InboxMessageRegistrationCalled(string TargetEmail, string Subject, string Body) : IMessage;
public sealed record InboxMessageRegistered(string TargetEmail, string Subject, string Body, DateTime RegistrationDate) : IMessage;