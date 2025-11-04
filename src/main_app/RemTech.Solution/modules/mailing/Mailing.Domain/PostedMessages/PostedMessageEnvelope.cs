namespace Mailing.Domain.PostedMessages;

public abstract class PostedMessageEnvelope(IPostedMessage Message) : IPostedMessage
{
    public IPostedMessageData Data { get; } = Message.Data;
}