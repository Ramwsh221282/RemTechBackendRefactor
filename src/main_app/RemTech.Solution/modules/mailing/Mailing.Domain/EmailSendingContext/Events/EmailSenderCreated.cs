using Mailing.Domain.EmailSendingContext.ValueObjects;
using RemTech.Core.Shared.DomainEvents;

namespace Mailing.Domain.EmailSendingContext.Events;

public sealed record EmailSenderCreated(
    Guid Id,
    string Email,
    string Service,
    string Password,
    int SendLimit,
    int SendAmount) : IDomainEvent
{
    public EmailSenderCreated(EmailSender sender)
        : this(
            sender.Id.Value,
            sender.Email.Value,
            sender.Service.ServiceName,
            sender.Service.Password,
            sender.Statistics.SendMessageLimit,
            sender.Statistics.SendedMessagesAmount)
    {
    }
}