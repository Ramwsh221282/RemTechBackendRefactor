namespace Mailing.Domain.PostedMessages;

public abstract class PostedMessageEnvelope(IPostedMessage message) : IPostedMessage
{
    public IPostedMessageData Data { get; } = message.Data;
}