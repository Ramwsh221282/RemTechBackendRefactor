using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Characteristics;

/// <summary>
/// Идентификатор характеристики транспортного средства.
/// </summary>
public readonly record struct CharacteristicId
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="CharacteristicId"/> с новым уникальным идентификатором.
	/// </summary>
	public CharacteristicId()
	{
		Value = Guid.NewGuid();
	}

	private CharacteristicId(Guid value)
	{
		Value = value;
	}

	/// <summary>
	/// Идентификатор характеристики транспортного средства.
	/// </summary>
	public Guid Value { get; }

	/// <summary>
	/// Создаёт идентификатор характеристики транспортного средства.
	/// </summary>
	/// <param name="value">Уникальный идентификатор характеристики.</param>
	/// <returns>Результат создания идентификатора характеристики.</returns>
	public static Result<CharacteristicId> Create(Guid value)
	{
		return value == Guid.Empty
			? (Result<CharacteristicId>)Error.Validation("Идентификатор характеристики не может быть пустым.")
			: (Result<CharacteristicId>)new CharacteristicId(value);
	}
}
