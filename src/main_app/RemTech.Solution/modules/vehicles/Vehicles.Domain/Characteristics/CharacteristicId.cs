using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Characteristics;

public readonly record struct CharacteristicId
{
	public CharacteristicId()
	{
		Value = Guid.NewGuid();
	}

	private CharacteristicId(Guid value)
	{
		Value = value;
	}

	public Guid Value { get; }

	public static Result<CharacteristicId> Create(Guid value)
	{
		return value == Guid.Empty
			? (Result<CharacteristicId>)Error.Validation("Идентификатор характеристики не может быть пустым.")
			: (Result<CharacteristicId>)new CharacteristicId(value);
	}
}
