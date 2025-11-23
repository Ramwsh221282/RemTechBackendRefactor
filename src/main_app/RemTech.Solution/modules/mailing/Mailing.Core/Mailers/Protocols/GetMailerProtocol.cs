namespace Mailing.Core.Mailers.Protocols;

public interface GetMailerProtocol
{
    Task<Mailer?> AvailableBySendLimit(CancellationToken ct = default);
    Task<Mailer?> ByEmail(string email, CancellationToken ct = default);
    Task<Mailer?> ById(Guid id, CancellationToken ct = default);
}