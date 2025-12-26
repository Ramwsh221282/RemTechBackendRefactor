namespace Mailing.Core.Mailers.Protocols;

public interface EnsureMailerEmailUniqueProtocol
{
    Task EnsureEmailUnique(Mailer mailer, CancellationToken ct);
}