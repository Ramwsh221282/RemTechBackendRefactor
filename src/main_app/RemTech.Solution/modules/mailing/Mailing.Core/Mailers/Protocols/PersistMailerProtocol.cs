namespace Mailing.Core.Mailers.Protocols;

public interface PersistMailerProtocol
{
    Task Persist(Mailer mailer, CancellationToken ct);
}