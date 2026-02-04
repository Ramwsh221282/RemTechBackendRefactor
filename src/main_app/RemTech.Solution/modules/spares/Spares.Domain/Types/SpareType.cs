using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Types;

/// <summary>
/// Тип запчасти.
/// </summary>
public sealed class SpareType
{
	private SpareType(string value, SpareTypeId id)
	{
		Value = value;
		Id = id;
	}

	/// <summary>
	/// Значение типа запчасти.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Идентификатор типа запчасти.
	/// </summary>
	public SpareTypeId Id { get; }

	/// <summary>
	/// Создаёт тип запчасти из строки.
	/// </summary>
	/// <param name="value">Строковое представление типа запчасти.</param>
	/// <returns>Результат создания типа запчасти.</returns>
	public static Result<SpareType> Create(string value)
	{
		SpareTypeId id = SpareTypeId.New();
		return string.IsNullOrWhiteSpace(value)
			? Error.Validation("Тип запчасти не может быть пустым")
			: new SpareType(value, id);
	}

	public static Result<SpareType> Create(string value, Guid id)
	{
		Result<SpareTypeId> spareTypeIdResult = SpareTypeId.FromGuid(id);
		if (spareTypeIdResult.IsFailure)
		{
			return spareTypeIdResult.Error;
		}

		return string.IsNullOrWhiteSpace(value)
			? Error.Validation("Тип запчасти не может быть пустым")
			: new SpareType(value, spareTypeIdResult.Value);
	}
}
