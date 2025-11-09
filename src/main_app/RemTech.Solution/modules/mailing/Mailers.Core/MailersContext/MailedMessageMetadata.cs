namespace Mailers.Core.MailersContext;

public sealed record MailedMessageMetadata(Guid MailerId, Guid MessageId);