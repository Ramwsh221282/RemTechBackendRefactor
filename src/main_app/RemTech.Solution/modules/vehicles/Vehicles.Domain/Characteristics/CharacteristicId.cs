using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Characteristics;

public readonly record struct CharacteristicId
{
	public Guid Value { get; }

	public CharacteristicId()
	{
		Value = Guid.NewGuid();
	}

	private CharacteristicId(Guid value)
	{
		Value = value;
	}

	public static Result<CharacteristicId> Create(Guid value)
	{
		if (value == Guid.Empty)
			return Error.Validation("Идентификатор характеристики не может быть пустым.");
		return new CharacteristicId(value);
	}
}
