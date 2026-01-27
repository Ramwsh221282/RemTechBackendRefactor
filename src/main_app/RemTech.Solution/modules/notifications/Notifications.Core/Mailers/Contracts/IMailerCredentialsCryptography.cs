namespace Notifications.Core.Mailers.Contracts;

/// <summary>
/// Контракт для шифрования и дешифрования учетных данных почтовых рассылок.
/// </summary>
public interface IMailerCredentialsCryptography
{
	/// <summary>
	/// Шифрует учетные данные почтовой рассылки.
	/// </summary>
	/// <param name="credentials">Учетные данные почтовой рассылки для шифрования.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию шифрования.</returns>
	public Task<MailerCredentials> Encrypt(MailerCredentials credentials, CancellationToken ct = default);

	/// <summary>
	/// Дешифрует учетные данные почтовой рассылки.
	/// </summary>
	/// <param name="credentials">Учетные данные почтовой рассылки для дешифрования.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию дешифрования.</returns>
	public Task<MailerCredentials> Decrypt(MailerCredentials credentials, CancellationToken ct = default);
}
