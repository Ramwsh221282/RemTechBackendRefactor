namespace Mailers.Core.MailersContext;

public sealed record MailedMessage(
    MailedMessageMetadata Metadata, 
    MailedMessageContent Content,
    MailedMessageDeliveryInfo DeliveryInfo);