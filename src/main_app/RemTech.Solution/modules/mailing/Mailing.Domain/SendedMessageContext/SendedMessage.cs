using Mailing.Domain.SendedMessageContext.ValueObjects;

namespace Mailing.Domain.SendedMessageContext;

public sealed class SendedMessage
{
    public SendedMessageId Id { get; }
    public SendedMessageSenderId SenderId { get; }
    public SendedMessageDate Date { get; }
    public SendedMessageContent Content { get; }

    public SendedMessage(
        SendedMessageSenderId senderId,
        SendedMessageContent content,
        SendedMessageDate date,
        SendedMessageId? id = null)
    {
        SenderId = senderId;
        Date = date;
        Content = content;
        Id = id ?? new SendedMessageId();
    }
}