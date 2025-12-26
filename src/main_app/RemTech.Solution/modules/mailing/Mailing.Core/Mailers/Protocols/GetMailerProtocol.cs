namespace Mailing.Core.Mailers.Protocols;

public sealed record GetMailerQueryArgs(
    Guid? Id = null, 
    string? Email = null, 
    bool WithLock = false);

public interface GetMailerProtocol
{
    Task<Mailer?> AvailableBySendLimit(bool withLock = false, CancellationToken ct = default);
    Task<Mailer?> Get(GetMailerQueryArgs args,  CancellationToken ct = default);
}