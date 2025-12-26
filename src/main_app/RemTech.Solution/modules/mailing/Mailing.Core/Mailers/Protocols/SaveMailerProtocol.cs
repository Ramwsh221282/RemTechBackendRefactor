namespace Mailing.Core.Mailers.Protocols;

public interface SaveMailerProtocol
{
    Task Save(Mailer mailer, CancellationToken ct);
}