namespace Notifications.Core.Mailers.Contracts;

public interface IMailerCredentialsCryptography
{
	Task<MailerCredentials> Encrypt(MailerCredentials credentials, CancellationToken ct = default);
	Task<MailerCredentials> Decrypt(MailerCredentials credentials, CancellationToken ct = default);
}
