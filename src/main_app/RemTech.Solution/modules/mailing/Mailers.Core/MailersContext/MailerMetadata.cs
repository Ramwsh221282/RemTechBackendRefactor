namespace Mailers.Core.MailersContext;

public sealed record MailerMetadata(Guid Id, Email Email, SmtpPassword Password);