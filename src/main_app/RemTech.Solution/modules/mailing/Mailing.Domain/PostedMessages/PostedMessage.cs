namespace Mailing.Domain.PostedMessages;

public sealed class PostedMessage(IPostedMessageData data) : IPostedMessage
{
    public IPostedMessageData Data { get; } = data;
}