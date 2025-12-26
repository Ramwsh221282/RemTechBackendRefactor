namespace Mailing.Core.Mailers;

public sealed record Mailer(Guid Id,  MailerDomain Domain, MailerConfig Config);