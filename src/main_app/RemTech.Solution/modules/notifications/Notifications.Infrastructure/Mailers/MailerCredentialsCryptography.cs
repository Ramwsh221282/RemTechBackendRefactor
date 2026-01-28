using Notifications.Core.Mailers;
using Notifications.Core.Mailers.Contracts;
using RemTech.SharedKernel.Infrastructure.AesEncryption;

namespace Notifications.Infrastructure.Mailers;

/// <summary>
/// Криптография учетных данных почтового ящика.
/// </summary>
/// <param name="cryptography">Криптографический сервис для шифрования и дешифрования.</param>
public sealed class MailerCredentialsCryptography(AesCryptography cryptography) : IMailerCredentialsCryptography
{
	private AesCryptography Cryptography { get; } = cryptography;

	/// <summary>
	/// Шифрует учетные данные почтового ящика.
	/// </summary>
	/// <param name="credentials">Учетные данные почтового ящика для шифрования.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Зашифрованные учетные данные почтового ящика.</returns>
	public async Task<MailerCredentials> Encrypt(MailerCredentials credentials, CancellationToken ct = default)
	{
		string encryptedPassword = await Cryptography.EncryptText(credentials.SmtpPassword, ct);
		return Recreate(credentials, encryptedPassword);
	}

	/// <summary>
	/// Дешифрует учетные данные почтового ящика.
	/// </summary>
	/// <param name="credentials">Учетные данные почтового ящика для дешифрования.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Дешифрованные учетные данные почтового ящика.</returns>
	public async Task<MailerCredentials> Decrypt(MailerCredentials credentials, CancellationToken ct = default)
	{
		string decryptedPassword = await Cryptography.DecryptText(credentials.SmtpPassword, ct);
		return Recreate(credentials, decryptedPassword);
	}

	private static MailerCredentials Recreate(MailerCredentials credentials, string password) =>
		MailerCredentials.Create(password, credentials.Email);
}
