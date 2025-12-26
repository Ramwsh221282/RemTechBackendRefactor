namespace Mailing.Core.Mailers.Protocols;

public interface DecryptMailerSmtpPasswordProtocol
{
    Task<Mailer> WithDecryptedPassword(Mailer mailer, CancellationToken ct);
}