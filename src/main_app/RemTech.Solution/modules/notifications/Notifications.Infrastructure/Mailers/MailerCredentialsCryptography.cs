using Notifications.Core.Mailers;
using Notifications.Core.Mailers.Contracts;
using RemTech.SharedKernel.Infrastructure.AesEncryption;

namespace Notifications.Infrastructure.Mailers;

public sealed class MailerCredentialsCryptography(AesCryptography cryptography) : IMailerCredentialsCryptography
{
	private AesCryptography Cryptography { get; } = cryptography;

	public async Task<MailerCredentials> Encrypt(MailerCredentials credentials, CancellationToken ct = default)
	{
		string encryptedPassword = await Cryptography.EncryptText(credentials.SmtpPassword, ct);
		return Recreate(credentials, encryptedPassword);
	}

	public async Task<MailerCredentials> Decrypt(MailerCredentials credentials, CancellationToken ct = default)
	{
		string decryptedPassword = await Cryptography.DecryptText(credentials.SmtpPassword, ct);
		return Recreate(credentials, decryptedPassword);
	}

	private static MailerCredentials Recreate(MailerCredentials credentials, string password) =>
		MailerCredentials.Create(password, credentials.Email);
}
