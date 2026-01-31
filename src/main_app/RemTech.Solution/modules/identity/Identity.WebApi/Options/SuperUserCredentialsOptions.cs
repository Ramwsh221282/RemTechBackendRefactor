using Microsoft.Extensions.Options;

namespace Identity.WebApi.Options;

/// <summary>
/// Настройки учетных данных суперпользователя.
/// </summary>
public sealed class SuperUserCredentialsOptions
{
	/// <summary>
	/// Логин суперпользователя.
	/// </summary>
	public string Login { get; set; } = string.Empty;

	/// <summary>
	/// Email суперпользователя.
	/// </summary>
	public string Email { get; set; } = string.Empty;

	/// <summary>
	/// Пароль суперпользователя.
	/// </summary>
	public string Password { get; set; } = string.Empty;

	/// <summary>
	/// Регистрация SuperUserCredentialsOptions из appsettings в контейнере служб.
	/// </summary>
	/// <param name="services">Контейнер служб для регистрации настроек.</param>
	/// <param name="section">Имя секции в файле конфигурации.</param>
	public static void AddFromAppsettings(
		IServiceCollection services,
		string section = nameof(SuperUserCredentialsOptions)
	)
	{
		services.AddOptions<IOptions<SuperUserCredentialsOptions>>().BindConfiguration(section);
	}

	/// <summary>
	/// Валидирует настройки учетных данных суперпользователя.
	/// </summary>
	/// <exception cref="InvalidOperationException">Выбрасывается, если одна из настроек пуста или содержит только пробелы.</exception>
	public void Validate()
	{
		if (string.IsNullOrWhiteSpace(Login))
		{
			throw new InvalidOperationException("Super user login is empty.");
		}

		if (string.IsNullOrWhiteSpace(Email))
		{
			throw new InvalidOperationException("Super user email is empty.");
		}

		if (string.IsNullOrWhiteSpace(Password))
		{
			throw new InvalidOperationException("Super user password is empty.");
		}
	}
}
