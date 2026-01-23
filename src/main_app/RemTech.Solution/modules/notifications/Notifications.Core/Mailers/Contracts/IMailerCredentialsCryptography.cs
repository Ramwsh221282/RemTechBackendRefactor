namespace Notifications.Core.Mailers.Contracts;

public interface IMailerCredentialsCryptography
{
    public Task<MailerCredentials> Encrypt(MailerCredentials credentials, CancellationToken ct = default);
    public Task<MailerCredentials> Decrypt(MailerCredentials credentials, CancellationToken ct = default);
}
