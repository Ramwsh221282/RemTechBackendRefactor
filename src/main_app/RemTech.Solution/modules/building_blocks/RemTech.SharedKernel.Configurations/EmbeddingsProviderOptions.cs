namespace RemTech.SharedKernel.Configurations;

/// <summary>
/// Конфигурация для провайдера эмбеддингов.
/// </summary>
public sealed class EmbeddingsProviderOptions
{
	/// <summary>
	/// Путь к токенизатору.
	/// </summary>
	public string TokenizerPath { get; set; } = string.Empty;

	/// <summary>
	/// Путь к модели.
	/// </summary>
	public string ModelPath { get; set; } = string.Empty;

	/// <summary>
	/// Валидировать настройки провайдера эмбеддингов.
	/// </summary>
	/// <returns>Проверка успешна или нет.</returns>
	/// <exception cref="InvalidOperationException">Пути не могут быть пустыми или содержать только пробелы.</exception>
	public bool Validate()
	{
		if (string.IsNullOrWhiteSpace(TokenizerPath))
			throw new InvalidOperationException("Tokenizer path is empty.");
		return string.IsNullOrWhiteSpace(ModelPath)
			? throw new InvalidOperationException("Model path is empty.")
			: true;
	}
}
