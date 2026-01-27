namespace RemTech.SharedKernel.Configurations;

/// <summary>
/// Конфигурация для RabbitMQ.
/// </summary>
public class RabbitMqOptions
{
	/// <summary>
	/// Хостname RabbitMQ.
	/// </summary>
	public string Hostname { get; set; } = string.Empty;

	/// <summary>
	/// Пароль для подключения к RabbitMQ.
	/// </summary>
	public string Password { get; set; } = string.Empty;

	/// <summary>
	/// Имя пользователя для подключения к RabbitMQ.
	/// </summary>
	public string Username { get; set; } = string.Empty;

	/// <summary>
	/// Порт RabbitMQ.
	/// </summary>
	public int Port { get; set; } = -1;

	/// <summary>
	/// Валидировать настройки RabbitMQ.
	/// </summary>
	/// <exception cref="ArgumentException">Выбрасывается, если одна из настроек не установлена.</exception>
	public void Validate()
	{
		if (string.IsNullOrWhiteSpace(Hostname))
			throw new ArgumentException("Cannot use RabbitMq Options. Hostname was not set.");
		if (string.IsNullOrWhiteSpace(Username))
			throw new ArgumentException("Cannot use RabbitMq Options. Username was not set.");
		if (string.IsNullOrWhiteSpace(Password))
			throw new ArgumentException("Cannot use RabbitMq Options. Password was not set.");
		if (Port <= 0)
			throw new ArgumentException("Cannot use RabbitMq Options. Port was not set.");
	}
}
