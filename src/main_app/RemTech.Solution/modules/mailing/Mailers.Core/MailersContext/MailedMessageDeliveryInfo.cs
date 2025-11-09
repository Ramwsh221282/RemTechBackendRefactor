namespace Mailers.Core.MailersContext;

public sealed record MailedMessageDeliveryInfo(Email To, DateTime SentOn);