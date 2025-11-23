namespace Mailing.Core.Mailers.Protocols;

public interface EncryptMailerSmtpPasswordProtocol
{ 
    Task<Mailer> WithEncryptedPassword(Mailer mailer, CancellationToken ct);
}