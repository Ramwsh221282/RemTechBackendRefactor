namespace Mailing.Domain.PostedMessages;

public interface IPostedMessageData
{
    Guid Id { get; }
    Guid PostedBy { get; }
    string Body { get; }
    string Subject { get; }
    string RecipientAddress { get; }
    DateTime CreatedOn { get; }
}