using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Models;

/// <summary>
/// Имя модели автомобиля.
/// </summary>
public sealed record ModelName
{
	private const int MAX_LENGTH = 128;

	private ModelName(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение имени модели.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создаёт имя модели из строки.
	/// </summary>
	/// <param name="value">Строковое значение имени модели.</param>
	/// <returns>Результат создания имени модели.</returns>
	public static Result<ModelName> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return Error.Validation("Имя модели не может быть пустым.");
		}

		return value.Length > MAX_LENGTH
			? Error.Validation($"Имя модели не может быть больше {MAX_LENGTH} символов.")
			: new ModelName(value);
	}
}
