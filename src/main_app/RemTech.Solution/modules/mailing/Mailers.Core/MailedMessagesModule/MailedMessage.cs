namespace Mailers.Core.MailedMessagesModule;

public sealed record MailedMessage(
    MailedMessageMetadata Metadata, 
    MailedMessageContent Content,
    MailedMessageDeliveryInfo DeliveryInfo);