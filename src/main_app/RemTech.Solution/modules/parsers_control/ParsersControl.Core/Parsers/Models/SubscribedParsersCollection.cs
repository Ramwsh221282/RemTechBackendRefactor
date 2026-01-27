using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

/// <summary>
/// Коллекция подписанных парсеров.
/// </summary>
/// <param name="parsers">Список подписанных парсеров.</param>
public sealed class SubscribedParsersCollection(IEnumerable<SubscribedParser> parsers)
{
	private SubscribedParsersCollection()
		: this([]) { }

	private SubscribedParser[] Parsers { get; set; } = [.. parsers];

	/// <summary>
	/// Создаёт пустую коллекцию подписанных парсеров.
	/// </summary>
	/// <returns>Пустая коллекция подписанных парсеров.</returns>
	public static SubscribedParsersCollection Empty() => new();

	/// <summary>
	/// Проверяет, пуста ли коллекция.
	/// </summary>
	/// <returns>True, если коллекция пуста; в противном случае false.</returns>
	public bool IsEmpty() => Parsers.Length == 0;

	/// <summary>
	/// Отключает все парсеры в коллекции.
	/// </summary>
	/// <returns>Результат операции отключения всех парсеров.</returns>
	public Result<Unit> PermanentlyDisableAll()
	{
		if (IsEmpty())
			return Error.NotFound("Список парсеров пуст.");
		foreach (SubscribedParser parser in Parsers)
			parser.Disable();

		return Result.Success(Unit.Value);
	}

	/// <summary>
	/// Включает все парсеры в коллекции.
	/// </summary>
	/// <returns>Результат операции включения всех парсеров.</returns>
	public Result<Unit> PermanentlyEnableAll()
	{
		if (IsEmpty())
			return Error.NotFound("Список парсеров пуст.");
		foreach (SubscribedParser parser in Parsers)
		{
			Result<Unit> enabling = parser.PermantlyEnable();
			if (enabling.IsFailure)
				return enabling.Error;
		}

		return Result.Success(Unit.Value);
	}

	/// <summary>
	/// Получает идентификаторы всех парсеров в коллекции.
	/// </summary>
	/// <returns>Идентификаторы всех парсеров в коллекции.</returns>
	public IEnumerable<Guid> GetIdentifiers() => Parsers.Select(p => p.Id.Value);

	/// <summary>
	/// Читает все парсеры в коллекции.
	/// </summary>
	/// <returns>Все подписанные парсеры в коллекции.</returns>
	public IEnumerable<SubscribedParser> Read() => Parsers;
}
