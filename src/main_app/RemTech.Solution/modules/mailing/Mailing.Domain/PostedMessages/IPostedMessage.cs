namespace Mailing.Domain.PostedMessages;

public interface IPostedMessage
{
    IPostedMessageData Data { get; }
}