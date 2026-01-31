using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserLinks.Models;

/// <summary>
/// Информация о ссылке на подписанный парсер.
/// </summary>
public sealed record SubscribedParserLinkUrlInfo
{
	private const int MAX_NAME_LENGTH = 255;

	private SubscribedParserLinkUrlInfo(string url, string name)
	{
		Url = url;
		Name = name;
	}

	/// <summary>
	/// Ссылка на парсер.
	/// </summary>
	public string Url { get; private init; }

	/// <summary>
	/// Название ссылки на парсер.
	/// </summary>
	public string Name { get; private init; }

	/// <summary>
	/// Создаёт информацию о ссылке на парсер.
	/// </summary>
	/// <param name="url">Ссылка на парсер.</param>
	/// <param name="name">Название ссылки.</param>
	/// <returns>Результат создания информации о ссылке.</returns>
	public static Result<SubscribedParserLinkUrlInfo> Create(string url, string name)
	{
		if (string.IsNullOrWhiteSpace(url))
		{
			return Error.Validation("Ссылка на парсер не может быть пустой.");
		}

		if (string.IsNullOrWhiteSpace(name))
		{
			return Error.Validation("Название ссылки на парсер не может быть пустым.");
		}

		return name.Length > MAX_NAME_LENGTH
			? Error.Validation($"Название ссылки на парсер не может быть больше {MAX_NAME_LENGTH} символов.")
			: new SubscribedParserLinkUrlInfo(url, name);
	}

	/// <summary>
	/// Переименовать ссылку.
	/// </summary>
	/// <param name="otherName">Новое имя.</param>
	/// <returns>Результат переименования.</returns>
	public Result<SubscribedParserLinkUrlInfo> Rename(string otherName)
	{
		return Create(Url, otherName);
	}

	/// <summary>
	/// Изменить url ссылки.
	/// </summary>
	/// <param name="otherUrl">Новый url.</param>
	/// <returns>Результат изменения url.</returns>
	public Result<SubscribedParserLinkUrlInfo> ChangeUrl(string otherUrl)
	{
		return Create(otherUrl, Name);
	}

	/// <summary>
	/// Создать копию информации о ссылке.
	/// </summary>
	/// <returns>Копия информации о ссылке.</returns>
	public SubscribedParserLinkUrlInfo Copy()
	{
		return new(Url, Name);
	}
}
