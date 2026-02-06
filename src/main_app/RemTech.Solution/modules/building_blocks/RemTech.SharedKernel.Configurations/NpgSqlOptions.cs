namespace RemTech.SharedKernel.Configurations;

/// <summary>
/// Конфигурация для подключения к базе данных PostgreSQL.
/// </summary>
public sealed class NpgSqlOptions
{
	/// <summary>
	/// Хост базы данных.
	/// </summary>
	public string Host { get; set; } = null!;

	/// <summary>
	/// Порт базы данных.
	/// </summary>
	public string Port { get; set; } = null!;

	/// <summary>
	/// Имя базы данных.
	/// </summary>
	public string Database { get; set; } = null!;

	/// <summary>
	/// Имя пользователя базы данных.
	/// </summary>
	public string Username { get; set; } = null!;

	/// <summary>
	/// Пароль пользователя базы данных.
	/// </summary>
	public string Password { get; set; } = null!;

	/// <summary>
	/// Преобразовать настройки в строку подключения.
	/// </summary>
	/// <returns>Строка подключения к базе данных PostgreSQL.</returns>
	public string ToConnectionString()
	{
		ValidateOptions();
		return CreateFromTemplate();
	}

	/// <summary>
	/// Создать строку подключения из шаблона.
	/// </summary>
	/// <returns>Строка подключения к базе данных PostgreSQL.</returns>
	private string CreateFromTemplate()
	{
		const string template = "Host={0};Port={1};Database={2};Username={3};Password={4};";
		return string.Format(template, Host, Port, Database, Username, Password);
	}

	/// <summary>
	/// Валидировать настройки подключения.
	/// </summary>
	/// <exception cref="ArgumentException">Исключение выбрасывается, если одна из настроек не установлена.</exception>
	private void ValidateOptions()
	{
		if (string.IsNullOrWhiteSpace(Host))
		{
			throw new ArgumentException("Cannot use NpgSql Options. Host was not set.");
		}
		if (string.IsNullOrWhiteSpace(Port))
		{
			throw new ArgumentException("Cannot use NpgSql Options. Port was not set.");
		}
		if (string.IsNullOrWhiteSpace(Database))
		{
			throw new ArgumentException("Cannot use NpgSql Options. Database was not set.");
		}
		if (string.IsNullOrWhiteSpace(Username))
		{
			throw new ArgumentException("Cannot use NpgSql Options. Username was not set.");
		}
		if (string.IsNullOrWhiteSpace(Password))
		{
			throw new ArgumentException("Cannot use NpgSql Options. Password was not set.");
		}
	}
}
