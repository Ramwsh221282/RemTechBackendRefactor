using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Brands;

/// <summary>
/// Название бренда.
/// </summary>
public sealed record BrandName
{
	private const int MAX_LENGTH = 128;

	private BrandName(string name)
	{
		Name = name;
	}

	/// <summary>
	/// Значение названия бренда.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Создаёт название бренда из строки.
	/// </summary>
	/// <param name="name">Строковое значение названия бренда.</param>
	/// <returns>Результат создания названия бренда.</returns>
	public static Result<BrandName> Create(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			return Error.Validation("Название бренда не может быть пустым.");
		}

		return name.Length > MAX_LENGTH
			? Error.Validation($"Название бренда превышает {MAX_LENGTH} символов.")
			: new BrandName(name);
	}
}
