namespace Mailers.Core.MailersContext;

public sealed record MailerSending(Mailer Mailer, MailedMessage Message);