using Mailing.Domain.SendedMessageContext;
using RemTech.Core.Shared.DomainEvents;

namespace Mailing.Domain.EmailSendingContext.Events;

public sealed class EmailMessageSent : IDomainEvent
{
    private readonly EmailMessageSenderEventArgs _sender;
    private readonly EmailSentMessageEventArgs _message;

    public void Fold(
        Func<EmailMessageSenderEventArgs, Action>? senderFold = null,
        Func<EmailSentMessageEventArgs, Action>? messageFold = null)
    {
        senderFold?.Invoke(_sender);
        messageFold?.Invoke(_message);
    }

    internal EmailMessageSent(EmailSender sender, SendedMessage message)
    {
        _sender = new EmailMessageSenderEventArgs(sender);
        _message = new EmailSentMessageEventArgs(message);
    }
}