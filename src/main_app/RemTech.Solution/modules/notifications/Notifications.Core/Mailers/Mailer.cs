using Notifications.Core.Mailers.Contracts;

namespace Notifications.Core.Mailers;

/// <summary>
/// Представляет почтовую рассылку.
/// </summary>
/// <param name="id">Идентификатор почтовой рассылки.</param>
/// <param name="credentials">Учетные данные почтовой рассылки.</param>
public sealed class Mailer(MailerId id, MailerCredentials credentials)
{
	private Mailer(Mailer mailer)
		: this(mailer.Id, mailer.Credentials) { }

	/// <summary>
	/// Идентификатор почтовой рассылки.
	/// </summary>
	public MailerId Id { get; } = id;

	/// <summary>
	/// Учетные данные почтовой рассылки.
	/// </summary>
	public MailerCredentials Credentials { get; private set; } = credentials;

	/// <summary>
	/// Создает новую почтовую рассылку с заданными учетными данными.
	/// </summary>
	/// <param name="credentials">Учетные данные почтовой рассылки.</param>
	/// <returns>Новая почтовая рассылка.</returns>
	public static Mailer CreateNew(MailerCredentials credentials)
	{
		MailerId id = MailerId.New();
		return new Mailer(id, credentials);
	}

	/// <summary>
	/// Изменяет учетные данные почтовой рассылки.
	/// </summary>
	/// <param name="credentials">Новые учетные данные почтовой рассылки.</param>
	public void ChangeCredentials(MailerCredentials credentials) => Credentials = credentials;

	/// <summary>
	/// Шифрует учетные данные почтовой рассылки.
	/// </summary>
	/// <param name="cryptography">Объект для выполнения криптографических операций.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию шифрования.</returns>
	public async Task EncryptCredentials(IMailerCredentialsCryptography cryptography, CancellationToken ct = default) =>
		Credentials = await Credentials.Encrypt(cryptography, ct);

	/// <summary>
	/// Дешифрует учетные данные почтовой рассылки.
	/// </summary>
	/// <param name="cryptography">Объект для выполнения криптографических операций.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию дешифрования.</returns>
	public async Task DecryptCredentials(IMailerCredentialsCryptography cryptography, CancellationToken ct = default) =>
		Credentials = await Credentials.Decrypt(cryptography, ct);

	/// <summary>
	/// Создает копию текущей почтовой рассылки.
	/// </summary>
	/// <returns>Копия текущей почтовой рассылки.</returns>
	public Mailer Copy() => new(this);
}
