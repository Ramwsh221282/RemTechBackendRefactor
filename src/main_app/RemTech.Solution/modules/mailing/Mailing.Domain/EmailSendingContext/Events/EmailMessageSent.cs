using Mailing.Domain.EmailSendingContext.ValueObjects;
using Mailing.Domain.SendedMessageContext;
using RemTech.Core.Shared.DomainEvents;

namespace Mailing.Domain.EmailSendingContext.Events;

public sealed record EmailMessageSent(
    Guid SenderId,
    Guid MessageId,
    string RecipientEmail,
    string Subject,
    string Body,
    int SenderLimit,
    int SenderCurrentLimitStatus) : IDomainEvent
{
    public EmailMessageSent(EmailSender sender, SendedMessage message)
        : this(
            sender.Id.Value,
            message.Id.Value,
            message.Content.RecipientEmail._value,
            message.Content.Subject,
            message.Content.Body,
            sender.Statistics.SendMessageLimit,
            sender.Statistics.SendedMessagesAmount)
    {
    }
}