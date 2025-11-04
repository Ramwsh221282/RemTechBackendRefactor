namespace Mailing.Domain.PostedMessages;

public sealed class PostedMessage(IPostedMessageData Data) : IPostedMessage
{
    public IPostedMessageData Data { get; } = Data;
}