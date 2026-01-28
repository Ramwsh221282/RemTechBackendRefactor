namespace RemTech.SharedKernel.Configurations;

/// <summary>
/// Конфигурация для фронтенда.
/// </summary>
public sealed class FrontendOptions
{
	/// <summary>
	/// URL фронтенда.
	/// </summary>
	public string Url { get; set; } = string.Empty;

	/// <summary>
	/// Валидировать настройки фронтенда.
	/// </summary>
	/// <exception cref="ArgumentNullException">URL фронтенда не может быть пустым или содержать только пробелы.</exception>
	public void Validate()
	{
		if (string.IsNullOrWhiteSpace(Url))
			throw new ArgumentNullException($"Frontend URL option is empty. {nameof(FrontendOptions)}");
	}
}
