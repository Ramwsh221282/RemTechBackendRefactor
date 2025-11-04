namespace Mailing.Domain.PostedMessages;

public sealed record PostedMessageData(
    Guid Id,
    Guid PostedBy,
    string Body,
    string Subject,
    string RecipientAddress,
    DateTime CreatedOn
) : IPostedMessageData;